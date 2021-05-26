using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Verse;
using Verse.Grammar;
using RimWorld;
using HarmonyLib;

namespace WhoYouAre {
	[StaticConstructorOnStartup]
	internal static class InteractionWorkerUtil {

		internal static FieldInfo RulePackRulesStringsInfo = AccessTools.DeclaredField(typeof(RulePack), "rulesStrings");
		internal static FieldInfo RulePackDefRulePackInfo = AccessTools.DeclaredField(typeof(RulePackDef), "rulePack");
		private static Random rand = new Random();

		internal static RulePackDef TopicLight {
			get => topicLight;
		}

		internal static Dictionary<string, RulePackDef> RulePackDefsTraits {
			get => rulePackDefsTraits;
		}

		private static Dictionary<string, RulePackDef> rulePackDefsTraits;
		private static RulePackDef topicLight;

		static InteractionWorkerUtil() {
			LongEventHandler.ExecuteWhenFinished(() => {
				rulePackDefsTraits = new Dictionary<string, RulePackDef>();
				foreach (var trait in DefDatabase<TraitDef>.AllDefs) {
					rulePackDefsTraits[trait.defName] = GenerateRulePack("WYA_TopicTrait_" + trait.defName, new List<string>() { "TOPIC->" + trait.defName });
				}
				topicLight = GenerateRulePack("WYA_TopicLight", new List<string>() { "TOPIC->[TalkTopicLight]" });
			});
		}

		private static RulePackDef GenerateRulePack(string defName, List<string> rules) {
			var rule = new RulePackDef();
			rule.defName = defName;
			var rulePack = new RulePack();
			RulePackRulesStringsInfo.SetValue(rulePack, rules);
			RulePackDefRulePackInfo.SetValue(rule, rulePack);
			return rule;
		}

		private static (Pawn, Pawn) ShufflePawn(Pawn pawn1, Pawn pawn2) {
			if (rand.Next(2) == 0) return (pawn1, pawn2);
			return (pawn2, pawn1);
		}

		internal static void EvaluateTrait(Pawn initiator, Pawn recipient) {
			(initiator, recipient) = ShufflePawn(initiator, recipient);
			var comp = initiator.GetComp<CompPawnInfo>();
			var temp = comp.GetAvaliableTraits(relation: initiator.relations.OpinionOf(recipient));
			if (temp.Count == 0) return;
			var topic = temp[rand.Next(temp.Count)];
			if (comp.TraitInfo[topic.def.defName]) return;
			comp.TraitInfo[topic.def.defName] = true;
			Find.LetterStack.ReceiveLetter("Trait Discovered", "Trait of " + initiator.Name + " has been discovered from interactoin, which is " + topic.def.label, LetterDefOf.NeutralEvent, initiator);
		}

		internal static void EvaluateSkill(Pawn initiator, Pawn recipient) {
			(initiator, recipient) = ShufflePawn(initiator, recipient);
			var comp = initiator.GetComp<CompPawnInfo>();
			var temp = comp.GetAvaliableSkills(relation: initiator.relations.OpinionOf(recipient));
			if (temp.Count == 0) return;
			var topic = temp[rand.Next(temp.Count)];
			if (comp.SkillInfo[topic.def.defName]) return;
			comp.SkillInfo[topic.def.defName] = true;
			Find.LetterStack.ReceiveLetter("Skill Evaluated", "Skill of " + initiator.Name + " has been evaluated from interactoin, which is " + topic.def.label, LetterDefOf.NeutralEvent, initiator);
		}

		internal static void EvaluateBackstory(Pawn initiator, Pawn recipient) {
			(initiator, recipient) = ShufflePawn(initiator, recipient);
			var comp = initiator.GetComp<CompPawnInfo>();
			var story = rand.Next(2);
			if (initiator.story.adulthood == null) story = 0;
			if (comp.BackStoryInfo[story]) return;
			var topic = initiator.story.GetBackstory((BackstorySlot)story);
			comp.BackStoryInfo[story] = true;
			Find.LetterStack.ReceiveLetter("Backstory Found", "Backstory of " + initiator.Name + " has been found from interactoin, which is " + topic.title, LetterDefOf.NeutralEvent, initiator);
		}
	}
}
