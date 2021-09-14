using HarmonyLib;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using RimWorld.Planet;

namespace Perspective
{
	public class Mod_Perspective : Mod
	{	
		public Mod_Perspective(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
		}

		public static List<Vector3> standardOffsets = new List<Vector3>() { new Vector3(0,0,0.2f), new Vector3(0,0,-0.2f) };
		public static Vector3 zero = new Vector3(0,0,0);
		public static Dictionary<int,CompOffsetter> offsetRegistry = new Dictionary<int, CompOffsetter>();
    }

	public class WorldComponent_OffsetRegistry : WorldComponent
	{
		public WorldComponent_OffsetRegistry(World world) : base(world)
		{
		}
		public override void FinalizeInit()
		{
			Mod_Perspective.offsetRegistry.Clear();
			base.FinalizeInit();
		}
	}
}