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
	internal class WorkTab__DrawUtilities__TipForPawnWorker__Giver {

		static WorkTab__DrawUtilities__TipForPawnWorker__Giver() {
			var classType = AccessTools.TypeByName("WorkTab.DrawUtilities");
			if (classType != null) {
				var method = AccessTools.Method(classType, "TipForPawnWorker", new Type[] { typeof(Pawn), typeof(WorkGiverDef), typeof(bool) });
				WhoYouAreMod.harmony.Patch(method, transpiler: new HarmonyMethod(typeof(WorkTab__DrawUtilities__TipForPawnWorker__Giver), nameof(WorkTab__DrawUtilities__TipForPawnWorker__Giver.Transpiler)));
			}
		}


		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredMethod(AccessTools.TypeByName("WorkTab.Pawn_Extensions"), "GetPriority", new Type[] { typeof(Pawn), typeof(WorkGiverDef), typeof(int) });
			var info2 = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetPriority), new Type[] { typeof(Pawn), typeof(WorkGiverDef), typeof(int) });

			foreach (var code in instructions) {
				if (CodeInstructionExtensions.Calls(code, info1)) {
					yield return new CodeInstruction(OpCodes.Call, info2);
				} else yield return code;
			}
		}

	}

	[StaticConstructorOnStartup]
	internal class WorkTab__DrawUtilities__TipForPawnWorker__Type {

		static WorkTab__DrawUtilities__TipForPawnWorker__Type() {
			var classType = AccessTools.TypeByName("WorkTab.DrawUtilities");
			if (classType != null) {
				var method = AccessTools.Method(classType, "TipForPawnWorker", new Type[] { typeof(Pawn), typeof(WorkTypeDef), typeof(bool) });
				WhoYouAreMod.harmony.Patch(method, transpiler: new HarmonyMethod(typeof(WorkTab__DrawUtilities__TipForPawnWorker__Type), nameof(WorkTab__DrawUtilities__TipForPawnWorker__Type.Transpiler)));
			}
		}


		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredMethod(AccessTools.TypeByName("WorkTab.Pawn_Extensions"), "GetPriority", new Type[] { typeof(Pawn), typeof(WorkTypeDef), typeof(int) });
			var info2 = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetPriority), new Type[] { typeof(Pawn), typeof(WorkTypeDef), typeof(int) });

			foreach (var code in instructions) {
				if (CodeInstructionExtensions.Calls(code, info1)) {
					yield return new CodeInstruction(OpCodes.Call, info2);
				} else yield return code;
			}
		}

	}
}
