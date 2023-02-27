using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Minecraft.Block {
    public static class BlockMaterialHelper {
        public static Material CreateBlockMaterial(string path, Texture2D altasTexture, bool isTransparent = false) {
            // Remove extension
            path = Path.ChangeExtension(path, null);

            var material = new Material(Shader.Find("Standard")) {
                name = "BlocksAtlasMaterial",
                mainTexture = altasTexture
            };

            // Set Smoothness value
            material.SetFloat(Shader.PropertyToID("_Glossiness"), 0.0f);

            // Disable Reflections
            material.SetFloat(Shader.PropertyToID("_GlossyReflections"), 0.0f);

            if (isTransparent) {
                // Set Blend Mode to Alpha Blend
                material.SetFloat(Shader.PropertyToID("_Mode"), 3.0f);
                material.SetInt(Shader.PropertyToID("_SrcBlend"), (int)BlendMode.SrcAlpha);
                material.SetInt(Shader.PropertyToID("_DstBlend"), (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt(Shader.PropertyToID("_ZWrite"), 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }

            AssetDatabase.DeleteAsset(path + ".mat");
            AssetDatabase.CreateAsset(material, path + ".mat");
            AssetDatabase.SaveAssets();

            return material;
        }
    }
}
