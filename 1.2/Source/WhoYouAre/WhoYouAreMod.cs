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


		public static Harmony Harmony { get => harmony; }

		static WhoYouAreMod() {
			harmony = new Harmony("Ordpus.WhoYouAre");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		public WhoYouAreMod(ModContentPack content) : base(content) {
			settings = GetSettings<WhoYouAreModSettings>();
		}

		public override void DoSettingsWindowContents(Rect canvas) {
			float lineHeight = Text.LineHeight;
			Listing_Standard listingStandard = new Listing_Standard();
			Rect rect2 = new Rect(0, 0, canvas.width - 20, (3f + lineHeight) * WhoYouAreModSettings.traitSettings.Count);
			Widgets.BeginScrollView(canvas, ref scrollLoc, rect2);
			float rowY = 0;
			foreach (var k in WhoYouAreModSettings.traitSettings.Keys) {
				DrawSingleSetting(new Rect(0, rowY, rect2.width, lineHeight), k);
				rowY += 3f + lineHeight;
			}
			Widgets.EndScrollView();

		}

		public override string SettingsCategory() {
			return "WhoYouAre.Setting".Translate();
		}

		public void DrawSingleSetting(Rect rect, string defName) {
			const float frontGap = 100;
			const float iconGap = 50;
			var row = new WidgetRow(rect.x + frontGap, rect.y);
			row.Label(defName, rect.width / 4);
			var traitInfo = WhoYouAreModSettings.traitSettings[defName];
			bool ForceShown = traitInfo.ForceShown, GainFromThought = traitInfo.GainFromThought, GainFromInteraction = traitInfo.GainFromInteraction, GainByTime = traitInfo.GainByTime, GainFromBreak = traitInfo.GainFromBreak;
			row.ToggleableIcon(ref ForceShown, null, "Force Shown");
			row.Gap(iconGap);
			row.ToggleableIcon(ref GainFromThought, null, "Gain From Thought");
			row.Gap(iconGap);
			row.ToggleableIcon(ref GainFromInteraction, null, "Gain From Interaction");
			row.Gap(iconGap);
			row.ToggleableIcon(ref GainByTime, null, "Gain By Time");
			row.Gap(iconGap);
			row.ToggleableIcon(ref GainFromBreak, null, "Gain From Break");
			row.Gap(iconGap);
			traitInfo.ForceShown = ForceShown;
			traitInfo.GainFromThought = traitInfo.GainFromThought;
			traitInfo.GainFromInteraction = traitInfo.GainFromInteraction;
			traitInfo.GainByTime = traitInfo.GainByTime;
			traitInfo.GainFromBreak = traitInfo.GainFromBreak;
		}
	}
}
