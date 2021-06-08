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

	[StaticConstructorOnStartup]
	internal class RimWorld__StatWorker {


		static RimWorld__StatWorker() {
			var classes = typeof(SkillNeed).AllSubclasses().AddItem(typeof(StatWorker)).Select(x => x.FullName).ToList();
			ModUtils.PatchAnyBySequence(
				classes,
				new List<string> { "WhoYouAre.HarmonyWYA.RimWorld__StatWorker.Transpiler" }.Repeat(classes.Count),
				new List<string> { ".*Explanation.*" }
			);
			ModUtils.PatchAnyBySequence(
				classes,
				new List<string> { "WhoYouAre.HarmonyWYA.RimWorld__StatWorker.TranspilerValue" }.Repeat(classes.Count),
				new List<string> { "GetValueUnfinalized" }
			); 
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (var code in instructions) {
				if (code.Calls(ModUtils.SkillRecordGetLevel)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsSkillLevel);
				else if (code.LoadsField(ModUtils.TraitSetAllTraits)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsAllTraits);
				else if (code.Calls(ModUtils.SkillNeedValueFor)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsSkillValueFor);
				else yield return code;
			}
		}

		internal static IEnumerable<CodeInstruction> TranspilerValue(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredMethod(typeof(RimWorld__StatWorker), nameof(RimWorld__StatWorker.SkillLevel));
			var info2 = AccessTools.DeclaredMethod(typeof(RimWorld__StatWorker), nameof(RimWorld__StatWorker.AllTraits));
			var info3 = AccessTools.DeclaredMethod(typeof(RimWorld__StatWorker), nameof(RimWorld__StatWorker.ValueFor));

			foreach (var code in instructions) {
				if (code.Calls(ModUtils.SkillRecordGetLevel)) {
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Call, info1);
				} else if (code.LoadsField(ModUtils.TraitSetAllTraits)) {
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Call, info2);
				} else if (code.Calls(ModUtils.SkillNeedValueFor)) {
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Call, info3);
				} else yield return code;
			}
		}

		internal static int SkillLevel(SkillRecord skill, bool exp) {
			if (!exp) return ModUtils.SkillLevel(skill);
			return skill.Level;
		}

		internal static List<Trait> AllTraits(TraitSet trait, bool exp) {
			if (!exp) return ModUtils.AllTraits(trait);
			return trait.allTraits;
		}

		internal static float ValueFor(SkillNeed skill, Pawn pawn, bool exp) {
			if (!exp) return ModUtils.SkillValueFor(skill, pawn);
			return skill.ValueFor(pawn);
		}
	}
}
