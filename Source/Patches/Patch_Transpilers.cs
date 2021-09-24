using HarmonyLib;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using static Perspective.ResourceBank;

namespace Perspective
{
    //[HarmonyDebug]
    [HarmonyPatch(typeof(Graphic), nameof(Graphic.DrawWorker))]
    public static class Patch_DrawWorker
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var count = codes.Count;
            for (int i = 0; i < count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == AccessTools.Method(typeof(Graphic), nameof(Graphic.DrawOffset)))
                {
                    codes.InsertRange(i + 3, new List<CodeInstruction>(){

                        new CodeInstruction(OpCodes.Ldarga_S, 1),
                        new CodeInstruction(OpCodes.Ldarg_S, 4),
                        new CodeInstruction(OpCodes.Call, typeof(Patch_Print).GetMethod(nameof(Patch_Print.DrawPerspectiveOffset)))
                    });
                    transpilerRan++;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }
    
    [HarmonyPatch(typeof(Graphic), nameof(Graphic.Print))]
    public static class Patch_Print
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var count = codes.Count;
            MethodInfo DrawPerspectiveMirror = AccessTools.Method(typeof(Patch_Print), nameof(Patch_Print.DrawPerspectiveMirror));

            for (int i = 0; i < count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == AccessTools.Method(typeof(Graphic), nameof(Graphic.DrawOffset)))
                {
                    codes.InsertRange(i + 3, new List<CodeInstruction>(){

                        new CodeInstruction(OpCodes.Ldloca_S, 3),
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(OpCodes.Call, typeof(Patch_Print).GetMethod(nameof(Patch_Print.DrawPerspectiveOffset))),

                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(OpCodes.Call, DrawPerspectiveMirror),
                        new CodeInstruction(OpCodes.Stloc_0)
                        
                    });
                    transpilerRan++;
                    break;
                }
            }
            if (transpilerRan < 2) Log.Error("[Perspective: Buildings] Transpiler failed to compile. There may be a mod conflict, or RimWorld updated?");
            return codes.AsEnumerable();
        }

        public static void DrawPerspectiveOffset(ref Vector3 pos, Thing thing)
        {
            if (offsetRegistry.TryGetValue(thing?.thingIDNumber ?? 0, out compBuffer))
            {
                pos += compBuffer.currentOffset;
            }
        }

        public static bool DrawPerspectiveMirror(bool flag, Thing thing)
        {
            if (offsetRegistry.TryGetValue(thing?.thingIDNumber ?? 0, out compBuffer))
            {
                if (compBuffer.isMirrored) return true;
            }
            return flag;
        }
    }
}