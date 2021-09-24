using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;
using UnityEngine;
using static Perspective.ResourceBank;

namespace Perspective
{
    [HarmonyPatch]
    static class Patch_SimpleUtilitiesCeiling
    {
        static MethodBase target;

        static bool Prepare()
        {
            Type type;

            var mod = LoadedModManager.RunningMods.FirstOrDefault(m => m.Name == "Simple Utilities: Ceiling");
            if (mod == null)
			{
                return false;
            }

            type = mod.assemblies.loadedAssemblies
                        .FirstOrDefault(a => a.GetName().Name == "CeilingUtilities")?
                        .GetType("CeilingUtilities.Graphic_FlickerMulti");

            if (type == null)
			{
                Log.Warning("[Perspective: Buildings] Failed to integrate with Simple Utilities: Ceiling. Class not found.");
                return false;
            }

            target = AccessTools.DeclaredMethod(type, "DrawWorker");

            if (target == null)
			{
                Log.Warning("[Perspective: Buildings] Failed to integrate with Simple Utilities: Ceiling. Method not found.");
                return false;
            }

            return true;
        }

        static MethodBase TargetMethod()
        {
            return target;
        }

		static void Prefix(ref Vector3 loc, Thing thing)
		{
            if (offsetRegistry.TryGetValue(thing?.thingIDNumber ?? 0, out compBuffer))
            {
                loc += compBuffer.currentOffset;
            }
        }
    }
}