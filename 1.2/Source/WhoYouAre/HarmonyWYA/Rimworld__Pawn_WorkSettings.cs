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
using Verse.AI;
using WhoYouAre;

namespace WhoYouAre.HarmonyWYA {

	[HarmonyPatch(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.SetPriority))]
	internal class Rimworld__Pawn_WorkSettings__SetPriority {

		private static FieldInfo Pawn_WorkSettingsPawnInfo = AccessTools.DeclaredField(typeof(Pawn_WorkSettings), "pawn");

		internal static void Prefix(Pawn_WorkSettings __instance, WorkTypeDef w, int priority) {
			var pawn = Pawn_WorkSettingsPawnInfo.GetValue(__instance) as Pawn;
			pawn.GetComp<CompPawnInfo>().SetWorkState(w, priority);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			bool first = true;
			foreach (var code in instructions) {
				if (first && code.opcode == OpCodes.Ldc_I4_4) {
					first = false;
					yield return new CodeInstruction(OpCodes.Ret);
				}
				yield return code;
			}
		}

	}

	[HarmonyPatch(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.EnableAndInitialize))]
	internal class Rimworld__Pawn_WorkSettings__EnableAndInitialize {

		private static FieldInfo Pawn_WorkSettingsPawnInfo = AccessTools.DeclaredField(typeof(Pawn_WorkSettings), "pawn");

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredMethod(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled));
			var info2 = AccessTools.DeclaredMethod(
				typeof(Rimworld__Pawn_WorkSettings__EnableAndInitialize),
				nameof(Rimworld__Pawn_WorkSettings__EnableAndInitialize.WorkTypeIsDisabled));
			var info3 = AccessTools.DeclaredMethod(typeof(Pawn), nameof(Pawn.GetDisabledWorkTypes));
			var info4 = AccessTools.DeclaredMethod(
				typeof(Rimworld__Pawn_WorkSettings__EnableAndInitialize),
				nameof(Rimworld__Pawn_WorkSettings__EnableAndInitialize.DisableWorkTypes));

			var list = instructions.ToList();
			for (int i = 0; i < list.Count; i++) {
				var code = list[i];
				if (code.opcode == OpCodes.Callvirt && code.OperandIs(info1)) {
					yield return new CodeInstruction(OpCodes.Call, info2);
				} else if (code.opcode == OpCodes.Callvirt && code.OperandIs(info3)) {
					yield return new CodeInstruction(OpCodes.Call, info4);
				} else if (code.opcode == OpCodes.Ldc_I4_6 && list[i - 1].opcode == OpCodes.Ldloc_0) {
					yield return new CodeInstruction(OpCodes.Ldc_I4_S, 127);
				} else yield return code;
			}
		}

		internal static List<WorkTypeDef> DisableWorkTypes(Pawn pawn, bool aBool = false) => pawn.GetComp<CompPawnInfo>().DisabledWorkTypes();


		internal static bool WorkTypeIsDisabled(Pawn instance, WorkTypeDef workDef) => instance.GetComp<CompPawnInfo>().WorkTypeDisabled(workDef);


	}

	[HarmonyPatch(typeof(Pawn_WorkSettings), "<EnableAndInitialize>b__18_0")]
	internal class Rimworld__Pawn_WorkSettings__EnableAndInitialize__b__18_0 {

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => Rimworld__Pawn_WorkSettings__EnableAndInitialize.Transpiler(instructions);

	}

}
