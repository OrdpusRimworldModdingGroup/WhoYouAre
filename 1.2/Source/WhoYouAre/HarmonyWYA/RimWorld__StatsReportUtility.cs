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

	[StaticConstructorOnStartup]
	internal class RimWorld__StatsReportUtility {

		internal static MethodInfo StatExtensionGetStatValue = AccessTools.DeclaredMethod(typeof(StatExtension), nameof(StatExtension.GetStatValue));
		internal static MethodInfo RimWorld__StatsReportUtilityGetStatValue = AccessTools.DeclaredMethod(typeof(RimWorld__StatsReportUtility), nameof(RimWorld__StatsReportUtility.GetStatValue));


		static RimWorld__StatsReportUtility() {
			var classes = typeof(StatsReportUtility).GetNestedTypes(AccessTools.all).Select(x => x.FullName).ToList().FindAll(x => x.Contains("StatsToDraw"));
			ModUtils.PatchAnyBySequence(
				classes,
				new List<string> { "WhoYouAre.HarmonyWYA.RimWorld__StatsReportUtility.Transpiler" }.Repeat(classes.Count),
				new List<string> { "MoveNext", "StatsToDraw" }
			);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (var code in instructions) {
				if (code.Calls(ModUtils.SkillRecordGetLevel)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsSkillLevel);
				else if (code.LoadsField(ModUtils.TraitSetAllTraits)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsAllTraits);
				else if (code.Calls(ModUtils.SkillNeedValueFor)) yield return new CodeInstruction(OpCodes.Call, ModUtils.ModUtilsSkillValueFor);
				else if (code.Calls(StatExtensionGetStatValue)) yield return new CodeInstruction(OpCodes.Call, RimWorld__StatsReportUtilityGetStatValue);
				else yield return code;
			}
		}

		internal static float GetStatValue(Thing thing, StatDef stat, bool applyPostProcess = true) {
			var req = StatRequest.For(thing);
			var instance = stat.Worker;
			if (stat.minifiedThingInherits) {
				MinifiedThing minifiedThing = req.Thing as MinifiedThing;
				if (minifiedThing != null) {
					if (minifiedThing.InnerThing != null) {
						return minifiedThing.InnerThing.GetStatValue(stat, applyPostProcess);
					}
					Log.Error("MinifiedThing's inner thing is null.");
				}
			}
			float val = instance.GetValueUnfinalized(req, false);
			instance.FinalizeValue(req, ref val, applyPostProcess);
			return val;
		}

	}
}
