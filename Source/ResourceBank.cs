using UnityEngine;
using System.Collections.Generic;

namespace Perspective
{
	internal static class ResourceBank
	{	
		public static int transpilerRan = 0;
		public static List<Vector3> standardOffsets = new List<Vector3>() { new Vector3(0,0,0.2f), new Vector3(0,0,-0.2f) };
		public static Vector3 zero = new Vector3(0,0,0);
		public static CompOffsetter compBuffer = null; //Micro-optimization, moving outside the loop seems to help a small amount.
		public static Dictionary<int,CompOffsetter> offsetRegistry = new Dictionary<int, CompOffsetter>(); //Used to prefetch the ThingComp to avoid using the laggy TryGetComp
    }
}