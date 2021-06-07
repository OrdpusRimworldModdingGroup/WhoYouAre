//#define DEBUGDisableTags

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using WhoYouAre;
using UnityEngine;

namespace WhoYouAre.HarmonyWYA {

	[HarmonyPatch(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.AverageOfRelevantSkillsFor))]
	internal class Rimworld__Pawn_SkillTracker__AverageOfRelevantSkillsFor {

		private static FieldInfo Pawn_SkillTrackerPawnInfo = AccessTools.DeclaredField(typeof(Pawn_SkillTracker), "pawn");

		internal static bool Prefix(Pawn_SkillTracker __instance, ref float __result, WorkTypeDef workDef) {
			if (ModUtils.StartingOrDebug()) return true;
			if (workDef.relevantSkills.Count == 0) {
				__result = 3f;
			} else {
				var comp = (Pawn_SkillTrackerPawnInfo.GetValue(__instance) as Pawn).GetComp<CompPawnInfo>();
				float num = 0f;
				int count = 0;
				for (int i = 0; i < workDef.relevantSkills.Count; i++) {
					if (comp.SkillState(__instance.GetSkill(workDef.relevantSkills[i]))) {
						num += __instance.GetSkill(workDef.relevantSkills[i]).Level;
						count++;
					}
				}
				if (count == 0) __result = 3f;
				else __result = num / count;
			}
			return false;
		}

	}

	[HarmonyPatch(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.MaxPassionOfRelevantSkillsFor))]
	internal class Rimworld__Pawn_SkillTracker__MaxPassionOfRelevantSkillsFor {

		private static FieldInfo Pawn_SkillTrackerPawnInfo = AccessTools.DeclaredField(typeof(Pawn_SkillTracker), "pawn");

		internal static bool Prefix(Pawn_SkillTracker __instance, ref Passion __result, WorkTypeDef workDef) {
			__result = Passion.None;
			if (workDef.relevantSkills.Count == 0) return false;
			var comp = (Pawn_SkillTrackerPawnInfo.GetValue(__instance) as Pawn).GetComp<CompPawnInfo>();
			for (int i = 0; i < workDef.relevantSkills.Count; i++) {
				var skill = __instance.GetSkill(workDef.relevantSkills[i]);
				if (comp.SkillState(skill)) {
					__result = (Passion)Math.Max((int)__result, (int)skill.passion);
				}
			}
			return false;
		}

	}
}
