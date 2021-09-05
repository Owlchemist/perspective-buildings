using Verse;
using RimWorld;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
 
namespace Perspective
{
    [HarmonyPatch (typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PostResolve))]
    static class Patch_DefGenerator
    {
        static void Postfix()
        {
            var dd = DefDatabase<ThingDef>.AllDefs.Where(x => x.category == ThingCategory.Building && x.graphicData !=null && !x.graphicData.Linked && x.graphicData.linkFlags == LinkFlags.None && x.useHitPoints).ToList();
            foreach (var thingDef in dd)
            {
                //Add comp list if missing
                if (thingDef.comps == null) thingDef.comps = new List<CompProperties>();

                //Add offsetter if it's not there. If it is there, check if it should be removed via ignore override.
                if (thingDef.GetCompProperties<CompProperties_Offsetter>() == null) thingDef.comps.Add(new CompProperties_Offsetter());
            }
        }
    }
}