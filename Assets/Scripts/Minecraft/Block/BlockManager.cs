using System;
using System.Collections.Generic;
using System.Linq;
using Minecraft.Texture;
using UnityEditor;
using UnityEngine;

namespace Minecraft.Block {
    public class BlockManager : MonoBehaviour {
        public const int AltasTextureSize = 256;
        public const int AltasTileTextureSize = 16;
        public const float AltasTextureOffset = 0.001f;

        [SerializeField] private BlockList blockList;

        private readonly Dictionary<string, BlockData> blockDatas = new Dictionary<string, BlockData>();
        private readonly Dictionary<ushort, string> blockIds = new Dictionary<ushort, string>();

        public Material BlockMaterial { get; private set; }
        public Material BlockTransparentMaterial { get; private set; }

        private void Awake() {
            if (blockList == null) {
                throw new Exception("Block list is null");
            }

            InitializeBlocks();
        }

        private void InitializeBlocks() {
            Debug.Log("Initializing blocks");

            // Clear before initializing
            blockDatas.Clear();

            Debug.Log("Registering blocks...");
            // Register all blocks
            foreach (BlockData blockData in blockList.Blocks) {
                string blockId = FormatBlockId(blockData.BlockId);
                blockDatas.Add(blockId, blockData);
            }

            Debug.Log("Generating atlas texture...");
            const string atlasTexturePath = "Assets/Textures/Block_Atlas.png";
            GenerateAtlasTexture(atlasTexturePath);

            Debug.Log("Importing atlas texture...");
            // Import atlas texture to Asset Database
            AssetDatabase.ImportAsset(atlasTexturePath);
            AssetDatabase.Refresh();

            // Load atlas texture from Asset Database and set some properties
            var altasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasTexturePath);
            altasTexture.alphaIsTransparency = true;
            altasTexture.filterMode = FilterMode.Point;
            altasTexture.wrapMode = TextureWrapMode.Clamp;

            Debug.Log("Generating block materials...");
            // Generate block materials
            BlockMaterial = BlockMaterialHelper.CreateBlockMaterial("Assets/Materials/BlocksAtlasMaterial", altasTexture);
            // Generate transparent block materials
            BlockTransparentMaterial = BlockMaterialHelper.CreateBlockMaterial("Assets/Materials/BlocksAtlasMaterialTransparent", altasTexture, true);

            Debug.Log("Generating block ids...");
            // Generate block ids
            GenerateBlockIds();
        }

        private void GenerateAtlasTexture(string path) {
            var atlasTexture = new AtlasTexture(AltasTextureSize, AltasTileTextureSize);

            int x = 0;
            int y = 0;
            foreach (KeyValuePair<string, BlockData> item in blockDatas.Where(item => item.Value.IsSolid)) {
                if (item.Value.Textures.TopTexture != null) {
                    atlasTexture.AddTexture(item.Value.Textures.TopTexture);
                    item.Value.Textures.TopTextureOffset = new Vector2Int(x, y);
                    x++;
                } else {
                    throw new Exception("Top texture is null");
                }

                if (item.Value.Textures.BottomTexture != null) {
                    atlasTexture.AddTexture(item.Value.Textures.BottomTexture);
                    item.Value.Textures.BottomTextureOffset = new Vector2Int(x, y);
                    x++;
                } else {
                    item.Value.Textures.BottomTextureOffset = item.Value.Textures.TopTextureOffset;
                }
                if (item.Value.Textures.SideTexture != null) {
                    atlasTexture.AddTexture(item.Value.Textures.SideTexture);
                    item.Value.Textures.SideTextureOffset = new Vector2Int(x, y);
                    x++;
                } else {
                    item.Value.Textures.SideTextureOffset = item.Value.Textures.TopTextureOffset;
                }

                if (!(x >= (float)AltasTextureSize / AltasTileTextureSize)) {
                    continue;
                }
                x = 0;
                y++;
            }

            // Generate atlas texture
            atlasTexture.GenerateTexture(path);
        }

        private void GenerateBlockIds() {
            // Clear before generating
            blockIds.Clear();

            ushort id = 0;
            foreach (KeyValuePair<string, BlockData> item in blockDatas) {
                blockIds.Add(id, item.Key);
                id++;
            }
        }

        /// <summary>
        ///     Formats the block id.
        /// </summary>
        /// <param name="blockId">The block id to format.</param>
        /// <returns>The formatted block id.</returns>
        private static string FormatBlockId(string blockId) {
            // Replace all spaces with underscores
            blockId = blockId.Replace(" ", "_");
            return blockId.ToLower();
        }

        /// <summary>
        ///     Gets the block data by block id.
        /// </summary>
        /// <param name="blockId">The block id.</param>
        /// <returns>The corresponding block data.</returns>
        public BlockData GetBlockDataById(string blockId) {
            if (blockId == null) {
                throw new ArgumentNullException(nameof(blockId));
            }
            if (!blockDatas.ContainsKey(blockId)) {
                throw new Exception($"Block data with id {blockId} does not exist");
            }
            return blockDatas[blockId];
        }

        /// <summary>
        ///     Gets the block data by block id.
        /// </summary>
        /// <param name="shortBlockId">The block id.</param>
        /// <returns>The corresponding block data.</returns>
        public BlockData GetBlockDataById(ushort shortBlockId) {
            if (!blockIds.ContainsKey(shortBlockId)) {
                throw new Exception($"Block data with id {shortBlockId} does not exist");
            }
            return GetBlockDataById(blockIds[shortBlockId]);
        }

        public ushort ConvertToShortId(string blockId) {
            if (blockId == null) {
                throw new ArgumentNullException(nameof(blockId));
            }
            if (!blockDatas.ContainsKey(blockId)) {
                throw new Exception($"Block data with id {blockId} does not exist");
            }
            return blockIds.First(x => x.Value == blockId).Key;
        }
    }
}
