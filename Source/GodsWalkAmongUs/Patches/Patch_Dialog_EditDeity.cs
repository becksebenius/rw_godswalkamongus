using System;
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

    [HarmonyPatch(typeof(Dialog_EditDeity), nameof(RimWorld.Dialog_EditDeity.DoWindowContents))]
    public class Dialog_EditDeity_DoWindowContents
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inList = instructions.ToList();

            InjectDeityDialogExtensionDraw(inList);
            RemoveTypeEditControl(inList);
            
            return inList;
        }

        static void RemoveTypeEditControl(List<CodeInstruction> instructions)
        {
            var index = instructions.FindIndex(
                instr =>
                    instr.opcode == OpCodes.Ldstr
                    && instr.operand is string stringOperand
                    && stringOperand == "DeityTitle");
            if (index == -1)
            {
                Log.Error("Failed to find injection point for EditDeity dialog");
                return;
            }

            int startIndex = index;
            PatchUtility.TrackBack(instructions, ref startIndex, instr => instr.opcode == OpCodes.Stloc_2);
            ++startIndex;

            int endIndex = index;
            PatchUtility.TrackForward(instructions, ref endIndex, instr => 
                instr.opcode == OpCodes.Stfld
                && instr.OperandIs(typeof(Dialog_EditDeity).GetField("newDeityTitle", BindingFlags.Instance | BindingFlags.NonPublic)));
            
            Log.Message("Removing " + (endIndex - startIndex) + " instructions");

            int totalToRemove = endIndex - startIndex + 1;
            for (int i = 0; i < totalToRemove; ++i)
            {
                instructions.RemoveAt(startIndex);
            }
        }

        static void InjectDeityDialogExtensionDraw(List<CodeInstruction> instructions)
        {
            int index = instructions.FindIndex(
                instr =>
                    instr.opcode == OpCodes.Ldstr
                    && instr.operand is string stringValue
                    && stringValue == "Back");
            if (index == -1)
            {
                Log.Error("Failed to find injection point for EditDeity dialog");
                return;
            }
            
            PatchUtility.TrackBack(instructions, ref index, instr => instr.opcode == OpCodes.Stloc_2);

            var newInstructions = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld,
                    typeof(Dialog_EditDeity).GetField("ideo", BindingFlags.Instance | BindingFlags.NonPublic)),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld,
                    typeof(Dialog_EditDeity).GetField("deity", BindingFlags.Instance | BindingFlags.NonPublic)),
                new CodeInstruction(OpCodes.Call,
                    typeof(DeityDialogExtension)
                        .GetMethod(
                            nameof(DeityDialogExtension.Draw),
                            BindingFlags.Static | BindingFlags.Public)),
                new CodeInstruction(OpCodes.Nop)
            };
            newInstructions.Reverse();
            foreach (var instr in newInstructions)
            {
                instructions.Insert(index, instr);
            }
        }
    }
}