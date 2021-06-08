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

namespace WhoYouAre.HarmonyWYA.CompatibilityPatch {

	[StaticConstructorOnStartup]
	internal class Numbers__PawnColumnWorker {

		static Numbers__PawnColumnWorker() {
			ModUtils.PatchAnyBySequence(
				new List<string> { "Numbers.PawnColumnWorker_Skill", "Numbers.PawnColumnWorker_Stat"},
				new List<string> { "WhoYouAre.HarmonyWYA.CompatibilityPatch.Numbers__PawnColumnWorker.Transpiler" }.Repeat(2),
				new List<string> { "DoCell", "Compare", "GetTextFor" }
			);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (var code in instructions) {
				if (code.Calls(ModUtils.SkillRecordTotallyDisabled)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsSkillTotallyDisabled);
				else if (code.LoadsField(ModUtils.SkillRecordPassion)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsSkillPassion);
				else if (code.Calls(ModUtils.SkillRecordGetLevel)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsSkillLevel);
				else if (code.Calls(ModUtils.StatWorkerIsDisabledFor)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsStatIsDisabledFor);
				else if (code.Calls(ModUtils.StatExtensionGetStatValue)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsGetStatValue);
				else yield return code;

			}
		}

	}
}
