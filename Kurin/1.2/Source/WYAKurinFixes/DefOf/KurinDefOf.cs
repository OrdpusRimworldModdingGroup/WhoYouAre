using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Garam_RaceAddon;
using Verse;
using RimWorld;

namespace WYAKurinFixes {
	[DefOf]
	public static class KurinDefOf {
		public static RaceAddonThingDef Kurin;

		public static FactionDef Kurin_Faction;


		static KurinDefOf() {
			DefOfHelper.EnsureInitializedInCtor(typeof(KurinDefOf));
		}
	}
}
