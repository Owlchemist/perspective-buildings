using HarmonyLib;
using Verse;
using UnityEngine;
using System.Collections.Generic;

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
    }
}