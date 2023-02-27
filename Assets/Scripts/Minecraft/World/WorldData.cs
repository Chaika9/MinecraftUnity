using System.Collections.Generic;
using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World {
    public class WorldData {
        public WorldData() {
            ChunkDatas = new Dictionary<Vector3Int, ChunkData>();
            ChunkRenderers = new Dictionary<Vector3Int, ChunkRenderer>();
        }

        public Dictionary<Vector3Int, ChunkData> ChunkDatas { get; }
        public Dictionary<Vector3Int, ChunkRenderer> ChunkRenderers { get; }
    }
}
