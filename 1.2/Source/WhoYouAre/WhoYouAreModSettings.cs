using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace WhoYouAre {
	[StaticConstructorOnStartup]
	public class WhoYouAreModSettings : ModSettings {
		public static Dictionary<string, TraitSelection> traitSettings = new Dictionary<string, TraitSelection>();
		public static Dictionary<string, TraitSelection> traitDefaultSettings;
		public static Dictionary<string, SkillSelection> skillSettings = new Dictionary<string, SkillSelection>();
		public static Dictionary<string, SkillSelection> skillDefaultSettings;


		static WhoYouAreModSettings() {
			LongEventHandler.ExecuteWhenFinished(() => {
				SettingExtension.GenerateSetting();
				DefDatabase<TraitDef>.AllDefs.ToList().ForEach(x => {
					if (!traitSettings.ContainsKey(x.defName)) {
						if (traitDefaultSettings.ContainsKey(x.defName)) {
							traitSettings[x.defName] = new TraitSelection(traitDefaultSettings[x.defName]);
						} else {
							traitSettings[x.defName] = new TraitSelection();
						}
					}
				});
			});
		}

		public override void ExposeData() {
			var save = new Dictionary<string, TraitSelection>();
			if (traitDefaultSettings != null) {
				foreach (var entry in save) {
					if (traitDefaultSettings.ContainsKey(entry.Key) && !traitDefaultSettings[entry.Key].Equals(entry.Value))
						save[entry.Key] = entry.Value;
				}
			}
			Scribe_Collections.Look(ref save, "traitSettings");
			foreach (var entry in save)
				traitSettings[entry.Key] = entry.Value;
		}

	}

	public class SkillSelection : IExposable, IEquatable<SkillSelection> {

		public bool ForceShown = false;
		public bool GainFromThought = true;
		public bool GainFromInteraction = true;
		public int RelationThreshold = 50;
		public bool GainByTime = false;
		public int MinimumTimePassed = 50;
		public bool GainFromMaster = true;

		public SkillSelection() { }

		public SkillSelection(SkillSelection copy) {
			ForceShown = copy.ForceShown;
			GainFromThought = copy.GainFromThought;
			GainFromInteraction = copy.GainFromInteraction;
			RelationThreshold = copy.RelationThreshold;
			GainByTime = copy.GainByTime;
			MinimumTimePassed = copy.MinimumTimePassed;
			GainFromMaster = copy.GainFromMaster;
		}

		public bool Equals(SkillSelection other) {
			if (this == other) return true;
			return ForceShown == other.ForceShown &&
				   GainFromThought == other.GainFromThought &&
				   GainFromInteraction == other.GainFromInteraction &&
				   RelationThreshold == other.RelationThreshold &&
				   GainByTime == other.GainByTime &&
				   MinimumTimePassed == other.MinimumTimePassed &&
				   GainFromMaster == other.GainFromMaster;
		}

		public void ExposeData() {
			Scribe_Values.Look(ref ForceShown, "ForceShown");
			Scribe_Values.Look(ref GainFromThought, "GainFromThought");
			Scribe_Values.Look(ref GainFromInteraction, "GainFromInteraction");
			Scribe_Values.Look(ref RelationThreshold, "RelationThreshold");
			Scribe_Values.Look(ref GainByTime, "GainByTime");
			Scribe_Values.Look(ref MinimumTimePassed, "MinimumTimePassed");
			Scribe_Values.Look(ref GainFromMaster, "GainFromMaster");
		}

	}

	public class TraitSelection : IExposable, IEquatable<TraitSelection> {
		public bool ForceShown = false;
		public bool GainFromThought = true;
		public bool GainFromInteraction = true;
		public int RelationThreshold = 50;
		public bool GainByTime = false;
		public int MinimumTimePassed = 50;
		public bool GainFromBreak = true;

		public TraitSelection() { }

		public TraitSelection(TraitSelection copy) {
			ForceShown = copy.ForceShown;
			GainFromThought = copy.GainFromThought;
			GainFromInteraction = copy.GainFromInteraction;
			RelationThreshold = copy.RelationThreshold;
			GainByTime = copy.GainByTime;
			MinimumTimePassed = copy.MinimumTimePassed;
			GainFromBreak = copy.GainFromBreak;
		}

		public bool Equals(TraitSelection other) {
			if (this == other) return true;
			return ForceShown == other.ForceShown &&
				   GainFromThought == other.GainFromThought &&
				   GainFromInteraction == other.GainFromInteraction &&
				   RelationThreshold == other.RelationThreshold &&
				   GainByTime == other.GainByTime &&
				   MinimumTimePassed == other.MinimumTimePassed &&
				   GainFromBreak == other.GainFromBreak;
		}

		public void ExposeData() {
			Scribe_Values.Look(ref ForceShown, "ForceShown");
			Scribe_Values.Look(ref GainFromThought, "GainFromThought");
			Scribe_Values.Look(ref GainFromInteraction, "GainFromInteraction");
			Scribe_Values.Look(ref RelationThreshold, "RelationThreshold");
			Scribe_Values.Look(ref GainByTime, "GainByTime");
			Scribe_Values.Look(ref MinimumTimePassed, "MinimumTimePassed");
			Scribe_Values.Look(ref GainFromBreak, "GainFromBreak");
		}

	}
}
