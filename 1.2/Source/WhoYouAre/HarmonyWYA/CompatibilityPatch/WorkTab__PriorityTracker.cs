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
	internal class WorkTab__PriorityTracker__SetPriority {

		static FieldInfo PawnInfo = AccessTools.DeclaredField(AccessTools.TypeByName("WorkTab.PawnPriorityTracker"), "pawn");

		static WorkTab__PriorityTracker__SetPriority() {
			var classType = AccessTools.TypeByName("WorkTab.PriorityTracker");
			if (classType != null) {
				var method = AccessTools.Method(classType, "SetPriority", new Type[] { typeof(WorkGiverDef), typeof(int), typeof(int), typeof(bool) });
				WhoYouAreMod.harmony.Patch(method, prefix: new HarmonyMethod(typeof(WorkTab__PriorityTracker__SetPriority), nameof(WorkTab__PriorityTracker__SetPriority.Prefix)));
			}
		}


		internal static void Prefix(object __instance, WorkGiverDef workgiver, int priority, int hour, bool recache = true) {
			var pawn = PawnInfo.GetValue(__instance) as Pawn;
			pawn.PawnInfo().SetWorkState(workgiver, priority, hour);
		}

	}
}
