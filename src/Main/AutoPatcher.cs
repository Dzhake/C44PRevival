using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;

namespace DuckGame.C44P;
/// <summary>
/// The handler that does the patching of AutoPatch attributes.
/// </summary>
public static class AutoPatchHandler
{
    /// <summary>
    /// The patching method that finds and executes Auto-Patches.
    /// </summary>
    public static void Patch()
    {
        try
        {
            var harmony = HarmonyLoader.Loader.harmonyInstance;
            DevConsole.Log("ADGM: starting patching", Color.LightGreen);

            foreach (var origInfo in GetAllAutoPatches())
            {
                if (!origInfo.IsStatic)
                {
                    DevConsole.Log("Skipping non-static patch method! (AutoPatch " + origInfo.Name + " in " + origInfo.DeclaringType?.Name + ")", Color.Orange);
                    continue;
                }

                List<AutoPatchAttribute> attributes = origInfo.GetCustomAttributes(typeof(AutoPatchAttribute), false).Cast<AutoPatchAttribute>().ToList();

                foreach (var attribute in attributes)
                {
                    MethodBase mPatch;

                    if (attribute.Method is ".ctor" or "")
                        mPatch = AccessTools.DeclaredConstructor(attribute.Type, attribute.Params);
                    else if (attribute.Method.StartsWith("get_"))
                        mPatch = AccessTools.DeclaredProperty(attribute.Type, attribute.Method.Remove(0,4)).GetGetMethod();
                    else if (attribute.Method.StartsWith("set_"))
                        mPatch = AccessTools.DeclaredProperty(attribute.Type, attribute.Method.Remove(0, 4)).GetSetMethod();
                    else
                        mPatch = AccessTools.DeclaredMethod(attribute.Type, attribute.Method, attribute.Params);

                    if (mPatch is null)
                    {
                        DevConsole.Log("Failed to find specified method: " + attribute.Method + ". on type of: " + attribute.Type.Name, Color.Orange);
                        continue;
                    }

                    switch (attribute.PatchType)
                    {
                        case PatchType.Prefix:
                            harmony.Patch(mPatch, new HarmonyMethod(origInfo));
                            break;
                        case PatchType.Postfix:
                            harmony.Patch(mPatch, null, new HarmonyMethod(origInfo));
                            break;
                        case PatchType.Transpiler:
                            harmony.Patch(mPatch, null, null, new HarmonyMethod(origInfo));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    //DevConsole.Log("Patched method " + origInfo.DeclaringType?.Name + "." + origInfo.Name + " Onto " + attribute.Type.Name + "." + attribute.Method, Color.LightGreen);
                }
            }
        }
        catch (Exception e)
        {
            DevConsole.Log("Error while patching!", Color.Red);
            DevConsole.Log(e.ToString());
        }

        DevConsole.Log("ADGM: patching ended (successfully?)");
    }

    private static IEnumerable<MethodInfo> GetAllAutoPatches()
    {
        return Assembly.GetExecutingAssembly().GetTypes().SelectMany(x => x.GetMethods()).Where(x =>
            x.GetCustomAttributes(typeof(AutoPatchAttribute), false).FirstOrDefault() != null);
    }
}