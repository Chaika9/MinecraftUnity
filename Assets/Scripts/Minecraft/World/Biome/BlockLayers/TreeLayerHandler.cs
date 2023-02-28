using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    public class TreeLayerHandler : BlockLayerHandler {
        [SerializeField] private float terrainHeightLimit = 25f;

        public override bool TryHandling(World world, ChunkData chunkData, Vector3Int blockPosition, int groundPosition) {
            if (chunkData.WorldPosition.y < 0 || groundPosition > terrainHeightLimit) {
                return false;
            }

            if (chunkData.TreeData.TreePositions.Contains(new Vector2Int(blockPosition.x, blockPosition.z))) {
                var groundBlockPosition = new Vector3Int(blockPosition.x, groundPosition - chunkData.WorldPosition.y, blockPosition.z);
                string blockId = ChunkHelper.GetBlockId(chunkData, groundBlockPosition);
                if (blockId is "grass" or "dirt") {
                    ChunkHelper.SetBlock(chunkData, groundBlockPosition, "dirt");
                    GenerateTree(chunkData, groundBlockPosition);
                }
                return true;
            }
            return false;
        }

        // TODO: Move this to a separate class (structure generator)
        private void GenerateTree(ChunkData chunkData, Vector3Int groundBlockPosition) {
            // TODO: Add randomization for trunk height (min, max)
            int trunkHeight = 5;

            // Generate trunk
            for (int y = 1; y <= trunkHeight; y++) {
                var blockPosition = new Vector3Int(groundBlockPosition.x, groundBlockPosition.y + y, groundBlockPosition.z);
                ChunkHelper.SetBlock(chunkData, blockPosition, "log");
            }

            // Generate leaves
            for (int y = trunkHeight - 2; y < trunkHeight; y++) {
                for (int x = -2; x < 3; x++) {
                    for (int z = -2; z < 3; z++) {
                        if (x == 0 && z == 0) {
                            continue;
                        }
                        if (x == -2 && z == -2 || x == -2 && z == 2 || x == 2 && z == -2 || x == 2 && z == 2) {
                            continue;
                        }
                        var blockPosition = new Vector3Int(groundBlockPosition.x + x, groundBlockPosition.y + y, groundBlockPosition.z + z);
                        ChunkHelper.SetBlock(chunkData, blockPosition, "oak_leaves", false);
                    }
                }
            }

            for (int y = trunkHeight; y < trunkHeight + 2; y++) {
                for (int x = -1; x < 2; x++) {
                    for (int z = -1; z < 2; z++) {
                        if (x == 0 && z == 0 && y < trunkHeight + 1) {
                            continue;
                        }
                        if (x % 2 == 0 || z % 2 == 0) {
                            var blockPosition = new Vector3Int(groundBlockPosition.x + x, groundBlockPosition.y + y, groundBlockPosition.z + z);
                            ChunkHelper.SetBlock(chunkData, blockPosition, "oak_leaves", false);
                        }
                    }
                }
            }
        }
    }
}
