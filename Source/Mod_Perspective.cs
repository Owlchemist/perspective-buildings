using HarmonyLib;
using Verse;
using RimWorld.Planet;
using static Perspective.ResourceBank;

namespace Perspective
{
	public class Mod_Perspective : Mod
	{	
		public Mod_Perspective(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
		}
    }

	public class WorldComponent_OffsetRegistry : WorldComponent
	{
		public WorldComponent_OffsetRegistry(World world) : base(world)
		{
		}
		public override void FinalizeInit()
		{
			offsetRegistry.Clear();
			base.FinalizeInit();
		}
	}
}