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

		private bool Generated => traitInfo != null && skillInfo != null && backStoryInfo != null;

		public Dictionary<string, bool> TraitInfo {
			get {
				if (traitInfo == null) GenerateComp(pawn);
				return traitInfo;
			}
		}

		public Dictionary<string, bool> SkillInfo {
			get {
				if (skillInfo == null) GenerateComp(pawn);
				return skillInfo;
			}
		}

		public List<bool> BackStoryInfo {
			get {
				if (backStoryInfo == null) GenerateComp(pawn);
				return backStoryInfo;
			}
		}

		private Dictionary<string, bool> traitInfo, skillInfo;
		private List<bool> backStoryInfo;

		CompProperties_PawnInfo Props => props as CompProperties_PawnInfo;

		public override void Initialize(CompProperties props) {

		}

		public override void PostSpawnSetup(bool respawningAfterLoad) {
			GenerateComp(pawn);
		}

		public List<Trait> GetAvaliableTraits(Thought thought = null, int relation = int.MinValue, MentalBreakDef mentalBreak = null) =>
			 pawn.story.traits.allTraits.FindAll(x => FilterTrait(pawn, x, thought, relation, mentalBreak));


		public List<SkillRecord> GetAvaliableSkills(Thought thought = null, int relation = int.MinValue) =>
			pawn.skills.skills.FindAll(x => FilterSkill(pawn, x, thought, relation));

		public static void GenerateComp(Pawn pawn) {
			var info = pawn.GetComp<CompPawnInfo>();
			if (pawn.story == null) return;
			if (info.Generated) return;
			info.traitInfo = new Dictionary<string, bool>();
			info.skillInfo = new Dictionary<string, bool>();
			info.backStoryInfo = new List<bool>() { false, false };
			foreach (var trait in pawn.story.traits.allTraits)
				info.traitInfo[trait.def.defName] = FilterTrait(pawn, trait);
			foreach (var skill in pawn.skills.skills)
				info.skillInfo[skill.def.defName] = FilterSkill(pawn, skill);
			if (pawn.Faction.IsPlayer) info.dayJoined = 0;
			else info.dayJoined = int.MaxValue;
		}

		public static bool FilterTrait(Pawn pawn, Trait trait, Thought thought = null, int relation = int.MinValue, MentalBreakDef mentalBreak = null) {
			var setting = WhoYouAreModSettings.traitSettings[trait.def.defName];
			// always shown
			if (setting.ForceShown) return true;
			// trait specific thought
			if (Current.ProgramState == ProgramState.Playing) {
				if (thought != null && setting.GainFromThought && thought.def.requiredTraits.Concat(thought.def.nullifyingTraits).Contains(trait.def)) return true;
				// close relation
				if (setting.GainFromInteraction && relation > setting.RelationThreshold) return true;
				// joined for a while
				if (setting.GainByTime && setting.MinimumTimePassed < GenDate.DaysPassed - pawn.GetComp<CompPawnInfo>().dayJoined) return true;
				// trait specific break
				if (mentalBreak != null && setting.GainFromBreak && trait.def.defName == mentalBreak.requiredTrait.defName) return true;
				// only one left
				if (pawn.Faction.IsPlayer && PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.Count == 1) return true;
			}
			return false;
		}

		public static bool FilterSkill(Pawn pawn, SkillRecord skill, Thought thought = null, int relation = int.MinValue) {
			// i am master
			WhoYouAreModSettings.skillSettings.ToList().ForEach(x => Log.Message(x.Key + " " + x.Value));
			Log.Message(skill.def.defName);
			var setting = WhoYouAreModSettings.skillSettings[skill.def.defName];
			if (setting.ForceShown) return true;
			if (skill.Level > 14) return true;
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
