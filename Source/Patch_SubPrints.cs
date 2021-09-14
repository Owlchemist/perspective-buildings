using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;

namespace Perspective
{
    //Pawns in bed follow position
    [HarmonyPatch(typeof(PawnRenderer), "GetBodyPos")]
	public class Patch_SubPrints
	{
        static Vector3 Postfix(Vector3 __result, Pawn ___pawn)
		{
            CompOffsetter comp;
            var bed = ___pawn.CurrentBed();
            if (bed != null && Mod_Perspective.offsetRegistry.TryGetValue(bed.GetHashCode(), out comp) && comp != null)
            {
                if (comp != null && comp.currentOffset != Mod_Perspective.zero) __result += comp.currentOffset;
            }
            return __result;
        }
    }
}