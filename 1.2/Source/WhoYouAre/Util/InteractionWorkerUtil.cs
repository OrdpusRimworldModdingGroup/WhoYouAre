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


		internal static void GenerateExtraRuleChitchat(string topic, List<RulePackDef> extraSentencePacks) {
			if (topic == "") {
				extraSentencePacks.Add(TopicLight);
				return;
			}
			var rule = RulePackDefsTraits[topic];
			Log.Message("has topic on " + topic);
			rule.RulesImmediate.ForEach(x => Log.Message(x.keyword + ';' + x.tag + ';' + x.requiredTag));
			extraSentencePacks.Add(rule);
		}

		private static (Pawn, Pawn) ShufflePawn(Pawn pawn1, Pawn pawn2) {
			if (rand.Next(2) == 0) return (pawn1, pawn2);
			return (pawn2, pawn1);
		}

		internal static void EvaluateTrait(Pawn initiator, Pawn recipient, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets) {
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
			(initiator, recipient) = ShufflePawn(initiator, recipient);
			var comp = initiator.GetComp<CompPawnInfo>();
			var temp = comp.GetAvaliableTraits(relation: initiator.relations.OpinionOf(recipient));
			//Log.Message("Trying to Gain Trait");
			//temp.ForEach(x => Log.Message(x.def.defName));
			if (temp.Count == 0) {
				//InteractionWorkerUtil.GenerateExtraRuleChitchat("", extraSentencePacks);
				return;
			}
			var topic = temp[rand.Next(temp.Count)];
			if (comp.TraitInfo[topic.def.defName]) return;
			comp.TraitInfo[topic.def.defName] = true;
			letterText = "Trait of " + initiator.Name + " has been discovered, which is " + topic.def.label;
			letterLabel = "Trait Discovered";
			letterDef = LetterDefOf.NeutralEvent;
			lookTargets = initiator;
		}

		internal static void EvaluateSkill(Pawn initiator, Pawn recipient, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets) {
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
			(initiator, recipient) = ShufflePawn(initiator, recipient);
			var comp = initiator.GetComp<CompPawnInfo>();
			var temp = comp.GetAvaliableSkills(relation: initiator.relations.OpinionOf(recipient));
			//Log.Message("Trying to Gain Trait");
			//temp.ForEach(x => Log.Message(x.def.defName));
			if (temp.Count == 0) {
				//InteractionWorkerUtil.GenerateExtraRuleChitchat("", extraSentencePacks);
				return;
			}
			var topic = temp[rand.Next(temp.Count)];
			if (comp.SkillInfo[topic.def.defName]) return;
			comp.SkillInfo[topic.def.defName] = true;
			letterText = "Skill of " + initiator.Name + " has been evaluated, which is " + topic.def.label;
			letterLabel = "Skill Evaluated";
			letterDef = LetterDefOf.NeutralEvent;
			lookTargets = initiator;
		}

		internal static void EvaluateBackstory(Pawn initiator, Pawn recipient, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets) {
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
			(initiator, recipient) = ShufflePawn(initiator, recipient);
			var comp = initiator.GetComp<CompPawnInfo>();
			var story = rand.Next(2);
			if (comp.BackStoryInfo[story]) return;
			var topic = initiator.story.GetBackstory((BackstorySlot)story);
			letterText = "Backstory of " + initiator.Name + " has been poured out, which is " + topic.title;
			letterLabel = "Skill Evaluated";
			letterDef = LetterDefOf.NeutralEvent;
			lookTargets = initiator;
		}
	}
}
