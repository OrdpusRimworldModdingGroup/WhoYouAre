//#define DEBUGFilterTrait
//#define DEBUGGenerateComp

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace WhoYouAre {
	public class CompPawnInfo : ThingComp {

		public Pawn pawn => parent as Pawn;
		public int dayJoined = int.MaxValue;

		private static Random random = new Random();

		private bool Generated => traitInfo != null && skillInfo != null && backStoryInfo != null;

		private Dictionary<string, bool> TraitInfo {
			get {
				if (traitInfo == null) GenerateComp(pawn);
				foreach (var trait in pawn.story.traits.allTraits)
					if (!traitInfo.ContainsKey(trait.def.defName)) traitInfo[trait.def.defName] = false;
				return traitInfo;
			}
		}

		private Dictionary<string, bool> SkillInfo {
			get {
				double traitChance = 0, skillChance = 0;
				if (TradeSession.Active) {
					traitChance = WhoYouAreModSettings.TradeTraitChance;
					skillChance = WhoYouAreModSettings.TradeSkillChance;
				}
				if (skillInfo == null) GenerateComp(pawn);
				return skillInfo;
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

		public void SetTraitState(Trait trait, bool state = false) {
			TraitInfo[trait.def.defName] = state;
		}

		public void SetSkillState(SkillRecord skill, bool state = false) {
			SkillInfo[skill.def.defName] = state;
		}

		public List<bool> BackStoryInfo {
			get {
				double traitChance = 0, skillChance = 0;
				if (TradeSession.Active) {
					traitChance = WhoYouAreModSettings.TradeTraitChance;
					skillChance = WhoYouAreModSettings.TradeSkillChance;
				}
				if (backStoryInfo == null) GenerateComp(pawn, traitChance, skillChance);
				return backStoryInfo;
			}
		}

		private Dictionary<string, bool> traitInfo, skillInfo;
		private List<bool> backStoryInfo;

		CompProperties_PawnInfo Props => props as CompProperties_PawnInfo;

		public override void Initialize(CompProperties props) {

		}

		public override void PostSpawnSetup(bool respawningAfterLoad) {
			//GenerateComp(pawn);
		}

		public List<Trait> GetAvaliableTraits(Thought_Situational thought = null, int relation = int.MinValue, MentalBreakDef mentalBreak = null) =>
			 pawn.story.traits.allTraits.FindAll(x => FilterTrait(pawn, x, thought, relation, mentalBreak));


		public List<SkillRecord> GetAvaliableSkills(int relation = int.MinValue) =>
			pawn.skills.skills.FindAll(x => FilterSkill(pawn, x, relation));

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
#if DEBUGGenerateComp
			Log.Message("GenerateComp " + i++);
#endif
			if (pawn.Faction?.IsPlayer ?? false) info.dayJoined = 0;
			else info.dayJoined = int.MaxValue;
#if DEBUGGenerateComp
			Log.Message("GenerateComp " + i++);
#endif
		}

		public static bool FilterTrait(Pawn pawn, Trait trait, Thought_Situational thought = null, int relation = int.MinValue, MentalBreakDef mentalBreak = null) {
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
				if (thought != null && setting.GainFromThought && thought.Active && ((thought.def.requiredTraits?.Contains(trait.def) ?? false) || (thought.def.nullifyingTraits?.Contains(trait.def) ?? false))) return true;
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				// close relation
				if (setting.GainFromInteraction && relation > setting.RelationThreshold) return true;
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				// joined for a while
				if (setting.GainByTime && setting.MinimumTimePassed < GenDate.DaysPassed - pawn.GetComp<CompPawnInfo>().dayJoined) return true;
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				// trait specific break
				if (mentalBreak != null && setting.GainFromBreak && trait.def.defName == (mentalBreak.requiredTrait?.defName ?? "")) return true;
#if DEBUGFilterTrait
				Log.Message("FilterTrait " + i + " " + j++);
#endif
				// only one left
				if (pawn.Faction?.IsPlayer ?? false && PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.Count == 1) return true;
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
				if (setting.GainByTime && setting.MinimumTimePassed < GenDate.DaysPassed - pawn.GetComp<CompPawnInfo>().dayJoined) return true;
				// passion
				if (skill.passion != Passion.None && pawn.jobs?.curDriver?.ActiveSkill == skill.def) return true;
			}
			return false;
		}

		public override void PostExposeData() {
			base.PostExposeData();
			Scribe_Collections.Look(ref traitInfo, "traitInfo");
			Scribe_Collections.Look(ref skillInfo, "skillInfo");
			Scribe_Collections.Look(ref backStoryInfo, "backStoryInfo");
			Scribe_Values.Look(ref dayJoined, "dayJoined");
		}

	}
}
