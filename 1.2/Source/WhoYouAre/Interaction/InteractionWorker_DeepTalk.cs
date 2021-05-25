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

	public class InteractionWorker_DeepTalk_Backstory : RimWorld.InteractionWorker_DeepTalk {


		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient) {
			return 0;
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets) {
			base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
			InteractionWorkerUtil.EvaluateBackstory(initiator, recipient);
			InteractionWorkerUtil.EvaluateSkill(initiator, recipient);
			InteractionWorkerUtil.EvaluateTrait(initiator, recipient);
		}

	}

	public class InteractionWorker_DeepTalk : RimWorld.InteractionWorker_DeepTalk {

		private static Random rand = new Random();

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets) {
			base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
			if (rand.NextDouble() < WhoYouAreModSettings.ChitchatChance) {
				if (rand.NextDouble() < WhoYouAreModSettings.TraitChance) InteractionWorkerUtil.EvaluateTrait(initiator, recipient);
				else InteractionWorkerUtil.EvaluateSkill(initiator, recipient);
			}
			if (rand.NextDouble() < WhoYouAreModSettings.BackStoryChance) InteractionWorkerUtil.EvaluateBackstory(initiator, recipient);
		}

	}

}
