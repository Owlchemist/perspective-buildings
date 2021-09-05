using HarmonyLib;
using Verse;
using UnityEngine;

namespace Perspective
{
    [HarmonyPatch(typeof(Thing), "DrawAt")]
	public class Patch_DrawAt
	{
        static void Prefix(ref Vector3 drawLoc, ref Thing __instance, bool flip = false)
		{
            if (__instance.TryGetComp<CompOffsetter>()?.currentOffset != null ) drawLoc += __instance.TryGetComp<CompOffsetter>().currentOffset;
        }
    }
}