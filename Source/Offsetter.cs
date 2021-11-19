using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace Perspective
{
	public class Offsetter : DefModExtension 
	{
		public Override mirror = Override.Normal;
		public Override ignore = Override.Normal;
		public enum Override {Normal, True, False}
		public List<Vector3> offsets;		
		public OffsetType offsetType;
		public enum OffsetType {Normal, Eight, Four}
	}
}