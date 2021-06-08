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

namespace WhoYouAre.HarmonyWYA {

	[HarmonyPatch(typeof(SituationalThoughtHandler), "TryCreateThought")]
	internal class Rimworld__SituationalThoughtHandler__TryCreateThought {

		internal static void Postfix(ref SituationalThoughtHandler __instance, ref Thought_Situational __result, ThoughtDef def) {
			Rimworld__SituationalThoughtHandler__TryCreateSocialThought.ApplyThoughtTrait(__instance.pawn, __result);
		}

	}

	[HarmonyPatch(typeof(SituationalThoughtHandler), "TryCreateSocialThought")]
	internal class Rimworld__SituationalThoughtHandler__TryCreateSocialThought {

		internal static void Postfix(ref SituationalThoughtHandler __instance, ref Thought_SituationalSocial __result, ThoughtDef def, Pawn otherPawn) {
			ApplyThoughtTrait(__instance.pawn, __result);
		}

		internal static void ApplyThoughtTrait(Pawn pawn, Thought_Situational thought) {
			if (thought == null) return;
			var comp = pawn.PawnInfo();
			var setting = WhoYouAreModSettings.traitSettings;
			foreach (var trait in comp.GetAvaliableTraits(thought: thought)) {
				if (ModUtils.ShouldShow(pawn, comp.TraitState(trait))) ModUtils.MakeDiscoverLetter("Trait", trait.def.defName, pawn, "thought " + thought.def.defName);
				comp.SetTraitState(trait, true);
			}
		}

	}
}
