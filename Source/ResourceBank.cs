using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace Perspective
{
	internal static class ResourceBank
	{	
		public static int transpilerRan = 0;
		public static List<Vector3> standardOffsets = new List<Vector3>() { new Vector3(0,0,0.2f), new Vector3(0,0,-0.2f) };
		public static Vector3 zero = new Vector3(0,0,0);
		public static Dictionary<int,CompOffsetter> offsetRegistry = new Dictionary<int, CompOffsetter>(); //Used to prefetch the ThingComp to avoid using the laggy TryGetComp. Key is the thingID
		
    }
	[StaticConstructorOnStartup]
	internal static class TextureBank
	{	
		public static readonly Texture2D iconMirror = ContentFinder<Texture2D>.Get("UI/Owl_Mirror", true);
		public static readonly Texture2D iconAdjust = ContentFinder<Texture2D>.Get("UI/Owl_Adjust", true);
		
    }
}