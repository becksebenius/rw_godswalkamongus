using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace GodsWalkAmongUs.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        public static Harmony harmonyInstance;

        static HarmonyInit()
        {
            harmonyInstance = new Harmony("GWAU.Mod");
            harmonyInstance.PatchAll();
        }
    }
    
    [HarmonyPatch(typeof(IdeoManager), nameof(IdeoManager.Remove))]
    public class IdeoManager_Remove
    {
        public static void Postfix(Ideo ideo, bool __result)
        {
            if (__result)
            {
                DeityTracker.Instance.OnIdeoRemoved(ideo);
            }
        }
    }
    
    [HarmonyPatch(typeof(IdeoManager), nameof(IdeoManager.Add))]
    public class IdeoManager_Add
    {
        public static void Postfix(Ideo ideo, bool __result)
        {
            if (__result)
            {
                DeityTracker.Instance.OnIdeoAdded(ideo);
            }
        }
    }

    [HarmonyPatch(typeof(Dialog_EditDeity), "ApplyChanges")]
    public class Dialog_EditDeity_ApplyChanges
    {
        public static bool Prefix()
        {
            DeityDialogExtension.ApplyChanges();
            return true;
        }
    }

    [HarmonyPatch(typeof(Dialog_EditDeity), nameof(Dialog_EditDeity.InitialSize), MethodType.Getter)]
    public class Dialog_EditDeity_InitialSize
    {
        public static bool Prefix(ref Vector2 __result)
        {
            __result = new Vector2(700f, 500f);
            return false;
        }
    }
    
    [HarmonyPatch(typeof(Dialog_EditDeity), nameof(Dialog_EditDeity.DoWindowContents))]
    public class Dialog_EditDeity_DoWindowContents
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Log.Message("Transpiling!");
            var inList = instructions.ToList();
            int? injectionIndex = null;
            for(int i = 0; i < inList.Count; ++i)
            {
                var instr = inList[i];
                if (!(instr.opcode == OpCodes.Ldstr
                    && instr.operand is string stringValue
                    && stringValue == "Back"))
                {
                    continue;
                }
                
                // Found the "Back" button
                // Now look for the previous expression
                for (int j = i; 0 <= j; --j)
                {
                    instr = inList[j];
                    if (instr.opcode == OpCodes.Stloc_2)
                    {
                        injectionIndex = j;
                        break;
                    }
                }

                break;
            }

            if (!injectionIndex.HasValue)
            {
                Log.Error("Failed to find injection point for EditDeity dialog");
                return inList;
            }

            var index = injectionIndex.Value;
            
            var newInstructions = new List<CodeInstruction>();
            newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_1));
            newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
            newInstructions.Add(new CodeInstruction(OpCodes.Ldfld, typeof(Dialog_EditDeity).GetField("ideo", BindingFlags.Instance | BindingFlags.NonPublic)));
            newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
            newInstructions.Add(new CodeInstruction(OpCodes.Ldfld, typeof(Dialog_EditDeity).GetField("deity", BindingFlags.Instance | BindingFlags.NonPublic)));
            newInstructions.Add(new CodeInstruction(OpCodes.Call, 
                typeof(DeityDialogExtension)
                    .GetMethod(
                        nameof(DeityDialogExtension.Draw), 
                        BindingFlags.Static | BindingFlags.Public)));
            newInstructions.Add(new CodeInstruction(OpCodes.Nop));
            newInstructions.Reverse();
            foreach (var instr in newInstructions)
            {
                inList.Insert(index, instr);
            }

            return inList;
        }
    }
}