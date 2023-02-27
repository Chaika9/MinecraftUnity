using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    public abstract class BlockLayerHandler : MonoBehaviour {
        public abstract bool TryHandling(World world, ChunkData chunkData, Vector3Int blockPosition, int groundPosition);
    }
}
