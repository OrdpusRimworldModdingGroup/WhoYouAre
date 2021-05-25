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
		public int dayJoined;

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

		public bool[] BackStoryInfo {
			get {
				if (backStoryInfo == null) GenerateComp(pawn);
				return backStoryInfo;
			}
		}

		private Dictionary<string, bool> traitInfo, skillInfo;
		private bool[] backStoryInfo;

		CompProperties_PawnInfo Props => props as CompProperties_PawnInfo;

		public override void Initialize(CompProperties props) {

		}

		public List<Trait> GetAvaliableTraits(Thought thought = null, int relation = int.MinValue, int time = -1, MentalBreakDef mentalBreak = null) =>
			 pawn.story.traits.allTraits.FindAll(x => FilterTrait(pawn, x, thought, relation, time, mentalBreak));


		public List<SkillRecord> GetAvaliableSkills(Thought thought = null, int relation = int.MinValue, int time = -1) =>
			pawn.skills.skills.FindAll(x => FilterSkill(pawn, x, thought, relation, time));

		public static void GenerateComp(Pawn pawn) {
			var info = pawn.GetComp<CompPawnInfo>();
			if (pawn.story == null) return;
			info.traitInfo = new Dictionary<string, bool>();
			info.skillInfo = new Dictionary<string, bool>();
			info.backStoryInfo = new bool[2];
			foreach (var trait in pawn.story.traits.allTraits)
				info.traitInfo[trait.def.defName] = FilterTrait(pawn, trait);
			foreach (var skill in pawn.skills.skills)
				info.skillInfo[skill.def.defName] = FilterSkill(pawn, skill);
			if (pawn.Faction.IsPlayer) info.dayJoined = 0;
			else info.dayJoined = int.MinValue;
		}

		public static bool FilterTrait(Pawn pawn, Trait trait, Thought thought = null, int relation = int.MinValue, int time = -1, MentalBreakDef mentalBreak = null) {
			var setting = WhoYouAreModSettings.traitSettings[trait.def.defName];
			// always shown
			if (setting.ForceShown) return true;
			// trait specific thought
			if (thought != null && setting.GainFromThought && thought.def.requiredTraits.Concat(thought.def.nullifyingTraits).Contains(trait.def)) return true;
			// close relation
			if (setting.GainFromInteraction && relation > setting.RelationThreshold) return true;
			// joined for a while
			if (time > GenDate.DaysPassed - pawn.GetComp<CompPawnInfo>().dayJoined) return true;
			// trait specific break
			if (mentalBreak != null && setting.GainFromBreak && trait.def.defName == mentalBreak.requiredTrait.defName) return true;
			// only one left
			if (pawn.Faction.IsPlayer && PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.Count == 1) return true;
			return false;
		}

		public static bool FilterSkill(Pawn pawn, SkillRecord skill, Thought thought = null, int relation = int.MinValue, int time = -1) {
			// i am master
			if (skill.Level > 14) return true;
			// joined for a while
			if (time > GenDate.DaysPassed - pawn.GetComp<CompPawnInfo>().dayJoined) return true;
			// passion
			if (skill.passion != Passion.None && pawn.jobs.curDriver.ActiveSkill == skill.def) return true;
			return false;
		}

		public override void PostExposeData() {
			base.PostExposeData();
			Scribe_Collections.Look(ref traitInfo, "traitInfo");
			Scribe_Collections.Look(ref skillInfo, "skillInfo");
			var list = backStoryInfo.ToList();
			Scribe_Collections.Look(ref list, "backStoryInfo");
			backStoryInfo = list.ToArray();
		}

	}
}
