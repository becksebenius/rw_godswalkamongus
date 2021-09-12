using System.CodeDom.Compiler;
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
    [HarmonyPatch(typeof(IdeoFoundation_Deity), "GenerateDeities")]
    public static class Patch_IdeoFoundation_Deity_GenerateDeities
    {
        public static void Prefix(IdeoFoundation_Deity __instance)
        {
            DeityTracker.Instance.RemoveDeitiesWithIdeo(__instance.ideo);
        }
        
        public static void Postfix(IdeoFoundation_Deity __instance)
        {
            foreach (var deity in __instance.DeitiesListForReading)
            {
                deity.type = DeityInfoGeneration.GenerateTypeString(DeityTracker.Instance.GetOrCreateDeityInfo(__instance.ideo, deity)); 
            }
        }
    }
    
    [HarmonyPatch(typeof(IdeoFoundation_Deity), "FillDeity")]
    public static class Patch_IdeoFoundation_Deity_FillDeity
    {
        public static void Postfix(IdeoFoundation_Deity.Deity deity, IdeoFoundation_Deity __instance)
        {
            var deityInfo = DeityTracker.Instance.GetDeityInfo(__instance.ideo, deity);
            if (deityInfo != null)
            {
                DeityInfoGeneration.Generate(deityInfo);
            }
        }
    }

    [StaticConstructorOnStartup]
    public static class Patch_IdeoFoundation_Deity_DeityBoxSize
    {
        static Patch_IdeoFoundation_Deity_DeityBoxSize()
        {
            var field = typeof(IdeoFoundation_Deity)
                .GetField(
                    "DeityBoxSize",
                    BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null)
            {
                Log.Error("Failed to overwrite DeityBoxSize");
                return;
            }
            field.SetValue(null, new Vector2(220f, 100f));
        }
    }

    [HarmonyPatch(typeof(IdeoFoundation_Deity), nameof(IdeoFoundation_Deity.DoInfo))]
    public static class Patch_IdeoFoundation_Deity_DoInfo
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inList = instructions.ToList();
            int index = inList.Count-1;
            PatchUtility.TrackBack(
                inList, 
                ref index, 
                instr => instr.opcode == OpCodes.Br);
            while (index < inList.Count)
            {
                inList.RemoveAt(index);
            }
            
            inList.Add(new CodeInstruction(OpCodes.Ldarg_0));
            inList.Add(new CodeInstruction(OpCodes.Ldarg_1));
            inList.Add(new CodeInstruction(OpCodes.Ldarg_2));
            inList.Add(new CodeInstruction(OpCodes.Ldarg_3));
            inList.Add(new CodeInstruction(OpCodes.Call, typeof(DeityListDrawer).GetMethod(nameof(DeityListDrawer.DrawDeities))));
            inList.Add(new CodeInstruction(OpCodes.Nop));
            inList.Add(new CodeInstruction(OpCodes.Ret));
            
            return inList;
        }
    }
}