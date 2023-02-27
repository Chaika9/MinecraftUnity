using Minecraft.World.Biome;
using UnityEngine;

namespace Minecraft.World.Chunk {
    public class ChunkData {
        public ChunkData(World world, Vector3Int worldPosition, int chunkSize, int chunkHeight) {
            World = world;
            WorldPosition = worldPosition;
            ChunkSize = chunkSize;
            ChunkHeight = chunkHeight;
            Blocks = new ushort[chunkSize * chunkHeight * chunkSize];
            IsModified = false;
        }

        /// <summary>
        ///     Returns block id list containing all blocks in the chunk.
        /// </summary>
        public ushort[] Blocks { get; }

        /// <summary>
        ///     Returns the chunk size. (x and z)
        /// </summary>
        public int ChunkSize { get; }

        /// <summary>
        ///     Returns the chunk height. (y)
        /// </summary>
        public int ChunkHeight { get; }

        /// <summary>
        ///     Returns world reference who owns this chunk.
        /// </summary>
        public World World { get; }

        /// <summary>
        ///     Returns the chunk position in the world.
        /// </summary>
        public Vector3Int WorldPosition { get; }

        /// <summary>
        ///     Returns if the chunk has been modified by placing or removing blocks. (Example: if the chunk has been modified by
        ///     the player)
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        ///     Returns the tree data for this chunk.
        /// </summary>
        public TreeData TreeData { get; set; }
    }
}
