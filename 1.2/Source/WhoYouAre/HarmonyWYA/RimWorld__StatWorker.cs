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
				new List<string> { ".*" }
			);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (var code in instructions) {
				if (code.Calls(ModUtils.SkillRecordGetLevel)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsSkillLevel);
				else yield return code;
			}
		}

	}

	//[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
	//internal class Rimworld__Pawn__SpawnSetup {

	//	internal static void Postfix(ref Pawn __instance, Map map, bool respawningAfterLoad) {
	//		Log.Message(string.Join("\n", __instance.AllComps.Select(x => x.GetType().ToString())));
	//	}

	//}
}
