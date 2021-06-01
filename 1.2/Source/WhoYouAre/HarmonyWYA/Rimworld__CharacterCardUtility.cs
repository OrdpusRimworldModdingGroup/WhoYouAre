﻿#define DEBUGDisableTags

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
				foreach (var k in pawn.story.traits.allTraits)
					comp.SetTraitState(k, true);
				foreach (var k in pawn.skills.skills)
					comp.SetSkillState(k, true);
				comp.BackStoryInfo[0] = true;
				comp.BackStoryInfo[1] = true;
			}
		}

		internal static List<Trait> FilterTraits(Pawn pawn) {
			var comp = pawn.GetComp<CompPawnInfo>();
			return pawn.story.traits.allTraits.FindAll(x => comp.TraitState(x));
		}

		internal static List<Trait> FilterPawnTrait(TraitSet traits) {
			if (ModUtils.StartingOrDebug()) return traits.allTraits;
			var pawn = (Pawn)TraitSetPawnInfo.GetValue(traits);
			var list = FilterTraits(pawn);
			return list;
		}

		internal static WorkTags FilterDisableTags(Pawn pawn) {
#if DEBUGDisableTags
			int debug_index = 0;
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			if (ModUtils.StartingOrDebug()) return pawn.CombinedDisabledWorkTags;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			var story = pawn.story;
			Log.Message("" + (story == null) + (pawn.GetComp<CompPawnInfo>() == null));
			var storyInfo = pawn.GetComp<CompPawnInfo>().BackStoryInfo;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			var traits = FilterTraits(pawn);
			var result = WorkTags.None;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			if (storyInfo[0]) result |= story?.childhood?.workDisables ?? WorkTags.None;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			if (storyInfo[1]) result |= story?.adulthood?.workDisables ?? WorkTags.None;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			foreach (var trait in traits)
				result |= trait.def.disabledWorkTags;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			pawn?.royalty?.AllTitlesForReading.ForEach(x => result |= x.conceited ? x.def.disabledWorkTags : WorkTags.None);
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			pawn?.health?.hediffSet?.hediffs.ForEach(x => result |= x?.CurStage?.disabledWorkTags ?? WorkTags.None);
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			foreach (QuestPart_WorkDisabled q in QuestUtility.GetWorkDisabledQuestPart(pawn))
				result |= q.disabledWorkTags;
#if DEBUGDisableTags
			Log.Message("FilterDisableTags " + debug_index++);
#endif
			return result;
		}
	}

	[StaticConstructorOnStartup]
	internal class Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_1__b__21 {

		private static MethodInfo FilterBackstoryInfo = AccessTools.DeclaredMethod(typeof(Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_1__b__21), nameof(FilterBackstory));
		private static MethodInfo GetBackstoryInfo = AccessTools.DeclaredMethod(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.GetBackstory));
		private static FieldInfo Pawn_StoryTracker_pawnInfo = AccessTools.DeclaredField(typeof(Pawn_StoryTracker), "pawn");

		static Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_1__b__21() {
			Type innerClass = AccessTools.Inner(typeof(CharacterCardUtility), "<>c__DisplayClass14_1");
			MethodInfo anonymousFunc = AccessTools.Method(innerClass, "<DrawCharacterCard>b__21");
			WhoYouAreMod.harmony.Patch(anonymousFunc, transpiler: new HarmonyMethod(typeof(Rimworld__CharacterCardUtility__DrawCharacterCard__c__DisplayClass14_1__b__21), "Transpiler"));
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (var code in instructions) {
				if (code.opcode == OpCodes.Callvirt && code.OperandIs(GetBackstoryInfo)) {
					yield return new CodeInstruction(OpCodes.Call, FilterBackstoryInfo);
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
