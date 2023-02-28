using System;
using System.Collections.Generic;
using System.Linq;
using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World {
    public static class WorldHelper {
        /// <summary>
        ///     Gets chunk position from world position.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="worldPosition">The world position of the chunk.</param>
        /// <returns>The chunk position.</returns>
        public static Vector3Int GetChunkPositionFromWorldPosition(World world, Vector3Int worldPosition) {
            return new Vector3Int(
                Mathf.FloorToInt(worldPosition.x / (float)world.ChunkSize) * world.ChunkSize,
                Mathf.FloorToInt(worldPosition.y / (float)world.ChunkHeight) * world.ChunkHeight,
                Mathf.FloorToInt(worldPosition.z / (float)world.ChunkSize) * world.ChunkSize
            );
        }

        /// <summary>
        ///     Gets the chunk at the given world position.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="worldPosition">The world position of the chunk.</param>
        /// <returns>The chunk at the given world position.</returns>
        public static ChunkRenderer GetChunk(World world, Vector3Int worldPosition) {
            Vector3Int chunkPosition = GetChunkPositionFromWorldPosition(world, worldPosition);
            world.WorldData.ChunkRenderers.TryGetValue(chunkPosition, out ChunkRenderer chunkRenderer);
            return chunkRenderer;
        }

        /// <summary>
        ///     Gets the chunk data at the given world position.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="worldPosition">The world position of the chunk.</param>
        /// <returns>The chunk data at the given world position.</returns>
        public static ChunkData GetChunkData(World world, Vector3Int worldPosition) {
            Vector3Int chunkPosition = GetChunkPositionFromWorldPosition(world, worldPosition);
            world.WorldData.ChunkDatas.TryGetValue(chunkPosition, out ChunkData chunkData);
            return chunkData;
        }

        /// <summary>
        ///     Removes the chunk of the world at the given world position.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="chunkPosition">The world position of the chunk.</param>
        public static void RemoveChunk(World world, Vector3Int chunkPosition) {
            if (!world.WorldData.ChunkRenderers.TryGetValue(chunkPosition, out ChunkRenderer chunkRenderer)) {
                return;
            }

            world.WorldRenderer.RemoveChunkRenderer(chunkRenderer);
            world.WorldData.ChunkRenderers.Remove(chunkPosition);
        }
        
        /// <summary>
        /// Removes the chunk data of the world at the given world position.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="chunkPosition">The world position of the chunk.</param>
        public static void RemoveChunkData(World world, Vector3Int chunkPosition) {
            world.WorldData.ChunkDatas.Remove(chunkPosition);
        }
        
        /// <summary>
        ///     Sets the block at the given world position.
        ///     Don't forget to call <see cref="ChunkRenderer.UpdateMesh()" /> after setting the block if you want to apply the
        ///     rendering changes.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="blockPosition">The world position of the block.</param>
        /// <param name="blockId">The block id.</param>
        /// <param name="replace">If true, replace the block if the block is already set.</param>
        /// <returns>True if the block was set, false otherwise.</returns>
        public static bool SetBlock(World world, Vector3Int blockPosition, string blockId, bool replace = true) {
            if (blockId == null) {
                throw new ArgumentNullException(nameof(blockId));
            }

            ChunkData chunkData = GetChunkData(world, blockPosition);
            if (chunkData == null) {
                return false;
            }

            Vector3Int localPosition = ChunkHelper.GetBlockLocalPositionFromWorldPosition(chunkData, blockPosition);
            ChunkHelper.SetBlock(chunkData, localPosition, blockId, replace);
            return true;
        }

        /// <summary>
        ///     Gets the block id at the given world position.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="blockPosition">The world position of the block.</param>
        /// <returns>
        ///     The block id at the given world position. Return null if the chunk is not loaded or the block position is out
        ///     of bounds.
        /// </returns>
        public static string GetBlockId(World world, Vector3Int blockPosition) {
            ChunkData chunkData = GetChunkData(world, blockPosition);
            if (chunkData == null) {
                return null;
            }

            Vector3Int localPosition = ChunkHelper.GetBlockLocalPositionFromWorldPosition(chunkData, blockPosition);
            return ChunkHelper.GetBlockId(chunkData, localPosition);
        }

        public static List<Vector3Int> GetChunkPositionsByRenderDistance(World world, Vector3Int position, int renderDistance) {
            int startX = position.x - renderDistance * world.ChunkSize;
            int startZ = position.z - renderDistance * world.ChunkSize;
            int endX = position.x + renderDistance * world.ChunkSize;
            int endZ = position.z + renderDistance * world.ChunkSize;

            var chunkPositions = new List<Vector3Int>();
            for (int x = startX; x <= endX; x += world.ChunkSize) {
                for (int z = startZ; z <= endZ; z += world.ChunkSize) {
                    Vector3Int chunkPosition = GetChunkPositionFromWorldPosition(world, new Vector3Int(x, 0, z));
                    chunkPositions.Add(chunkPosition);

                    // TODO: Add chunks above and below and check if limit y is reached

                    /*// Check if limit y is reached
                    if (chunkPosition.y >= position.y + 2 * world.ChunkHeight) {
                        continue;
                    }
                    
                    // Add chunks above and below
                    if (x >= position.x - world.ChunkSize && x <= position.x + world.ChunkSize 
                        && z >= position.z - world.ChunkSize && z <= position.z + world.ChunkSize) {
                        for (int y = -world.ChunkHeight; y >= position.y - world.ChunkHeight * 2; y -= world.ChunkHeight) {
                            chunkPosition = GetChunkPositionFromWorldPosition(world, new Vector3Int(x, y, z));
                            chunkPositions.Add(chunkPosition);
                        }
                    }*/
                }
            }
            return chunkPositions;
        }

        public static List<Vector3Int> SelectChunkPositionsToGenerate(World world, IEnumerable<Vector3Int> chunkPositionsNeeded, Vector3Int position) {
            return chunkPositionsNeeded
                .Where(chunkPosition => !world.WorldData.ChunkRenderers.ContainsKey(chunkPosition))
                .OrderBy(chunkPosition => Vector3Int.Distance(chunkPosition, position))
                .ToList();
        }

        public static List<Vector3Int> SelectChunkDataPositionsToGenerate(World world, IEnumerable<Vector3Int> chunkPositionsNeeded, Vector3Int position) {
            return chunkPositionsNeeded
                .Where(chunkPosition => !world.WorldData.ChunkRenderers.ContainsKey(chunkPosition))
                .OrderBy(chunkPosition => Vector3Int.Distance(chunkPosition, position))
                .ToList();
        }

        public static List<Vector3Int> SelectChunkPositionsToDestroy(World world, List<Vector3Int> chunkPositionsNeeded) {
            List<Vector3Int> chunkPositions = world.WorldData.ChunkRenderers.Keys
                .Where(chunkPosition => !chunkPositionsNeeded.Contains(chunkPosition)).ToList();
            return chunkPositions.Where(chunkPosition => world.WorldData.ChunkRenderers.ContainsKey(chunkPosition))
                .ToList();
        }

        public static List<Vector3Int> SelectChunkDataPositionsToDestroy(World world, List<Vector3Int> chunkPositionsNeeded) {
            return world.WorldData.ChunkDatas.Keys
                .Where(chunkPosition => !chunkPositionsNeeded.Contains(chunkPosition))
                .Where(chunkPosition => !world.WorldData.ChunkDatas[chunkPosition].IsModified)
                .ToList();
        }

        /// <summary>
        ///     Check if the block is on the edge of the chunk.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="worldPosition">The block world position.</param>
        /// <returns>True if the block is on the edge of the chunk.</returns>
        public static bool IsOnEdgeOfTheChunk(ChunkData chunkData, Vector3Int worldPosition) {
            Vector3Int localPosition = ChunkHelper.GetBlockLocalPositionFromWorldPosition(chunkData, worldPosition);
            return localPosition.x == 0 || localPosition.x == chunkData.ChunkSize - 1 ||
                localPosition.y == 0 || localPosition.y == chunkData.ChunkHeight - 1 ||
                localPosition.z == 0 || localPosition.z == chunkData.ChunkSize - 1;
        }

        /// <summary>
        ///     Gets the neighbour chunks of the block.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="worldPosition">The block world position.</param>
        /// <returns>The neighbour chunks of the block.</returns>
        public static IEnumerable<ChunkData> GetNeighbourChunks(ChunkData chunkData, Vector3Int worldPosition) {
            Vector3Int localPosition = ChunkHelper.GetBlockLocalPositionFromWorldPosition(chunkData, worldPosition);
            var neighbourChunks = new List<ChunkData>();
            if (localPosition.x == 0) {
                neighbourChunks.Add(GetChunkData(chunkData.World, worldPosition - Vector3Int.right));
            }
            if (localPosition.x == chunkData.ChunkSize - 1) {
                neighbourChunks.Add(GetChunkData(chunkData.World, worldPosition + Vector3Int.right));
            }
            if (localPosition.y == 0) {
                neighbourChunks.Add(GetChunkData(chunkData.World, worldPosition - Vector3Int.up));
            }
            if (localPosition.y == chunkData.ChunkHeight - 1) {
                neighbourChunks.Add(GetChunkData(chunkData.World, worldPosition + Vector3Int.up));
            }
            if (localPosition.z == 0) {
                neighbourChunks.Add(GetChunkData(chunkData.World, worldPosition - Vector3Int.forward));
            }
            if (localPosition.z == chunkData.ChunkSize - 1) {
                neighbourChunks.Add(GetChunkData(chunkData.World, worldPosition + Vector3Int.forward));
            }
            return neighbourChunks;
        }
    }
}
