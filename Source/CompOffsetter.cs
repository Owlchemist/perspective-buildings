using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Linq;
using RimWorld;
using static Perspective.ResourceBank;


namespace Perspective
{
	public class CompOffsetter : ThingComp
	{
		public Offsetter Props;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);

			//Modextensoins will be impersonating CompProperties, basically
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
				Scribe_Values.Look<bool>(ref this.resetNext, "resetNext", false, false);
				Scribe_Values.Look<bool>(ref this.isMirrored, "mirrored", false, false);
				Scribe_Values.Look<int>(ref this.index, "index", 0, false);
				Scribe_Values.Look<Vector3>(ref this.currentOffset, "currentOffset", new Vector3(0,0,0), false);

				UpdateRegistry();
				if (currentOffset != zero) isOffset = true;
			}
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
			if (index > Props.offsets.Count() - 1) resetNext = true;

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
                yield return new Command_Action()
                {
                    defaultLabel = "Owl_Adjust".Translate(),
                    defaultDesc = "Owl_AdjustDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Owl_Adjust", false),
                    action = () => SetCurrentOffset()
                };

                // Mirror
                if ((!this.parent.def.rotatable && this.Props.mirror != Offsetter.Override.False) || (this.parent.def.rotatable && this.Props.mirror == Offsetter.Override.True))
                {
                    yield return new Command_Action()
                    {
                    	defaultLabel = "Owl_Mirror".Translate(),
						defaultDesc = "Owl_MirrorDesc".Translate(),
						icon = ContentFinder<Texture2D>.Get("UI/Owl_Mirror", false),
                        action = () => SetMirroredState()
                    };
                }
            }
		}
		private bool resetNext = false;
		private int index = 0;
        public Vector3 currentOffset = zero;
		public Vector3 cachedTrueCenter = zero;
		public bool isOffset = false; //Notes that the thing is in the registry because it's offset, in case it's only in the registry because it's mirrored
		public bool isMirrored = false;
	}
}