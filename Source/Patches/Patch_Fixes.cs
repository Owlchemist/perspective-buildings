using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;
using static Perspective.ResourceBank;

namespace Perspective
{
    //Pawns in bed follow position
    [HarmonyPatch(typeof(PawnRenderer), "GetBodyPos")]
	public class Patch_SubPrints
	{
        static Vector3 Postfix(Vector3 __result, Pawn ___pawn)
		{
            var bed = ___pawn.CurrentBed();
            if (bed != null && offsetRegistry.TryGetValue(bed.thingIDNumber, out compBuffer) && compBuffer.isOffset) __result += compBuffer.currentOffset;
            return __result;
        }
    }

    //Dynamic shadows (non-static)
    [HarmonyPatch(typeof(Graphic_Shadow), "Print")]
	public class Patch_ShadowPrint
	{
        static bool Prefix(SectionLayer layer, Thing thing, ShadowData ___shadowInfo, float ___GlobalShadowPosOffsetX, float ___GlobalShadowPosOffsetZ)
        {
            if (offsetRegistry.TryGetValue(thing.thingIDNumber, out compBuffer))
            {    
                Vector3 center = compBuffer.cachedTrueCenter + (___shadowInfo.offset + new Vector3(___GlobalShadowPosOffsetX, 0f, ___GlobalShadowPosOffsetZ)).RotatedBy(thing.Rotation);
                center.y = AltitudeLayer.Shadows.AltitudeFor();
                Printer_Shadow.PrintShadow(layer, center, ___shadowInfo, thing.Rotation);
                return false;
            }
            return true;
        }
    }

    //Pawns in bed follow position
    [HarmonyPatch(typeof(Graphic_Flicker), "DrawWorker")]
	public class Patch_Graphic_Flicker_DrawWorker
	{
        static void Prefix(ref Vector3 loc, Thing thing)
		{
            if (offsetRegistry.TryGetValue(thing?.thingIDNumber ?? 0, out compBuffer))
            {
                loc += compBuffer.currentOffset;
            }
        }
    }
}