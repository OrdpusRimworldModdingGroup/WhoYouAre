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

	[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
	internal class Rimworld__CharacterCardUtility__DrawCharacterCard {
		private static MethodInfo FilterPawnTraitInfo = AccessTools.DeclaredMethod(typeof(Rimworld__CharacterCardUtility__DrawCharacterCard), nameof(FilterPawnTrait));
		private static MethodInfo FilterDisableTagsInfo = AccessTools.DeclaredMethod(typeof(Rimworld__CharacterCardUtility__DrawCharacterCard), nameof(FilterDisableTags));
		private static FieldInfo TraitSetAllTraitsInfo = AccessTools.DeclaredField(typeof(TraitSet), nameof(TraitSet.allTraits));
		private static MethodInfo GetCombinedDisabledWorkTagsInfo = AccessTools.DeclaredPropertyGetter(typeof(Pawn), "CombinedDisabledWorkTags");
		private static FieldInfo TraitSetPawnInfo = AccessTools.DeclaredField(typeof(TraitSet), "pawn");
		//private static FieldInfo TraitSetPawnInfo = typeof(TraitSet).GetField("pawn", BindingFlags.NonPublic);

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (var code in instructions) {
				if (code.opcode == OpCodes.Ldfld && code.OperandIs(TraitSetAllTraitsInfo)) {
					yield return new CodeInstruction(OpCodes.Call, FilterPawnTraitInfo);
				} else if (code.opcode == OpCodes.Callvirt && code.OperandIs(GetCombinedDisabledWorkTagsInfo)) {
					yield return new CodeInstruction(OpCodes.Call, FilterDisableTagsInfo);
				} else yield return code;

			}
		}

		internal static void Prefix(Rect rect, Pawn pawn, Action randomizeCallback = null, Rect creationRect = default(Rect)) {
			var rect2 = new Rect();
			rect2.x = rect.width / 4 * 3 + rect.x;
			rect2.y = rect.y + 50;
			rect2.width = Mathf.Min(rect.width / 4, 150);
			rect2.height = 24;
			if (DebugSettings.godMode && Widgets.ButtonText(rect2, "Debug: Unlock All")) {
				var comp = pawn.GetComp<CompPawnInfo>();
				pawn.story.traits.allTraits.ForEach(x => comp.SetTraitState(x, true));
				pawn.skills.skills.ForEach(x => comp.SetSkillState(x, true));
				comp.BackStoryInfo[0] = true;
				comp.BackStoryInfo[1] = true;
			}
		}

		internal static List<Trait> FilterPawnTrait(TraitSet traits) {
			if (ModUtils.StartingOrDebug()) return traits.allTraits;
			var pawn = (Pawn)TraitSetPawnInfo.GetValue(traits);
			return pawn.GetComp<CompPawnInfo>().GetKnownTraits();
		}

		internal static WorkTags FilterDisableTags(Pawn pawn) => pawn.GetComp<CompPawnInfo>().DisabledWorkTags();


	}

	[StaticConstructorOnStartup]
	internal class Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_1__b__21 {

		private static FieldInfo Pawn_StoryTracker_pawnInfo = AccessTools.DeclaredField(typeof(Pawn_StoryTracker), "pawn");

		static Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_1__b__21() {
			Type innerClass = AccessTools.Inner(typeof(CharacterCardUtility), "<>c__DisplayClass14_1");
			MethodInfo anonymousFunc = AccessTools.Method(innerClass, "<DrawCharacterCard>b__21");
			WhoYouAreMod.harmony.Patch(anonymousFunc, transpiler: new HarmonyMethod(typeof(Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_1__b__21), "Transpiler"));
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			var info1 = AccessTools.DeclaredMethod(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.GetBackstory));
			var info2 = AccessTools.DeclaredMethod(typeof(Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_1__b__21), nameof(FilterBackstory));

			foreach (var code in instructions) {
				if (code.opcode == OpCodes.Callvirt && code.OperandIs(info1)) {
					yield return new CodeInstruction(OpCodes.Call, info2);
				} else yield return code;

			}
		}

		internal static Backstory FilterBackstory(Pawn_StoryTracker story, BackstorySlot slot) {
			if (ModUtils.StartingOrDebug()) return story.GetBackstory(slot);
			var storyInfo = ((Pawn)Pawn_StoryTracker_pawnInfo.GetValue(story)).GetComp<CompPawnInfo>().BackStoryInfo;
			if (storyInfo[(int)slot]) return story.GetBackstory(slot);
			return null;
		}

	}

	[StaticConstructorOnStartup]
	internal class Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_9__b__32 {

		static Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_9__b__32() {
			Type innerClass = AccessTools.Inner(typeof(CharacterCardUtility), "<>c__DisplayClass14_9");
			MethodInfo anonymousFunc = AccessTools.Method(innerClass, "<DrawCharacterCard>b__32");
			WhoYouAreMod.harmony.Patch(anonymousFunc, transpiler: new HarmonyMethod(typeof(Rimworld__CharacterCardUtility__DrawCharacterCard), "Transpiler"));
		}

	}
}
