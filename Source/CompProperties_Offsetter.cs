using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace Perspective
{
	public class CompProperties_Offsetter : CompProperties
	{
      public List<Vector3> offsets;
      public bool ignore = false;
      public bool mirror = true;
      public CompProperties_Offsetter()
      {
         this.compClass = typeof(CompOffsetter);
         if (offsets == null) offsets = Mod_Perspective.standardOffsets;
      }
   }
}