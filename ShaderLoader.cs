using System.Collections.Generic;
using UnityEngine;

namespace VolumetricEffects
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class ShaderLoader : MonoBehaviour
    {
        private static Dictionary<string, Shader> shaders = null;
        public static bool isLoaded = false;

        private void Start()
        {
            Debug.Log("[VolumetricEffects] Starting Shader Loader!");
            LoadShaders();
        }

        // Load all shaders from assetbundle
        private void LoadShaders()
        {
            // Return if already set
            if (shaders != null)
            {
                return;
            }

            shaders = new Dictionary<string, Shader>();
            foreach (Shader shader in Resources.FindObjectsOfTypeAll<Shader>())
            {
                shaders[shader.name] = shader;
            }

            using (WWW www = new WWW("file://" + KSPUtil.ApplicationRootPath + "GameData/VolumetricEffects/volumetriceffects.bundle"))
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.Log("[VolumetricEffects] shaderbundle.bundle not detected!");
                    return;
                }

                AssetBundle bundle = www.assetBundle;
                foreach (Shader shader in bundle.LoadAllAssets<Shader>())
                {
                    Debug.Log($"[VolumetricEffects] Shader {shader.name} has been loaded!");
                    shaders.Add(shader.name, shader);
                }
                bundle.Unload(false);
                www.Dispose();
            }

            isLoaded = true;
        }

        // Util used to find a loaded shader! Might move to a Utils class
        public static Shader shaderFinder(string name)
        {
            if (shaders == null)
            {
                Debug.Log("[Volumetric Effects] Cannot load shader before asset bundle is loaded!");
                return null;
            }
            if (shaders.ContainsKey(name))
            {
                Debug.Log($"[Volumetric Effects] Found shader by the name of {name}");
                return shaders[name];
            }
            Debug.Log($"[Volumetric Effects] Could not located shader by the name of {name}");
            return null;
        }
    }
}