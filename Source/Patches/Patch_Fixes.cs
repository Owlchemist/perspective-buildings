using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;
using static Perspective.ResourceBank;

namespace Perspective
{
    //Pawns in bed follow position
    [HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.GetBodyPos))]
	public class Patch_SubPrints
	{
        static Vector3 Postfix(Vector3 __result, Pawn ___pawn)
		{
            var bed = ___pawn.CurrentBed();
            return (bed != null && offsetRegistry.TryGetValue(bed.thingIDNumber, out CompOffsetter compBuffer) && compBuffer.isOffset) ? __result += compBuffer.currentOffset : __result;
        }
    }

    //Dynamic shadows (non-static). This should probably be replaced with a transpiler one of these days...
    [HarmonyPatch(typeof(Graphic_Shadow), nameof(Graphic_Shadow.Print))]
	public class Patch_ShadowPrint
	{
        static bool Prefix(Thing thing, ShadowData ___shadowInfo, SectionLayer layer, float ___GlobalShadowPosOffsetX, float ___GlobalShadowPosOffsetZ)
        {
            if (offsetRegistry.TryGetValue(thing.thingIDNumber, out CompOffsetter compBuffer) && compBuffer.isOffset)
            {    
                Vector3 center = compBuffer.cachedTrueCenter + compBuffer.currentOffset + (___shadowInfo.offset + (new Vector3(___GlobalShadowPosOffsetX, 0f, ___GlobalShadowPosOffsetZ)).RotatedBy(thing.Rotation));
                center.y = AltitudeLayer.Shadows.AltitudeFor();
                Printer_Shadow.PrintShadow(layer, center, ___shadowInfo, thing.Rotation);
                return false;
            }
            return true;
        }
    }

    //Fire graphics follow offset
    [HarmonyPatch(typeof(Graphic_Flicker), nameof(Graphic_Flicker.DrawWorker))]
	public class Patch_Graphic_Flicker_DrawWorker
	{
        static void Prefix(ref Vector3 loc, Thing thing)
		{
            if (offsetRegistry.TryGetValue(thing?.thingIDNumber ?? 0, out CompOffsetter compBuffer)) loc += compBuffer.currentOffset;
        }
    }

    //Refresh cache on game load
    [HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
	public class Patch_LoadGame
	{
        static void Prefix()
		{
            offsetRegistry.Clear();
        }
    }
}