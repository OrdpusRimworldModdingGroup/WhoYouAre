using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using HarmonyLib;

namespace WhoYouAre.HarmonyWYA {


	[HarmonyPatch(typeof(Dialog_DebugSettingsMenu), "DoListingItems")]
	internal class Verse__Dialog_DebugSettingsMenu {

		private static MethodInfo DoField_NewTmpInfo = AccessTools.DeclaredMethod(typeof(Dialog_DebugSettingsMenu), "DoField_NewTmp");
		private static FieldInfo ModSettingShowPawnInfo = AccessTools.DeclaredField(typeof(WhoYouAreModSettings), nameof(WhoYouAreModSettings.showPawnInfo));
		private static FieldInfo ModSettingShowEnemyLetter = AccessTools.DeclaredField(typeof(WhoYouAreModSettings), nameof(WhoYouAreModSettings.ShowEnemyLetter));


		internal static void Postfix(ref Dialog_DebugSettingsMenu __instance) {
			DoField_NewTmpInfo.Invoke(__instance, new object[] { ModSettingShowPawnInfo, false });
			DoField_NewTmpInfo.Invoke(__instance, new object[] { ModSettingShowEnemyLetter, false });
		}
	}
}
