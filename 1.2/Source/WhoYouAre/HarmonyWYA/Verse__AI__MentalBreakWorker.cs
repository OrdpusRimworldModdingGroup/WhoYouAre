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

	[HarmonyPatch(typeof(MentalBreakWorker), nameof(MentalBreakWorker.TryStart))]
	internal class Verse__AI__MentalBreakWorker__TryStart {

		internal static void Postfix(ref MentalBreakWorker __instance, ref bool __result, Pawn pawn, string reason, bool causedByMood) {
			if (!__result) return;
			var comp = pawn.GetComp<CompPawnInfo>();
			foreach (var trait in comp.GetAvaliableTraits(mentalBreak: __instance.def)) {
				if (ModUtils.ShouldShow(pawn, comp.TraitState(trait))) ModUtils.MakeDiscoverLetter("Trait", trait.def.defName, pawn, "mental break");
				comp.SetTraitState(trait, true);
			}
		}

	}

}
