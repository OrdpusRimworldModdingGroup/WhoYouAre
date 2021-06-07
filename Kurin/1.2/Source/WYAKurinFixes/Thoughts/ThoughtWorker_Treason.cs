using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace WYAKurinFixes {
	public class ThoughtWorker_Treason : ThoughtWorker {

		private static List<Faction> factions;

		protected override ThoughtState CurrentStateInternal(Pawn p) {
			if (p.def != KurinDefOf.Kurin) {
				return ThoughtState.Inactive;
			}
			if (p.Faction == null) return ThoughtState.Inactive;
			if (factions == null) factions = Find.FactionManager.AllFactions.ToList().FindAll(x => x.def == KurinDefOf.Kurin_Faction);
			if (factions.Find(x => FactionUtility.HostileTo(x, p.Faction)) != null) {
				return ThoughtState.ActiveAtStage(0);
			}
			return ThoughtState.Inactive;
		}
	}
}
