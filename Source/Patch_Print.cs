using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;
using System;

namespace Perspective
{
    //Mirroring (todo: consider changing this to a transpiler so it's less invasive?)
    [HarmonyPatch(typeof(Graphic), "Print")]
	public class Patch_Print
	{
        static bool Prefix(ref Thing thing, ref Graphic __instance, SectionLayer layer)
		{
            CompOffsetter comp;
            if (Mod_Perspective.offsetRegistry.TryGetValue(thing.thingIDNumber, out comp) && comp != null && comp.mirrored)
            {
                Vector3 center = thing.TrueCenter() + __instance.DrawOffset(thing.Rotation);
                Material mat = __instance.MatAt(thing.Rotation, thing);
                Vector2[] uvs;
                Color32 color;
                Graphic.TryGetTextureAtlasReplacementInfo(mat, thing.def.category.ToAtlasGroup(), true, true, out mat, out uvs, out color);
                Printer_Plane.PrintPlane(layer, center, __instance.drawSize, mat, 0, true, uvs, new Color32[]
                {
                    color,
                    color,
                    color,
                    color
                }, 0.01f, 0f);

                //Add shadow
                if (__instance.ShadowGraphic != null && thing != null) __instance.ShadowGraphic.Print(layer, thing, 0f);

                return false;
            }
            return true;
        }
    }

    //Non-realtime graphics offsetter
    [HarmonyPatch(typeof(GenThing), "TrueCenter", new Type[] { typeof(Thing)})]
	public class Patch_TrueCenter
	{
        static Vector3 Postfix(Vector3 __result, ref Thing t)
		{
            CompOffsetter comp;
            if (Mod_Perspective.offsetRegistry.TryGetValue(t.thingIDNumber, out comp) && comp != null && comp.isOffset) __result += comp.currentOffset;
            return __result;
        }
    }

    //Dynamic shadows (non-static)
    [HarmonyPatch(typeof(Graphic_Shadow), "Print")]
	public class Patch_ShadowPrint
	{
        static bool Prefix(SectionLayer layer, Thing thing, ShadowData ___shadowInfo, float ___GlobalShadowPosOffsetX, float ___GlobalShadowPosOffsetZ)
        {
            CompOffsetter comp;
            if (Mod_Perspective.offsetRegistry.TryGetValue(thing.thingIDNumber, out comp) && comp != null && comp.isOffset)
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