using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    public class SurfaceLayerHandler : BlockLayerHandler {
        [SerializeField] private string surfaceBlockId = "grass";
        [SerializeField] private string surfaceUndergroundBlockId = "dirt";
        [SerializeField] private int surfaceUndergroundDepth = 4;

        public override bool TryHandling(World world, ChunkData chunkData, Vector3Int blockPosition, int groundPosition) {
            if (blockPosition.y == groundPosition) {
                var localPosition = new Vector3Int(blockPosition.x, blockPosition.y - chunkData.WorldPosition.y, blockPosition.z);
                ChunkHelper.SetBlock(chunkData, localPosition, surfaceBlockId);
                return true;
            }

            if (blockPosition.y < groundPosition && blockPosition.y >= groundPosition - surfaceUndergroundDepth) {
                var localPosition = new Vector3Int(blockPosition.x, blockPosition.y - chunkData.WorldPosition.y, blockPosition.z);
                ChunkHelper.SetBlock(chunkData, localPosition, surfaceUndergroundBlockId);
                return true;
            }
            return false;
        }
    }
}
