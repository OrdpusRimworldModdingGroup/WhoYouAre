using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using RimWorld;
using Verse;
using HarmonyLib;

namespace WhoYouAre {

	public static class ModUtils {

		public static readonly int[] WholeDay = new int[24].Select((x, i) => i).ToArray();
		public static readonly MethodInfo ModUtilsGetPriorityGiver = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetPriority), new Type[] { typeof(Pawn), typeof(WorkGiverDef), typeof(int) });
		public static readonly MethodInfo ModUtilsGetPriorityType = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetPriority), new Type[] { typeof(Pawn), typeof(WorkTypeDef), typeof(int) });
		public static readonly FieldInfo LogCount = AccessTools.DeclaredField(typeof(Log), "messageCount");
		public static readonly FieldInfo Pawn_WorkSettingsPawn = AccessTools.DeclaredField(typeof(Pawn_WorkSettings), "pawn");
		public static readonly MethodInfo ModUtilsWorkTypeIsDisabled = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.WorkTypeIsDisabled));
		public static readonly MethodInfo PawnWorkTypeIsDisabled = AccessTools.DeclaredMethod(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled));
		public static readonly MethodInfo Pawn_WorkSettingsGetPriority = AccessTools.DeclaredMethod(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.GetPriority));
		public static readonly MethodInfo ModUtilsGetPriority = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetPriority), new Type[] { typeof(Pawn_WorkSettings), typeof(WorkTypeDef) });
		public static readonly MethodInfo Pawn_Extensions__GetPriorityGiver = AccessTools.DeclaredMethod(AccessTools.TypeByName("WorkTab.Pawn_Extensions"), "GetPriority", new Type[] { typeof(Pawn), typeof(WorkGiverDef), typeof(int) });
		public static readonly MethodInfo Pawn_Extensions__GetPriorityType = AccessTools.DeclaredMethod(AccessTools.TypeByName("WorkTab.Pawn_Extensions"), "GetPriority", new Type[] { typeof(Pawn), typeof(WorkTypeDef), typeof(int) });
		public static readonly MethodInfo PawnGetDisabledWorkTypes = AccessTools.DeclaredMethod(typeof(Pawn), nameof(Pawn.GetDisabledWorkTypes));
		public static readonly MethodInfo ModUtilsGetDisabledWorkTypes = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetDisabledWorkTypes));
		public static readonly MethodInfo Pawn_SkillTrackerGetSkill = AccessTools.DeclaredMethod(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.GetSkill));
		public static readonly FieldInfo Pawn_SkillTrackerPawn = AccessTools.DeclaredField(typeof(Pawn_SkillTracker), "pawn");
		public static readonly MethodInfo SkillRecordGetLevel = AccessTools.PropertyGetter(typeof(SkillRecord), nameof(SkillRecord.Level));
		public static readonly FieldInfo SkillRecordPawn = AccessTools.DeclaredField(typeof(SkillRecord), "pawn");
		public static readonly MethodInfo ModUtilsSkillLevel = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.SkillLevel));
		public static readonly MethodInfo SkillNeedValueFor = AccessTools.DeclaredMethod(typeof(SkillNeed), nameof(SkillNeed.ValueFor));
		public static readonly MethodInfo ModUtilsSkillValueFor = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.SkillValueFor));
		public static readonly FieldInfo TraitSetAllTraits = AccessTools.DeclaredField(typeof(TraitSet), nameof(TraitSet.allTraits));
		public static readonly FieldInfo TraitSetPawn = AccessTools.DeclaredField(typeof(TraitSet), "pawn");
		public static readonly MethodInfo ModUtilsAllTraits = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.AllTraits));
		public static readonly MethodInfo SkillRecordTotallyDisabled = AccessTools.PropertyGetter(typeof(SkillRecord), nameof(SkillRecord.TotallyDisabled));
		public static readonly MethodInfo ModUtilsSkillTotallyDisabled = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.SkillTotallyDisabled));
		public static readonly FieldInfo SkillRecordPassion = AccessTools.DeclaredField(typeof(SkillRecord), nameof(SkillRecord.passion));
		public static readonly MethodInfo ModUtilsSkillPassion = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.SkillPassion));
		public static readonly MethodInfo StatWorkerIsDisabledFor = AccessTools.DeclaredMethod(typeof(StatWorker), nameof(StatWorker.IsDisabledFor));
		public static readonly MethodInfo ModUtilsStatIsDisabledFor = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.StatIsDisabledFor));
		public static readonly FieldInfo StatWorkerStat = AccessTools.DeclaredField(typeof(StatWorker), "stat");
		public static readonly MethodInfo StatExtensionGetStatValue = AccessTools.DeclaredMethod(typeof(StatExtension), nameof(StatExtension.GetStatValue));
		public static readonly MethodInfo ModUtilsGetStatValue = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetStatValue));


		public static bool ShouldShow(Pawn pawn, bool pawnUnlocked) {
			bool isPlayer = pawn.Faction?.IsPlayer ?? false;
			return (WhoYouAreModSettings.ShowEnemyLetter || isPlayer) && !pawnUnlocked;
		}

		public static bool StartingOrDebug() {
			return (Current.ProgramState != ProgramState.Playing && !WhoYouAreModSettings.HideStartingPawns) || WhoYouAreModSettings.showPawnInfo;
		}

		public static void MakeDiscoverLetter(string topic, string topicName, Pawn pawn, string reason) {
			Find.LetterStack.ReceiveLetter(topic + " Discovered", "Trait of " + pawn.Name + " has been discovered from " + reason + ", which is " + topicName, LetterDefOf.NeutralEvent, pawn);
		}

		public static bool WorkTypeIsDisabled(Pawn pawn, WorkTypeDef work) {
			if (StartingOrDebug()) return pawn.WorkTypeIsDisabled(work);
			var res = pawn.GetComp<CompPawnInfo>().WorkTypeDisabled(work);
			return res;
		}

		public static int GetPriority(Pawn_WorkSettings instance, WorkTypeDef w) {
			if (StartingOrDebug()) return instance.GetPriority(w);
			var pawn = (Pawn_WorkSettingsPawn.GetValue(instance) as Pawn);
			var res = pawn.GetComp<CompPawnInfo>().WorkState(w);
			return res;
		}

		public static int GetPriority(Pawn pawn, WorkGiverDef work, int hour) {
			if (StartingOrDebug()) return (int)Pawn_Extensions__GetPriorityGiver.Invoke(null, new object[] { pawn, work, hour });
			var res = pawn.GetComp<CompPawnInfo>().WorkState(work, hour);
			return res;
		}

		public static int GetPriority(Pawn pawn, WorkTypeDef work, int hour) {
			if (StartingOrDebug()) return (int)Pawn_Extensions__GetPriorityType.Invoke(null, new object[] { pawn, work, hour });
			var res = pawn.GetComp<CompPawnInfo>().WorkState(work, hour);
			return res;
		}

		public static List<WorkTypeDef> GetDisabledWorkTypes(Pawn pawn, bool permanentOnly = false) {
			if (StartingOrDebug()) return pawn.GetDisabledWorkTypes(permanentOnly);
			var res = pawn.GetComp<CompPawnInfo>().DisabledWorkTypes();
			return res;
		}

		public static int SkillLevel(SkillRecord instance) {
			if (StartingOrDebug()) return instance.Level;
			var res = (SkillRecordPawn.GetValue(instance) as Pawn).GetComp<CompPawnInfo>().SkillState(instance);
			return res ? instance.Level : 0;
		}

		public static float SkillValueFor(SkillNeed instance, Pawn pawn) {
			if (StartingOrDebug()) return (float)SkillNeedValueFor.Invoke(instance, new object[] { pawn });
			var res = pawn.PawnInfo().SkillState(instance.skill) ? (float)SkillNeedValueFor.Invoke(instance, new object[] { pawn }) : 0;
			return res;
		}

		public static List<Trait> AllTraits(TraitSet instance) {
			if (StartingOrDebug()) return instance.allTraits;
			var res = (TraitSetPawn.GetValue(instance) as Pawn).GetComp<CompPawnInfo>().KnownTraits;
			return res;
		}

		public static bool SkillTotallyDisabled(SkillRecord instance) {
			if (StartingOrDebug()) return instance.TotallyDisabled;
			var comp = (SkillRecordPawn.GetValue(instance) as Pawn).GetComp<CompPawnInfo>();
			return instance.def.IsDisabled(comp.DisabledWorkTags(permanentOnly: true), comp.DisabledWorkTypes(permanentOnly: true));
		}

		public static Passion SkillPassion(SkillRecord instance) {
			if (StartingOrDebug()) return instance.passion;
			return (SkillRecordPawn.GetValue(instance) as Pawn).GetComp<CompPawnInfo>().SkillState(instance) ? instance.passion : Passion.None;
		}

		public static bool StatIsDisabledFor(StatWorker instance, Thing thing) {
			if (StartingOrDebug()) return instance.IsDisabledFor(thing);
			var stat = StatWorkerStat.GetValue(instance) as StatDef;
			if (stat.neverDisabled || (stat.skillNeedFactors.NullOrEmpty() && stat.skillNeedOffsets.NullOrEmpty())) {
				return false;
			}
			Pawn pawn = thing as Pawn;
			if (pawn != null && pawn.story != null) {
				if (stat.skillNeedFactors != null) {
					for (int i = 0; i < stat.skillNeedFactors.Count; i++) {
						if (ModUtils.SkillTotallyDisabled(pawn.skills.GetSkill(stat.skillNeedFactors[i].skill))) {
							return true;
						}
					}
				}
				if (stat.skillNeedOffsets != null) {
					for (int j = 0; j < stat.skillNeedOffsets.Count; j++) {
						if (ModUtils.SkillTotallyDisabled(pawn.skills.GetSkill(stat.skillNeedOffsets[j].skill))) {
							return true;
						}
					}
				}
			}
			return false;
		}

		public static float GetStatValue(Thing thing, StatDef stat, bool applyPostProcess = true) {
			if (StartingOrDebug()) return thing.GetStatValue(stat, applyPostProcess);
			return stat.Worker.GetValue(StatRequest.For(thing), false);
		}

		internal static IEnumerable<CodeInstruction> TranspilerType(IEnumerable<CodeInstruction> instructions) {
			foreach (var code in instructions) {
				if (code.Calls(Pawn_Extensions__GetPriorityType)) {
					yield return new CodeInstruction(OpCodes.Call, ModUtilsGetPriorityType);
				} else {
					var ins = GeneralTranspilerGiverTypes(code);
					if (ins != null) yield return ins;
					else yield return code;
				}
			}
		}

		internal static IEnumerable<CodeInstruction> TranspilerGiver(IEnumerable<CodeInstruction> instructions) {
			foreach (var code in instructions) {
				if (code.Calls(Pawn_Extensions__GetPriorityGiver)) {
					yield return new CodeInstruction(OpCodes.Call, ModUtilsGetPriorityGiver);
				} else {
					var ins = GeneralTranspilerGiverTypes(code);
					if (ins != null) yield return ins;
					else yield return code;
				}
			}
		}

		private static CodeInstruction GeneralTranspilerGiverTypes(CodeInstruction code) {
			if (code.Calls(PawnWorkTypeIsDisabled)) {
				return new CodeInstruction(OpCodes.Call, ModUtilsWorkTypeIsDisabled);
			} else if (code.Calls(ModUtilsGetDisabledWorkTypes)) {
				return new CodeInstruction(OpCodes.Call, ModUtilsGetDisabledWorkTypes);
			} else if (code.Calls(Pawn_WorkSettingsGetPriority)) {
				return new CodeInstruction(OpCodes.Call, ModUtilsGetPriority);
			} else return null;
		}

		internal static void PatchAnyBySequence(List<string> classNames, List<string> transpilerNames, List<string> regex) {
			if (classNames.Count != transpilerNames.Count) throw new ArgumentException("class name and transpiler name not equal");
			var regexPattern = regex.Select(x => new Regex(x));
			var classTypes = classNames.Select(AccessTools.TypeByName);
			if (classTypes.All(x => x != null)) {
				var transpilers = transpilerNames.Select(x => {
					var split = x.Split('.');
					return new HarmonyMethod(
						AccessTools.TypeByName(string.Join(".", split.Take(split.Count() - 1))),
						string.Join(".", split.Skip(split.Count() - 1)));
				});
				var methods = classTypes.Select(
					x => AccessTools.GetDeclaredMethods(x)
									.FindAll(x => regexPattern.Any(y => y.IsMatch(x.Name))));
				methods.Zip(transpilers, (x, y) => (x, y))
					   .ToList()
					   .ForEach(x
							=> x.x.ForEach(z
								=> {
									WhoYouAreMod.harmony.Patch(z, transpiler: x.y);
									Log.Message("Patched " + z.DeclaringType.FullName + "." + z.Name);
								}));
			}
		}


		public static List<TResult> Repeat<TResult>(this List<TResult> list, int count) {
			var result = new List<TResult>();
			for (int i = 0; i < count; i++)
				result = result.Concat(list).ToList();
			return result;
		}

		public static CompPawnInfo PawnInfo(this Pawn pawn) => pawn.GetComp<CompPawnInfo>();
	}
}

