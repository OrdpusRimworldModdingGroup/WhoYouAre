//#define DEBUGFilterTrait
#define DEBUGGenerateComp
//#define DEBUGDisableTags

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Verse;
using RimWorld;
using HarmonyLib;

namespace WhoYouAre {
	public class CompPawnInfo : ThingComp {

		private static bool workTab = WhoYouAreMod.WorkTab;
		public Pawn pawn => parent as Pawn;
		public int dayJoined = int.MaxValue;

		private static Random random = new Random();
		private static FieldInfo pawnPrioritiesInfo = AccessTools.DeclaredField(typeof(Pawn_WorkSettings), "priorities");

		private bool Generated => traitInfo != null && skillInfo != null && backStoryInfo != null && workInfo != null;

		private Dictionary<string, bool> TraitInfo {
			get {
				if (traitInfo == null) {
					double r1, r2;
					(r1, r2) = GetChances();
					GenerateComp(pawn, r1, r2);
				}
				foreach (var trait in pawn.story.traits.allTraits)
					if (!traitInfo.ContainsKey(trait.def.defName)) traitInfo[trait.def.defName] = false;
				return traitInfo;
			}
		}

		private Dictionary<string, bool> SkillInfo {
			get {
				if (skillInfo == null) {
					double r1, r2;
					(r1, r2) = GetChances();
					GenerateComp(pawn, r1, r2);
				}
				return skillInfo;
			}
		}

		public List<bool> BackStoryInfo {
			get {
				if (backStoryInfo == null) {
					double r1, r2;
					(r1, r2) = GetChances();
					GenerateComp(pawn, r1, r2);
				}
				return backStoryInfo;
			}
		}

		private Dictionary<WorkTypeDef, int> WorkInfo {
			get {
				if (workInfo == null) {
					double r1, r2;
					(r1, r2) = GetChances();
					GenerateComp(pawn, r1, r2);
				}
				return workInfo;
			}
		}

		private Dictionary<WorkGiverDef, int[]> WorkGiverInfo {
			get {
				if (workGiverInfo == null) {
					double r1, r2;
					(r1, r2) = GetChances();
					GenerateComp(pawn, r1, r2);
				}
				return workGiverInfo;
			}
		}

		public bool TraitState(Trait trait) {
			if (TraitInfo.ContainsKey(trait.def.defName)) return TraitInfo[trait.def.defName];
			var result = FilterTrait(pawn, trait);
			TraitInfo[trait.def.defName] = result;
			return result;
		}

		public bool SkillState(SkillRecord skill) {
			if (SkillInfo.ContainsKey(skill.def.defName)) return SkillInfo[skill.def.defName];
			var result = FilterSkill(pawn, skill);
			SkillInfo[skill.def.defName] = result;
			return result;
		}

		public int WorkState(WorkTypeDef work) => WorkInfo[work];

		public int WorkState(WorkTypeDef work, int hour = -1) {
			hour = GetHour(hour);
			return work.WorkGivers().Min(x => WorkGiverInfo[x][hour]);
		}

		public int[] WorkStates(WorkTypeDef work) => ModUtils.WholeDay.Select(x => WorkState(work, x)).ToArray();

		public int WorkState(WorkGiverDef work, int hour = -1) => WorkGiverInfo[work][GetHour(hour)];

		public int[] WorkStates(WorkGiverDef work) => WorkGiverInfo[work].Clone() as int[];

		public void SetTraitState(Trait trait, bool state = false) => TraitInfo[trait.def.defName] = state;


		public void SetSkillState(SkillRecord skill, bool state = false) => SkillInfo[skill.def.defName] = state;

		public void SetWorkState(WorkTypeDef work, int state = 0) {
			WorkInfo[work] = state;
		}


		public void SetWorkState(WorkGiverDef work, int state = 0, int hour = -1) {
			//Log.Message(pawn.Name.ToStringFull + " " + work.workType.defName + " " + work.defName + " " + state + " " + hour);
			WorkGiverInfo[work][GetHour(hour)] = state;
		}

		private Dictionary<string, bool> traitInfo, skillInfo;
		private Dictionary<WorkTypeDef, int> workInfo;
		private Dictionary<WorkGiverDef, int[]> workGiverInfo;
		private List<bool> backStoryInfo;

		CompProperties_PawnInfo Props => props as CompProperties_PawnInfo;

		public List<Trait> GetKnownTraits() => pawn.story.traits.allTraits.FindAll(x => TraitState(x));

		public List<SkillRecord> GetKnownSkills() => pawn.skills.skills.FindAll(x => SkillState(x));

		public List<Backstory> GetKnownBackstories() {
			var result = new List<Backstory>(2);
			if (BackStoryInfo[0]) result.Add(pawn.story.childhood);
			if (BackStoryInfo[1]) result.Add(pawn.story.adulthood);
			return result;
		}

		public List<Trait> GetAvaliableTraits(Thought_Situational thought = null, int relation = int.MinValue, MentalBreakDef mentalBreak = null) =>
			 pawn.story.traits.allTraits.FindAll(x => FilterTrait(pawn, x, thought, relation, mentalBreak));


		public List<SkillRecord> GetAvaliableSkills(int relation = int.MinValue) =>
			pawn.skills.skills.FindAll(x => FilterSkill(pawn, x, relation));

		private (double, double) GetChances() {
			double traitChance = 0, skillChance = 0;
			if (TradeSession.Active || pawn.IsQuestLodger()) {
				traitChance = WhoYouAreModSettings.TradeTraitChance;
				skillChance = WhoYouAreModSettings.TradeSkillChance;
			}
			return (traitChance, skillChance);
		}

		public WorkTags DisabledWorkTags(bool ignoreHealth = true) {
#if DEBUGDisableTags
			int debug_index = 0;
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			if (ModUtils.StartingOrDebug()) return pawn.CombinedDisabledWorkTags;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			var story = pawn.story;
			var storyInfo = pawn.GetComp<CompPawnInfo>().BackStoryInfo;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			var traits = pawn.GetComp<CompPawnInfo>().GetKnownTraits();
			var result = WorkTags.None;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			if (storyInfo[0]) result |= story?.childhood?.workDisables ?? WorkTags.None;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			if (storyInfo[1]) result |= story?.adulthood?.workDisables ?? WorkTags.None;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			foreach (var trait in traits)
				result |= trait.def.disabledWorkTags;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			pawn?.royalty?.AllTitlesForReading.ForEach(x => result |= x.conceited ? x.def.disabledWorkTags : WorkTags.None);
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			if (!ignoreHealth) pawn?.health?.hediffSet?.hediffs.ForEach(x => result |= x?.CurStage?.disabledWorkTags ?? WorkTags.None);
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			foreach (QuestPart_WorkDisabled q in QuestUtility.GetWorkDisabledQuestPart(pawn))
				result |= q.disabledWorkTags;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			return result;
		}

		public List<WorkTypeDef> DisabledWorkTypes(bool ignoreHealth = true) => pawn.GetDisabledWorkTypes().FindAll(x => (x.workTags & DisabledWorkTags(ignoreHealth)) != 0);

		public bool WorkTypeDisabled(WorkTypeDef workDef, bool ignoreHealth = true) => (DisabledWorkTags(ignoreHealth) & workDef.workTags) != 0;



		public static void GenerateComp(Pawn pawn, double traitRate = 0, double skillRate = 0) {
#if DEBUGGenerateComp
			int i = 0;
			Log.Message("GenerateComp " + i++);
#endif
			var info = pawn.GetComp<CompPawnInfo>();
			if (pawn.story == null) return;
			if (info.Generated) return;
#if DEBUGGenerateComp
			Log.Message("GenerateComp " + i++);
#endif
			info.traitInfo = new Dictionary<string, bool>();
			info.skillInfo = new Dictionary<string, bool>();
			info.workInfo = new Dictionary<WorkTypeDef, int>();
			if (workTab) info.workGiverInfo = new Dictionary<WorkGiverDef, int[]>();
			info.backStoryInfo = new List<bool>() { false, false };
			foreach (var trait in pawn.story.traits.allTraits) {
#if DEBUGGenerateComp
				int j = 0;
				Log.Message("GenerateComp traits " + trait.def.defName + " " + i + " " + j++);
#endif
				if (random.NextDouble() < traitRate) info.traitInfo[trait.def.defName] = true;
				else info.traitInfo[trait.def.defName] = FilterTrait(pawn, trait);

#if DEBUGGenerateComp
				Log.Message("GenerateComp traits " + trait.def.defName + " " + i + " " + j++);
#endif
			}
			foreach (var skill in pawn.skills.skills) {
#if DEBUGGenerateComp
				int j = 0;
				Log.Message("GenerateComp skills " + skill.def.defName + " " + i + " " + j++);
#endif
				if (random.NextDouble() < skillRate) info.skillInfo[skill.def.defName] = true;
				else info.skillInfo[skill.def.defName] = FilterSkill(pawn, skill);
#if DEBUGGenerateComp
				Log.Message("GenerateComp skills " + skill.def.defName + " " + i + " " + j++);
#endif
			}

			static void GetPriority(CompPawnInfo comp, WorkTypeDef work, int priority) {
				comp.workInfo[work] = priority;
				if (workTab) work.workGiversByPriority.ForEach(x => comp.workGiverInfo[x] = new int[24].Select(x => priority).ToArray());
			}

			if (pawn.IsColonist) {
				var workSetting = pawnPrioritiesInfo.GetValue(pawn.workSettings) as DefMap<WorkTypeDef, int>;
				if (workSetting == null)
					foreach (var work in DefDatabase<WorkTypeDef>.AllDefs) GetPriority(info, work, 3);
				else foreach (var work in workSetting) GetPriority(info, work.Key, work.Value);
				info.dayJoined = 0;
			} else info.dayJoined = int.MaxValue;
#if DEBUGGenerateComp
			Log.Message("GenerateComp " + i++);
#endif
		}

		public static bool FilterTrait(Pawn pawn, Trait trait, Thought_Situational thought = null, int relation = -1000, MentalBreakDef mentalBreak = null) {
#if DEBUGFilterTrait
			int i = 0;
			Log.Message("FilterTrait " + i++);
#endif
			var setting = WhoYouAreModSettings.traitSettings[trait.def.defName];
#if DEBUGFilterTrait
			Log.Message("FilterTrait " + i++);
#endif
			// always shown
			if (setting.ForceShown) return true;
			// trait specific thought
			if (Current.ProgramState == ProgramState.Playing) {
#if DEBUGFilterTrait
				int j = 0;
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				if (thought != null && setting.GainFromThought) {
					thought.RecalculateState();
					bool hasTrait = thought.def.requiredTraits?.Contains(trait.def) ?? false;
					if (!hasTrait) hasTrait |= thought.def.nullifyingTraits?.Contains(trait.def) ?? false;
					if (thought.Active && hasTrait) return true;
				}
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				// close relation
				if (setting.GainFromInteraction && relation > setting.RelationThreshold) return true;
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				// joined for a while
				//if (setting.GainByTime && setting.MinimumTimePassed < GenDate.DaysPassed - pawn.GetComp<CompPawnInfo>().dayJoined) return true;
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				// trait specific break
				if (mentalBreak != null && setting.GainFromBreak && trait.def.defName == (mentalBreak.requiredTrait?.defName ?? "")) return true;
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				// only one left
				//if (pawn.Faction?.IsPlayer ?? false && PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.Count == 1) return true;
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
			}
			return false;
		}

		public static bool FilterSkill(Pawn pawn, SkillRecord skill, int relation = int.MinValue) {
			// i am master
			var setting = WhoYouAreModSettings.skillSettings[skill.def.defName];
			if (setting.ForceShown) return true;
			if (setting.GainFromMaster && skill.Level > 14) return true;
			if (Current.ProgramState == ProgramState.Playing) {
				if (setting.GainFromInteraction && relation > setting.RelationThreshold) return true;
				// joined for a while
				//if (setting.GainByTime && setting.MinimumTimePassed < GenDate.DaysPassed - pawn.GetComp<CompPawnInfo>().dayJoined) return true;
				// passion
				if (skill.passion != Passion.None && pawn.jobs?.curDriver?.ActiveSkill == skill.def) return true;
			}
			return false;
		}

		private int GetHour(int hour = -1) {
			return hour < 0 ? GenLocalDate.HourOfDay(pawn) : hour;
		}

		public override void PostExposeData() {
			base.PostExposeData();
			Scribe_Collections.Look(ref traitInfo, "WYA.traitInfo");
			Scribe_Collections.Look(ref skillInfo, "WYA.skillInfo");
			Scribe_Collections.Look(ref backStoryInfo, "WYA.backStoryInfo");
			Scribe_Collections.Look(ref workInfo, "WYA.workInfo");
			if (workTab) Scribe_Collections.Look(ref workGiverInfo, "WYA.workGiverInfo");
			Scribe_Values.Look(ref dayJoined, "WYA.dayJoined");
		}

	}
}
