using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Minecraft.Texture {
    public class AtlasTexture {
        private readonly List<Texture2D> tileTextures = new List<Texture2D>();

        public AtlasTexture(int textureSize, int tileSize) {
            TextureSize = textureSize;
            TileSize = tileSize;
        }

        public int TextureSize { get; }

        public int TileSize { get; }

        public void AddTexture(Texture2D texture) {
            tileTextures.Add(texture);
        }

        public void GenerateTexture(string path) {
            // Remove extension
            path = Path.ChangeExtension(path, null);

            var texture = new Texture2D(TextureSize, TextureSize, TextureFormat.ARGB32, false) {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                alphaIsTransparency = true
            };

            // Clear texture (set all pixels to transparent)
            ClearTexture(texture);

            int x = 0;
            int y = 0;
            foreach (Texture2D tileTexture in tileTextures) {
                CopyTexture(tileTexture, texture, x, y);
                x += TileSize;
                if (x < TextureSize) {
                    continue;
                }
                x = 0;
                y += TileSize;
                if (y < TextureSize) {
                    throw new InvalidDataException("Atlas texture is too small");
                }
            }

            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path + ".png", bytes);
        }

        private static void ClearTexture(Texture2D texture) {
            for (int x = 0; x < texture.width; x++) {
                for (int y = 0; y < texture.height; y++) {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        private void CopyTexture(Texture2D source, Texture2D destination, int x, int y) {
            Debug.Assert(source.isReadable, "Source texture is not readable");
            Debug.Assert(source.width == TileSize, "Source texture width is not 16");
            Debug.Assert(source.height == TileSize, "Source texture height is not 16");

            for (int tileX = 0; tileX < source.width; tileX++) {
                for (int tileY = 0; tileY < source.height; tileY++) {
                    destination.SetPixel(x + tileX, y + tileY, source.GetPixel(tileX, tileY));
                }
            }
        }
    }
}
