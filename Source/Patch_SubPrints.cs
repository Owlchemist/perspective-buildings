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
			var comp = ___pawn.CurrentBed()?.GetComp<CompOffsetter>();

			if (comp != null && comp.currentOffset != Mod_Perspective.zero) __result += comp.currentOffset;
            return __result;
        }
    }
}