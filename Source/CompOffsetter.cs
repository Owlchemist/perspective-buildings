using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Linq;
using RimWorld;

namespace Perspective
{
	public class CompOffsetter : ThingComp
	{
		public Offsetter Props;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			Mod_Perspective.offsetRegistry.Add(this.parent.thingIDNumber, this);

			//Modextensoins will be impersonating CompProperties, basically
			if (this.parent.def.HasModExtension<Offsetter>()) Props = this.parent.def.GetModExtension<Offsetter>();
		}

		public override void PostDeSpawn(Map map)
		{
			Mod_Perspective.offsetRegistry.Remove(this.parent.thingIDNumber);
			base.PostDeSpawn(map);
		}

		public override void PostExposeData()
		{
			if (Props != null)
			{
				Scribe_Values.Look<bool>(ref this.resetNext, "resetNext", false, false);
				Scribe_Values.Look<bool>(ref this.mirrored, "mirrored", false, false);
				Scribe_Values.Look<int>(ref this.index, "index", 0, false);
				Scribe_Values.Look<Vector3>(ref this.currentOffset, "currentOffset", new Vector3(0,0,0), false);

				Mod_Perspective.offsetRegistry[this.parent.thingIDNumber] = (mirrored || currentOffset != Mod_Perspective.zero) ? this : null;
				if (currentOffset != Mod_Perspective.zero) isOffset = true;
			}
		}

        public void SetCurrentOffset()
        {
			SoundDefOf.Click.PlayOneShotOnCamera(null);
			if (resetNext)
			{
				currentOffset = new Vector3(0,0,0);
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

			//Special handling if this is a not a realtime drawer
			if (this.parent.def.drawerType == DrawerType.MapMeshOnly || this.parent.def.drawerType == DrawerType.MapMeshAndRealTime)
			{
				this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
			}

			Mod_Perspective.offsetRegistry[this.parent.thingIDNumber] = (mirrored || currentOffset != Mod_Perspective.zero) ? this : null;
        }

		public void SetMirroredState()
        {
			SoundDefOf.Click.PlayOneShotOnCamera(null);
			mirrored ^= true;
			//Special handling if this is a not a realtime drawer
			if (this.parent.def.drawerType == DrawerType.MapMeshOnly || this.parent.def.drawerType == DrawerType.MapMeshAndRealTime)
			{
				this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
			}

			Mod_Perspective.offsetRegistry[this.parent.thingIDNumber] = (mirrored || currentOffset != Mod_Perspective.zero) ? this : null;
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
        public Vector3 currentOffset = new Vector3(0,0,0);
		public bool isOffset = false;
		public bool mirrored = false;
	}
}