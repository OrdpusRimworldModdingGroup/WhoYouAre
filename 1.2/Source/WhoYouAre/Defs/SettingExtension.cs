using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace WhoYouAre {
	[StaticConstructorOnStartup]
	public class SettingExtension : DefModExtension {

		private List<Entry_Value<string, TraitSelection>> traitData;
		private List<Entry_Value<string, SkillSelection>> skillData;
		private List<Entry_Value<string, ThingSetMaker_Sum.Option>> thingSetMakerOption;


		public static void GenerateSetting() {
			var ext = DefDatabase<ThingDef>.GetNamed("WYA_ModSetting").GetModExtension<SettingExtension>();
			WhoYouAreModSettings.traitDefaultSettings = DictScribeUtil.ListToDict(ext.traitData);
			WhoYouAreModSettings.skillDefaultSettings = DictScribeUtil.ListToDict(ext.skillData);
			WhoYouAreModSettings.thingSetMakerOptions = DictScribeUtil.ListToDict(ext.thingSetMakerOption);
			WhoYouAreModSettings.thingSetMakerOptions["ExtraMechSerumResurrector"].chance = WhoYouAreModSettings.MechSerumResurrectorChance;
		}

	}
}
