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
		public static Dictionary<string, ThingSetMaker_Sum.Option> thingSetMakerOptions;
		public static bool HideStartingPawns, ShowEnemyLetter, showPawnInfo = false;
		public static double ChitchatChance = 0.01, TraitChance = 0.2, BackStoryChance = 0.2;
		//public const double ChitchatChance = 1, TraitChance = 1, BackStoryChance = 1;
		public static float MechSerumResurrectorChance = 0.25f;

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
				DefDatabase<SkillDef>.AllDefs.ToList().ForEach(x => {
					if (!skillSettings.ContainsKey(x.defName)) {
						if (skillDefaultSettings.ContainsKey(x.defName)) {
							skillSettings[x.defName] = new SkillSelection(skillDefaultSettings[x.defName]);
						} else {
							skillSettings[x.defName] = new SkillSelection();
						}
					}
				});
			});
		}

		public override void ExposeData() {
			if (traitSettings == null) traitSettings = new Dictionary<string, TraitSelection>();
			Scribe_Collections.Look(ref traitSettings, "traitSettings");
			if (traitDefaultSettings != null) {
				foreach (var entry in traitSettings) {
					if (traitDefaultSettings.ContainsKey(entry.Key) && traitDefaultSettings[entry.Key].Equals(entry.Value))
						traitSettings.Remove(entry.Key);
				}
			}
			if (skillSettings == null) skillSettings = new Dictionary<string, SkillSelection>();
			Scribe_Collections.Look(ref skillSettings, "skillSettings");
			if (skillDefaultSettings != null) {
				foreach (var entry in skillSettings) {
					if (skillDefaultSettings.ContainsKey(entry.Key) && skillDefaultSettings[entry.Key].Equals(entry.Value))
						skillSettings.Remove(entry.Key);
				}
			}
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

		public static string[] LabelsCheckbox = new string[] {
			"Force Shown",
			"Gain From Thought",
			"Gain From Interaction",
			"Gain By Time",
			"Gain From Master"
		};

		public static string[] LabelsSlider = new string[] {
			"Relation Threshold",
			"Minimum Time Passed"
		};

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

		public (bool[], int[], int[]) ToArray() {
			return (new bool[] { ForceShown, GainFromThought, GainFromInteraction, GainByTime, GainFromMaster }, new int[] { RelationThreshold, MinimumTimePassed }, new int[] { 2, 3 });
		}

		public void FromArray(bool[] bools, int[] ints) {
			(ForceShown, GainFromThought, GainFromInteraction, GainByTime, GainFromMaster) = (bools[0], bools[1], bools[2], bools[3], bools[4]);
			(RelationThreshold, MinimumTimePassed) = (ints[0], ints[1]);
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

		public static string[] LabelsCheckbox = new string[] {
			"Force Shown",
			"Gain From Thought",
			"Gain From Interaction",
			"Gain By Time",
			"Gain From Break"
		};

		public static string[] LabelsSlider = new string[] {
			"Relation Threshold",
			"Minimum Time Passed"
		};

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

		public (bool[], int[], int[]) ToArray() {
			return (new bool[] { ForceShown, GainFromThought, GainFromInteraction, GainByTime, GainFromBreak }, new int[] { RelationThreshold, MinimumTimePassed }, new int[] { 2, 3 });
		}

		public void FromArray(bool[] bools, int[] ints) {
			(ForceShown, GainFromThought, GainFromInteraction, GainByTime, GainFromBreak) = (bools[0], bools[1], bools[2], bools[3], bools[4]);
			(RelationThreshold, MinimumTimePassed) = (ints[0], ints[1]);
		}

	}
}
