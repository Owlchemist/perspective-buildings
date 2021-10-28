using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Linq;
using RimWorld;
using static Perspective.ResourceBank;
using static Perspective.TextureBank;

namespace Perspective
{
	public class CompOffsetter : ThingComp
	{
		public Offsetter Props;
		private int index;
        public Vector3 currentOffset, cachedTrueCenter;
		public bool isOffset, isMirrored, resetNext;
		Command_Action adjustGizmo, mirrorGizmo;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);

			//Modextensions will be impersonating CompProperties, basically
			if (this.parent.def.HasModExtension<Offsetter>()) Props = this.parent.def.GetModExtension<Offsetter>();
		}

		public override void PostDeSpawn(Map map)
		{
			offsetRegistry.Remove(this.parent.thingIDNumber);
			base.PostDeSpawn(map);
		}

		public override void PostExposeData()
		{
			if (Props != null)
			{
				Scribe_Values.Look<bool>(ref resetNext, "resetNext", false, false);
				Scribe_Values.Look<bool>(ref isMirrored, "mirrored", false, false);
				Scribe_Values.Look<int>(ref index, "index", 0, false);
				Scribe_Values.Look<Vector3>(ref currentOffset, "currentOffset", new Vector3(0,0,0), false);

				UpdateRegistry();
				if (currentOffset != zero) isOffset = true;
			}

			//Cache gizmos
			adjustGizmo = new Command_Action()
			{
				defaultLabel = "Owl_Adjust".Translate(),
				defaultDesc = "Owl_AdjustDesc".Translate(),
				icon = iconAdjust,
				action = () => SetCurrentOffset()
			};

			mirrorGizmo = new Command_Action()
			{
				defaultLabel = "Owl_Mirror".Translate(),
				defaultDesc = "Owl_MirrorDesc".Translate(),
				icon = iconMirror,
				action = () => SetMirroredState()
			};

			//Validate index
			if (index > Props.offsets.Count) index = 0;
		}

		public void UpdateRegistry()
		{
			offsetRegistry.Remove(this.parent.thingIDNumber);

			if (isMirrored || currentOffset != zero)
			{
				if (!offsetRegistry.ContainsKey(this.parent.thingIDNumber)) offsetRegistry.Add(this.parent.thingIDNumber, this);
			}
		}

        public void SetCurrentOffset()
        {
			SoundDefOf.Click.PlayOneShotOnCamera(null);
			if (resetNext)
			{
				currentOffset = zero;
				isOffset = false;
				resetNext = false;
				index = 0;
			}
			else
			{
				currentOffset = Props.offsets[index++];
				isOffset = true;
			}

			//Reset the index if we've reached the end
			if (index > Props.offsets.Count - 1) resetNext = true;

			UpdateRegistry();
			if (this.parent.def.drawerType == DrawerType.MapMeshOnly || this.parent.def.drawerType == DrawerType.MapMeshAndRealTime)
			{
				this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
			}
        }

		public void SetMirroredState()
        {
			SoundDefOf.Click.PlayOneShotOnCamera(null);
			isMirrored ^= true;
			UpdateRegistry();
			if (this.parent.def.drawerType == DrawerType.MapMeshOnly || this.parent.def.drawerType == DrawerType.MapMeshAndRealTime)
			{
				this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
			}
        }

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
            if (Props != null && this.parent?.Faction != null && this.parent.Faction.IsPlayer)
            {
                // Adjust
                yield return adjustGizmo;

                // Mirror
                if ((!this.parent.def.rotatable && this.Props.mirror != Offsetter.Override.False) || (this.parent.def.rotatable && this.Props.mirror == Offsetter.Override.True))
                {
                    yield return mirrorGizmo;
                }
            }
		}
	}
}