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
	internal class BetterPawnControl__WorkManager__SavePawnPriorities {


		private static FieldInfo Pawn_WorkSettingsPawnInfo = AccessTools.DeclaredField(typeof(Pawn_WorkSettings), "pawn");

		static BetterPawnControl__WorkManager__SavePawnPriorities() {
			var classType = AccessTools.TypeByName("BetterPawnControl.WorkManager");
			if (classType != null) {
				var method = AccessTools.Method(classType, "SavePawnPriorities");
				WhoYouAreMod.harmony.Patch(method, transpiler: new HarmonyMethod(typeof(BetterPawnControl__WorkManager__SavePawnPriorities), nameof(BetterPawnControl__WorkManager__SavePawnPriorities.Transpiler)));
			}
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredMethod(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.GetPriority));
			var info2 = AccessTools.DeclaredMethod(typeof(BetterPawnControl__WorkManager__SavePawnPriorities), nameof(BetterPawnControl__WorkManager__SavePawnPriorities.GetPriority));


			foreach (var code in instructions) {
				if (code.opcode == OpCodes.Callvirt && code.OperandIs(info1)) {
					yield return new CodeInstruction(OpCodes.Call, info2);
				} else yield return code;

			}
		}

		internal static int GetPriority(Pawn_WorkSettings instance, WorkTypeDef workDef) => (Pawn_WorkSettingsPawnInfo.GetValue(instance) as Pawn).GetComp<CompPawnInfo>().WorkState(workDef);

	}
}
