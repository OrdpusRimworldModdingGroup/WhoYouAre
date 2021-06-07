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
	internal class WorkTab__WorkerAny {

		internal static List<string> contains = new List<string> { "HandleInteraction" };
		internal static List<string> exact = new List<string> { "ShouldDrawCell", "DrawWorkGiverBoxFor" };

		static WorkTab__WorkerAny() {
			//var classTypes = new List<Type> {
			//	AccessTools.TypeByName("WorkTab.PawnColumnWorker_WorkType"),
			//	AccessTools.TypeByName("WorkTab.PawnColumnWorker_WorkGiver")
			//};
			//if (classTypes.All(x => x != null)) {
			//	var transpilers = new List<string> { "TranspilerType", "TranspilerGiver" }
			//		.Select(x => new HarmonyMethod(typeof(ModUtils), x));
			//	var methods = classTypes.Select(
			//		x => AccessTools.GetDeclaredMethods(x)
			//						.FindAll(x =>
			//							contains.Any(y => x.Name.Contains(y)) ||
			//							exact.Any(y => x.Name == y)
			//						)).ToList();
			//	methods.Zip(transpilers, (x, y) => (x, y))
			//		   .ToList()
			//		   .ForEach(x
			//				=> x.x.ForEach(z
			//					=> WhoYouAreMod.harmony.Patch(z, transpiler: x.y)));
			//}

			ModUtils.PatchAnyBySequence(
				new List<string> { "WorkTab.PawnColumnWorker_WorkType", "WorkTab.PawnColumnWorker_WorkGiver" },
				new List<string> { "ModUtils.TranspilerType", "ModUtils.TranspilerGiver" },
				new List<string> { ".*Interaction.*", "ShouldDrawCell", "DrawWork.*BoxFor" }
			);
			ModUtils.PatchAnyBySequence(
				new List<string> {
					"WorkTab.WorkType_Extensions", "WorkTab.WorkGiver_Extensions" ,
					"WorkTab.WorkType_Extensions.<>c__DisplayClass3_0", "WorkTab.WorkGiver_Extensions.<>c__DisplayClass1_0",
					"WorkTab.WorkType_Extensions.<>c__DisplayClass5_0","WorkTab.WorkGiver_Extensions.<>c__DisplayClass3_0"},
				new List<string> { "ModUtils.TranspilerType", "ModUtils.TranspilerGiver" }.Repeat(3),
				new List<string> { "Priority.*" }
			);
		}
	}

}