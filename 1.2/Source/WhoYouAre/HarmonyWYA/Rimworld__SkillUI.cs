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


	[HarmonyPatch(typeof(SkillUI), nameof(SkillUI.DrawSkill), typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string))]
	internal class Rimworld__SkillUI__DrawSkill {

		private static MethodInfo FilterSkillInfo = AccessTools.DeclaredMethod(typeof(Rimworld__SkillUI__DrawSkill), nameof(FilterSkillInfo));

		private static FieldInfo PawnInfo = AccessTools.DeclaredField(typeof(SkillRecord), "pawn");


		internal static void Prefix(ref SkillRecord skill, Rect holdingRect, SkillUI.SkillDrawMode mode, string tooltipPrefix = "") {
			skill = FilterSkill(skill);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredPropertyGetter(typeof(SkillRecord), nameof(SkillRecord.TotallyDisabled));
			var info2 = AccessTools.DeclaredMethod(typeof(Rimworld__SkillUI__DrawSkill), nameof(FilterDisabled));

			foreach (var code in instructions) {
				if (code.opcode == OpCodes.Callvirt && code.OperandIs(info1)) {
					yield return new CodeInstruction(OpCodes.Call, info2);
				} else yield return code;
			}
		}

		private static SkillRecord FilterSkill(SkillRecord skill) {
			if (ModUtils.StartingOrDebug()) return skill;
			var pawn = PawnInfo.GetValue(skill) as Pawn;
			var comp = pawn.PawnInfo();
			if (comp.SkillState(skill) || (skill.def.disablingWorkTags & Rimworld__CharacterCardUtility__DrawCharacterCard.FilterDisableTags(pawn)) != 0) return skill;
			return new SkillRecord(pawn, skill.def);
		}

		private static bool FilterDisabled(SkillRecord skill) {
			if (ModUtils.StartingOrDebug()) return skill.TotallyDisabled;
			return false;
		}

	}
}
