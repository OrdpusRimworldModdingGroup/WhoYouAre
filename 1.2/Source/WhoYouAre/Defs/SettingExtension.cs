using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WhoYouAre {
	[StaticConstructorOnStartup]
	public class SettingExtension : DefModExtension {

		private List<Entry_Value<string, TraitSelection>> traitData;
		private List<Entry_Value<string, SkillSelection>> skillData;


		public static void GenerateSetting() {
			WhoYouAreModSettings.traitDefaultSettings =
					DictScribeUtil.ListToDict(
						DefDatabase<ThingDef>.GetNamed("WYA_ModSetting").GetModExtension<SettingExtension>().traitData);
			WhoYouAreModSettings.skillDefaultSettings =
					DictScribeUtil.ListToDict(
						DefDatabase<ThingDef>.GetNamed("WYA_ModSetting").GetModExtension<SettingExtension>().skillData);
		}

	}
}
