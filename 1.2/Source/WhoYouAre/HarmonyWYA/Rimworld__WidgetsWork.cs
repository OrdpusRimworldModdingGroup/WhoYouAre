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

	[HarmonyPatch(typeof(WidgetsWork), nameof(WidgetsWork.DrawWorkBoxFor))]
	internal class Rimworld__WidgetsWork__DrawWorkBoxFor {

		private static FieldInfo Pawn_WorkSettingsPawnInfo = AccessTools.DeclaredField(typeof(Pawn_WorkSettings), "pawn");

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredMethod(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled));
			var info2 = AccessTools.DeclaredMethod(typeof(Rimworld__WidgetsWork__DrawWorkBoxFor), nameof(Rimworld__WidgetsWork__DrawWorkBoxFor.FilterDisabled));
			var info3 = AccessTools.DeclaredMethod(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.GetPriority));
			var info4 = AccessTools.DeclaredMethod(typeof(Rimworld__WidgetsWork__DrawWorkBoxFor), nameof(Rimworld__WidgetsWork__DrawWorkBoxFor.GetPriority));

			foreach (var code in instructions) {
				if (code.opcode == OpCodes.Callvirt && code.OperandIs(info1)) {
					yield return new CodeInstruction(OpCodes.Call, info2);
				} else if (code.opcode == OpCodes.Callvirt && code.OperandIs(info3)) {
					yield return new CodeInstruction(OpCodes.Call, info4);
				} else yield return code;
			}
		}

		internal static bool FilterDisabled(Pawn pawn, WorkTypeDef workDef) {
			var list = pawn.GetDisabledWorkTypes();
			if (ModUtils.StartingOrDebug()) return list.Contains(workDef);
			return pawn.GetComp<CompPawnInfo>().WorkTypeDisabled(workDef);
		}

		internal static int GetPriority(Pawn_WorkSettings instance, WorkTypeDef workDef) {
			var pawn = Pawn_WorkSettingsPawnInfo.GetValue(instance) as Pawn;
			var comp = pawn.GetComp<CompPawnInfo>();
			return comp.WorkState(workDef);
		}
	}

	[HarmonyPatch(typeof(WidgetsWork), nameof(WidgetsWork.TipForPawnWorker))]
	internal class Rimworld__WidgetsWork__TipForPawnWorker {

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => Rimworld__WidgetsWork__DrawWorkBoxFor.Transpiler(instructions);
	}

}


