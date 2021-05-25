using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Grammar;
using HarmonyLib;

namespace WhoYouAre {

	public class InteractionWorker_Chitchat_Trait : InteractionWorker_Chitchat {

		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient) {
			return base.RandomSelectionWeight(initiator, recipient) * 0.1f;
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets) {
			base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
			InteractionWorkerUtil.EvaluateTrait(initiator, recipient, out letterText, out letterLabel, out letterDef, out lookTargets);
		}

	}

	public class InteractionWorker_Chitchat_Skill : InteractionWorker_Chitchat {

		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient) {
			return base.RandomSelectionWeight(initiator, recipient) * 0.1f;
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets) {
			base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
			InteractionWorkerUtil.EvaluateSkill(initiator, recipient, out letterText, out letterLabel, out letterDef, out lookTargets);
		}

	}
}
