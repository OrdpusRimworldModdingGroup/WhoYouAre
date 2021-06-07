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


	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SetFaction))]
	internal class Rimworld__Pawn__SetFaction {

		internal static void Postfix(ref Pawn __instance, Faction newFaction, Pawn recruiter = null) {
			if (newFaction.IsPlayer) {
				__instance.GetComp<CompPawnInfo>().dayJoined = GenDate.DaysPassed;
			} else __instance.GetComp<CompPawnInfo>().dayJoined = int.MinValue;
		}

	}

	//[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
	//internal class Rimworld__Pawn__SpawnSetup {

	//	internal static void Postfix(ref Pawn __instance, Map map, bool respawningAfterLoad) {
	//		Log.Message(string.Join("\n", __instance.AllComps.Select(x => x.GetType().ToString())));
	//	}

	//}
}
