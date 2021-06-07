using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RimWorld;
using Verse;
using HarmonyLib;

namespace WhoYouAre {
	public static class RefInfo {

		private static bool workTab = WhoYouAreMod.WorkTab;

		public static MethodInfo WorkTab__WorkType_Extensions = AccessTools.DeclaredMethod(AccessTools.TypeByName("WorkTab.WorkType_Extensions"), "WorkGivers");

		public static List<WorkGiverDef> WorkGivers(this WorkTypeDef worktype) {
			if (workTab) return WorkTab__WorkType_Extensions.Invoke(null, new object[] { worktype }) as List<WorkGiverDef>;
			return worktype.workGiversByPriority;
		}

	}
}
