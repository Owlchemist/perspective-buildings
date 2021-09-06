using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;

namespace Perspective
{
    [HarmonyPatch(typeof(Graphic), "Print")]
	public class Patch_PostDraw
	{
        static Vector3 zero = new Vector3(0,0,0);
        //This could probably be replaced by a transpiler if you want to get fancy. Current method is a copy-modification. Ugly, but it'll do for now. Maybe revisit when we got a better handle things.
        static bool Prefix(ref Thing thing, ref Graphic __instance, SectionLayer layer, float extraRotation)
		{
            var comp = thing.TryGetComp<CompOffsetter>();
            //This only applies to things with offset components, and the component must be used (not zero), and the drawtype must be mesh.
            if (comp != null && (comp.currentOffset != zero || comp.mirrored) && (thing.def.drawerType == DrawerType.MapMeshOnly || thing.def.drawerType == DrawerType.MapMeshAndRealTime))
            {
                Vector2 size;
                bool drawRotated;
                if (__instance.ShouldDrawRotated)
                {
                    size = __instance.drawSize;
                    drawRotated = false;
                }
                else
                {
                    if (!thing.Rotation.IsHorizontal) size = __instance.drawSize;
                    else size = __instance.drawSize.Rotated();

                    drawRotated = (thing.Rotation == Rot4.West && __instance.WestFlipped) || (thing.Rotation == Rot4.East && __instance.EastFlipped);
                }
                //Check mirrored bool
                if (comp.mirrored) drawRotated = true;

                float num = AngleFromRot(__instance, thing.Rotation) + extraRotation;

                if (drawRotated && __instance.data != null) num += __instance.data.flipExtraRotation;

                Vector3 center = thing.TrueCenter() + __instance.DrawOffset(thing.Rotation) + comp.currentOffset;
                Material mat = __instance.MatAt(thing.Rotation, thing);
                Vector2[] uvs;
                Color32 color;
                Graphic.TryGetTextureAtlasReplacementInfo(mat, thing.def.category.ToAtlasGroup(), drawRotated, true, out mat, out uvs, out color);
                Printer_Plane.PrintPlane(layer, center, size, mat, num, drawRotated, uvs, new Color32[]
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

        static float AngleFromRot(Graphic graphic, Rot4 rot)
		{
			if (graphic.ShouldDrawRotated)
			{
				float num = rot.AsAngle;
				num += graphic.DrawRotatedExtraAngleOffset;
				if ((rot == Rot4.West && graphic.WestFlipped) || (rot == Rot4.East && graphic.EastFlipped))
				{
					num += 180f;
				}
				return num;
			}
			return 0f;
		}
    }
}