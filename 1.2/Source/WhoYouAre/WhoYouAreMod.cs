using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace WhoYouAre {

	[StaticConstructorOnStartup]
	public class WhoYouAreMod : Mod {

		internal static Harmony harmony;
		private static Vector2 scrollLoc = Vector2.zero;

		public static WhoYouAreModSettings settings;

		internal static bool WorkTab = ModLister.HasActiveModWithName("Work Tab");


		public static Harmony Harmony { get => harmony; }

		static WhoYouAreMod() {
			harmony = new Harmony("Ordpus.WhoYouAre");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		public WhoYouAreMod(ModContentPack content) : base(content) {
			settings = GetSettings<WhoYouAreModSettings>();
			var list = HarmonyLib.Harmony.GetAllPatchedMethods();
			Log.Message(string.Join("\n", list.Select(x => x.ReflectedType.FullName + '.' + x.Name + '\n' + GetPatchesInfo(HarmonyLib.Harmony.GetPatchInfo(x)))));
			Log.Message("Has worktab " + WorkTab);
		}

		private static string GetPatchesInfo(Patches patch) {
			var tmp = string.Join("\n\t\t", patch.Prefixes.Select(GetPatchInfo));
			var result = "\tPrefix :: " + (string.IsNullOrEmpty(tmp) ? "" : "\n\t\t" + tmp) + '\n';
			tmp = string.Join("\n\t\t", patch.Postfixes.Select(GetPatchInfo));
			result += "\tPostfix :: " + (string.IsNullOrEmpty(tmp) ? "" : "\n\t\t" + tmp) + '\n';
			tmp = string.Join("\n\t\t", patch.Transpilers.Select(GetPatchInfo));
			result += "\tTranspiler ::" + (string.IsNullOrEmpty(tmp) ? "" : "\n\t\t" + tmp);
			return result;
		}

		private static string GetPatchInfo(Patch patch) => patch.owner + " " + patch.priority + " " + patch.PatchMethod.Module.Assembly.GetName().Name;

		public override void DoSettingsWindowContents(Rect canvas) {
			float lineHeight = Text.LineHeight;
			Listing_Standard listingStandard = new Listing_Standard();
			var showedSliders = WhoYouAreModSettings.traitSettings.Aggregate(0, (tot, x) => x.Value.GainFromInteraction || x.Value.GainByTime ? 1 : 0);
			Rect rect2 = new Rect(0, 0, canvas.width - 20, (3f + lineHeight) * (WhoYouAreModSettings.traitSettings.Count + showedSliders));
			Widgets.BeginScrollView(canvas, ref scrollLoc, rect2);
			float rowY = 0;
			foreach (var entry in WhoYouAreModSettings.traitSettings) {
				bool[] bools;
				int[] ints, pos;
				(bools, ints, pos) = entry.Value.ToArray();
				DrawSingleSetting(new Rect(0, rowY, rect2.width, lineHeight), entry.Key, TraitSelection.LabelsCheckbox, bools, TraitSelection.LabelsSlider, ints, pos);
				entry.Value.FromArray(bools, ints);
				var temp = false;
				var param = 1;
				foreach (var item in pos) temp |= bools[item];
				if (temp) param += 1;
				rowY += (3f + lineHeight) * param;
			}
			Widgets.EndScrollView();

		}

		public override string SettingsCategory() {
			return "WhoYouAre.Setting".Translate();
		}

		public void DrawSingleSetting(Rect rect, string defName, string[] labels, bool[] bools, string[] labels2, int[] ints, int[] pos) {
			const float frontGap = 100;
			const float labelWidth = 100;
			const float iconGap = 20;
			const float boxWidth = 100;
			const float boxGap = 50;
			var row = new WidgetRow(rect.x + frontGap, rect.y);
			row.Label(defName, labelWidth);
			bool temp;
			int posi = 0;
			for (int i = 0; i < labels.Length; i++) {
				temp = bools[i];
				row.ToggleableIcon(ref temp, null, labels[i]);
				bools[i] = temp;
				row.Gap(iconGap);
				if (posi < pos.Length && pos[posi] == i) {
					if (temp) {
						var rect2 = new Rect(row.FinalX, row.FinalY, boxWidth, Text.LineHeight);
						var temp2 = ints[posi];
						string buffer = temp2.ToString();
						Widgets.TextFieldNumeric(rect2, ref temp2, ref buffer);
						TooltipHandler.TipRegion(rect2, labels2[posi]);
						ints[posi] = temp2;
						row.Gap(rect2.width + iconGap);
					}
					posi++;
				}
			}
		}
	}
}
