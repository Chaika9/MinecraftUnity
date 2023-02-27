using System;
using UnityEngine;

namespace Minecraft.Block {
    [CreateAssetMenu(fileName = "Block", menuName = "Minecraft/Block")]
    public class BlockData : ScriptableObject {
        [Tooltip("Whitespace, uppercase letters, special characters, and numbers are not allowed. \nExample: \"oak_planks\"")]
        [SerializeField] private string blockId;

        [SerializeField] private string blockName;
        [SerializeField] private bool isSolid;
        [SerializeField] private bool isTransparent;
        [SerializeField] private bool isCollidable;
        [SerializeField] private TextureData textures;

        /// <summary>
        ///     The ID of the block. This is used to identify the block in the world.
        /// </summary>
        public string BlockId => blockId;

        /// <summary>
        ///     The default name of the block. (english)
        /// </summary>
        public string BlockName => blockName;

        /// <summary>
        ///     Is the block solid? (can it be walked through?)
        ///     Warning: Liquid blocks are solid.
        /// </summary>
        public bool IsSolid => isSolid;

        /// <summary>
        ///     Is the block transparent? (can you see through it?)
        /// </summary>
        public bool IsTransparent => isTransparent;

        /// <summary>
        ///     Is the block collidable? (can you collide with it?)
        /// </summary>
        public bool IsCollidable => isCollidable;

        /// <summary>
        ///     The textures of the block.
        /// </summary>
        public TextureData Textures => textures;

        [Serializable]
        public class TextureData {
            [SerializeField] private Texture2D topTexture;
            [SerializeField] private Texture2D bottomTexture;
            [SerializeField] private Texture2D sideTexture;

            public Texture2D TopTexture => topTexture;

            public Texture2D BottomTexture => bottomTexture;

            public Texture2D SideTexture => sideTexture;

            public Vector2Int TopTextureOffset { get; set; }

            public Vector2Int BottomTextureOffset { get; set; }

            public Vector2Int SideTextureOffset { get; set; }
        }
    }
}
