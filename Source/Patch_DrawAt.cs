using HarmonyLib;
using Verse;
using UnityEngine;

namespace Perspective
{
    [HarmonyPatch(typeof(Thing), "DrawAt")]
	public class Patch_DrawAt
	{
        static Vector3 zero = new Vector3(0,0,0);
        static void Prefix(ref Vector3 drawLoc, ref Thing __instance, bool flip = false)
		{
            if (__instance.TryGetComp<CompOffsetter>() != null && __instance.TryGetComp<CompOffsetter>().currentOffset != zero) drawLoc += __instance.TryGetComp<CompOffsetter>().currentOffset;
        }
    }
}