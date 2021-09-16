using Verse;
using RimWorld;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using static Perspective.Offsetter.Override;
 
namespace Perspective
{
    [HarmonyPatch (typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PostResolve))]
    static class Patch_DefGenerator
    {
        static void Postfix()
        {
            //Give standard offsets to the following defs:
            var dd = DefDatabase<ThingDef>.AllDefs.Where(x => (x.HasModExtension<Offsetter>() && x.GetModExtension<Offsetter>().ignore == False) || 
            x.category == ThingCategory.Building && 
            x.graphicData !=null && !x.graphicData.Linked && x.graphicData.linkFlags == LinkFlags.None && x.useHitPoints && 
            (x.GetCompProperties<CompProperties_Power>() == null || x.GetCompProperties<CompProperties_Power>().basePowerConsumption > 0)).ToList();
            foreach (var def in dd)
            {
                //Has a pre-defined offsetter?
                if (def.HasModExtension<Offsetter>())
                {
                    var modX = def.GetModExtension<Offsetter>();
                    if (modX.offsets == null && modX.mirror == Normal && modX.ignore == Normal)
                    {
                        def.modExtensions.Remove(modX);
                        continue;
                    }
                    else
                    {
                        if (modX.offsets == null) modX.offsets = Mod_Perspective.standardOffsets;
                    }
                }
                else
                {
                    //Add modX list if missing
                    if (def.modExtensions == null) def.modExtensions = new List<DefModExtension>();
                    def.modExtensions.Add(new Offsetter(){offsets = Mod_Perspective.standardOffsets});
                }

                //Add component
                if (def.comps == null) def.comps = new List<CompProperties>();
                def.comps.Add(new CompProperties(){compClass = typeof(CompOffsetter)});
            }
        }
    }
}