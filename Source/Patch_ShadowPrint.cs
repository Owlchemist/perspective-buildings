using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;

namespace Perspective
{
    [HarmonyPatch(typeof(Graphic_Shadow), "Print")]
	public class Patch_ShadowPrint
	{
        static bool Prefix(ref Graphic_Shadow __instance, SectionLayer layer, Thing thing, ShadowData ___shadowInfo, float ___GlobalShadowPosOffsetX, float ___GlobalShadowPosOffsetZ)
        {
            var comp = thing.TryGetComp<CompOffsetter>();
            if (comp != null && comp.currentOffset != Mod_Perspective.zero)
            {
                Vector3 center = thing.TrueCenter() + (___shadowInfo.offset + new Vector3(___GlobalShadowPosOffsetX, 0f, ___GlobalShadowPosOffsetZ)).RotatedBy(thing.Rotation) + comp.currentOffset;
			    center.y = AltitudeLayer.Shadows.AltitudeFor();
			    Printer_Shadow.PrintShadow(layer, center, ___shadowInfo, thing.Rotation);
                return false;
            }
            return true;
        }
    }
}