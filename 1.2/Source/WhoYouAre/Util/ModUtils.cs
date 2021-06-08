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

	[StaticConstructorOnStartup]
	public static class ModUtils {

		public static int[] WholeDay = new int[24].Select((x, i) => i).ToArray();
		public static MethodInfo ModUtilsGetPriorityGiver = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetPriority), new Type[] { typeof(Pawn), typeof(WorkGiverDef), typeof(int) });
		public static MethodInfo ModUtilsGetPriorityType = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetPriority), new Type[] { typeof(Pawn), typeof(WorkTypeDef), typeof(int) });
		public static FieldInfo LogCount = AccessTools.DeclaredField(typeof(Log), "messageCount");
		public static FieldInfo Pawn_WorkSettingsPawn = AccessTools.DeclaredField(typeof(Pawn_WorkSettings), "pawn");
		public static MethodInfo ModUtilsWorkTypeIsDisabled = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.WorkTypeIsDisabled));
		public static MethodInfo PawnWorkTypeIsDisabled = AccessTools.DeclaredMethod(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled));
		public static MethodInfo Pawn_WorkSettingsGetPriority = AccessTools.DeclaredMethod(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.GetPriority));
		public static MethodInfo ModUtilsGetPriority = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetPriority), new Type[] { typeof(Pawn_WorkSettings), typeof(WorkTypeDef) });
		public static MethodInfo Pawn_Extensions__GetPriorityGiver = AccessTools.DeclaredMethod(AccessTools.TypeByName("WorkTab.Pawn_Extensions"), "GetPriority", new Type[] { typeof(Pawn), typeof(WorkGiverDef), typeof(int) });
		public static MethodInfo Pawn_Extensions__GetPriorityType = AccessTools.DeclaredMethod(AccessTools.TypeByName("WorkTab.Pawn_Extensions"), "GetPriority", new Type[] { typeof(Pawn), typeof(WorkTypeDef), typeof(int) });
		public static MethodInfo PawnGetDisabledWorkTypes = AccessTools.DeclaredMethod(typeof(Pawn), nameof(Pawn.GetDisabledWorkTypes));
		public static MethodInfo ModUtilsGetDisabledWorkTypes = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.GetDisabledWorkTypes));
		public static MethodInfo Pawn_SkillTrackerGetSkill = AccessTools.DeclaredMethod(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.GetSkill));
		public static FieldInfo Pawn_SkillTrackerPawn = AccessTools.DeclaredField(typeof(Pawn_SkillTracker), "pawn");
		public static MethodInfo SkillRecordGetLevel = AccessTools.PropertyGetter(typeof(SkillRecord), nameof(SkillRecord.Level));
		public static FieldInfo SkillRecordPawn = AccessTools.DeclaredField(typeof(SkillRecord), "pawn");
		public static MethodInfo ModUtilsSkillLevel = AccessTools.DeclaredMethod(typeof(ModUtils), nameof(ModUtils.SkillLevel));

		static ModUtils() {
		}

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
	}
}

