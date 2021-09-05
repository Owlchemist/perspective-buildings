using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using System.Linq;

namespace Perspective
{
	public class CompOffsetter : ThingComp
	{
		public CompProperties_Offsetter Props
		{
			get
			{
				return (CompProperties_Offsetter)this.props;
			}
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look<bool>(ref this.resetNext, "resetNext", false, false);
			Scribe_Values.Look<int>(ref this.index, "index", 0, false);
			Scribe_Values.Look<Vector3>(ref this.currentOffset, "currentOffset", new Vector3(0,0,0), false);
		}

        public void SetCurrentOffset()
        {
			if (resetNext)
			{
				currentOffset = new Vector3(0,0,0);
				resetNext = false;
				index = 0;
			}
			else currentOffset = Props.offsets[index++];

			//Reset the index if we've reached the end
			if (index > Props.offsets.Count() - 1) resetNext = true;

			//Special handling if this is a not a realtime drawer
			if (this.parent.def.drawerType == DrawerType.MapMeshOnly || this.parent.def.drawerType == DrawerType.MapMeshAndRealTime)
			{
				this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
				this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Buildings);
			}
        }

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Log.Message(this.Props.ignore.ToString());
			if (this.parent.Faction != null && this.parent.Faction.IsPlayer && this.Props.ignore == false)
			{
				yield return new Command_Action
				{
					defaultLabel = "Owl_Adjust".Translate(),
					defaultDesc = "Owl_AdjustDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Owl_Offset", true),
					action = delegate()
					{
						LongEventHandler.ExecuteWhenFinished(new Action(this.SetCurrentOffset));
					}
				};
			}
			yield break;
		}
		private bool resetNext = false;
		private int index = 0;
        public Vector3 currentOffset = new Vector3(0,0,0);
	}
}