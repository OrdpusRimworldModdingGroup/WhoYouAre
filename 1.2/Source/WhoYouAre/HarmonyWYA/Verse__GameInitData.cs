using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using Verse;
using HarmonyLib;

namespace WhoYouAre.HarmonyWYA {

	[StaticConstructorOnStartup]
	[HarmonyPatch(typeof(GameInitData), nameof(GameInitData.PrepForMapGen))]
	internal class Verse__GameInitData__PrepForMapGen {

		static Verse__GameInitData__PrepForMapGen() {
			var transpiler = AccessTools.DeclaredMethod(typeof(Verse__GameInitData__PrepForMapGen), nameof(Verse__GameInitData__PrepForMapGen.Transpiler));
			var inner = AccessTools.Inner(typeof(GameInitData), "<>c__DisplayClass16_0");
			new List<MethodInfo> {
				AccessTools.DeclaredMethod(inner, "<PrepForMapGen>b__0"),
				AccessTools.DeclaredMethod(inner, "<PrepForMapGen>b__1")
			}.ForEach(x => WhoYouAreMod.harmony.Patch(x, transpiler: new HarmonyMethod(transpiler)));
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredMethod(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled));
			var info2 = AccessTools.DeclaredMethod(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.AverageOfRelevantSkillsFor));
			foreach (var code in instructions) {
				if (code.Calls(info1)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsWorkTypeIsDisabled);
				else if (code.opcode == OpCodes.Ldc_R4 && code.OperandIs(6)) yield return new CodeInstruction(OpCodes.Ldc_R4, -1f);
				else yield return code;
			}
		}

	}

}
