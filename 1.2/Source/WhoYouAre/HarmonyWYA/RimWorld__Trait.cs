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
	[HarmonyPatch(typeof(Trait), nameof(Trait.MultiplierOfStat))]
	internal class RimWorld__Trait__MultiplierOfStat {

		internal static bool Prefix(Trait __instance, ref float __result, StatDef stat) {
			if (__instance.pawn == null) return true;
			var comp = __instance.pawn.GetComp<CompPawnInfo>();
			if (comp.TraitState(__instance)) return true;
			__result = 1f;
			return false;
		}

	}

	[HarmonyPatch(typeof(Trait), nameof(Trait.OffsetOfStat))]
	internal class RimWorld__Trait__OffsetOfStat {

		internal static bool Prefix(Trait __instance, ref float __result, StatDef stat) {
			if (__instance.pawn == null) return true;
			var comp = __instance.pawn.GetComp<CompPawnInfo>();
			if (comp.TraitState(__instance)) return true;
			__result = 0f;
			return false;
		}

	}
}
