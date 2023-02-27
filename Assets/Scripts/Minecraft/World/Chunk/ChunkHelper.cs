using System;
using Minecraft.Block;
using UnityEngine;

namespace Minecraft.World.Chunk {
    public static class ChunkHelper {
        /// <summary>
        ///     Gets the block index in the chunk block array from the block local position.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="localPosition">The block local position in the chunk.</param>
        /// <returns>The block index in the chunk block array.</returns>
        private static int GetIndexFromBlockLocalPosition(ChunkData chunkData, Vector3Int localPosition) {
            return localPosition.x + chunkData.ChunkSize * localPosition.y +
                chunkData.ChunkSize * chunkData.ChunkHeight * localPosition.z;
        }

        /// <summary>
        ///     Gets the block local position in the chunk from the block index in the chunk block array.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="index">The block index in the chunk block array.</param>
        /// <returns>The block local position in the chunk.</returns>
        private static Vector3Int GetBlockLocalPositionFromIndex(ChunkData chunkData, int index) {
            return new Vector3Int(
                index % chunkData.ChunkSize,
                index / chunkData.ChunkSize % chunkData.ChunkHeight,
                index / (chunkData.ChunkSize * chunkData.ChunkHeight)
            );
        }

        /// <summary>
        ///     Gets the block local position in the chunk from the block world position.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="worldPosition">The block world position.</param>
        /// <returns>The block local position in the chunk.</returns>
        public static Vector3Int GetBlockLocalPositionFromWorldPosition(ChunkData chunkData, Vector3Int worldPosition) {
            return new Vector3Int(
                worldPosition.x - chunkData.WorldPosition.x,
                worldPosition.y - chunkData.WorldPosition.y,
                worldPosition.z - chunkData.WorldPosition.z
            );
        }

        /// <summary>
        ///     Check if the position is in bounds of the chunk.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="localPosition">The local position in the chunk to check.</param>
        /// <returns>True if the position is in bounds of the chunk.</returns>
        private static bool InBounds(ChunkData chunkData, Vector3Int localPosition) {
            return localPosition.x >= 0 && localPosition.x < chunkData.ChunkSize &&
                localPosition.y >= 0 && localPosition.y < chunkData.ChunkHeight &&
                localPosition.z >= 0 && localPosition.z < chunkData.ChunkSize;
        }

        /// <summary>
        ///     Gets the block local position in the chunk.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="actionToPerform">The action to perform on every block local position in the chunk.</param>
        private static void IterateBlockLocalPosition(ChunkData chunkData, Action<Vector3Int> actionToPerform) {
            for (int index = 0; index < chunkData.Blocks.Length; index++) {
                actionToPerform(GetBlockLocalPositionFromIndex(chunkData, index));
            }
        }

        /// <summary>
        ///     Updates the chunk mesh.
        ///     Returns the updated mesh data. But does not apply the mesh to the chunk.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <returns>The updated mesh data.</returns>
        public static MeshData UpdateMeshData(ChunkData chunkData) {
            var meshData = new MeshData();
            IterateBlockLocalPosition(chunkData, localPosition => {
                string blockId = GetBlockId(chunkData, localPosition);
                meshData = BlockHelper.UpdateBlockMeshData(meshData, chunkData, blockId, localPosition);
            });
            return meshData;
        }

        /// <summary>
        ///     Gets the block in the chunk.
        ///     If the block is out of bounds of the chunk, get the block in world position.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="localPosition">The block local position in the chunk.</param>
        /// <returns>
        ///     The block id at the given world position. Return null if the chunk is not loaded or the block position is out
        ///     of bounds.
        /// </returns>
        public static string GetBlockId(ChunkData chunkData, Vector3Int localPosition) {
            if (!InBounds(chunkData, localPosition)) {
                return WorldHelper.GetBlockId(chunkData.World, localPosition + chunkData.WorldPosition);
            }

            ushort blockId = chunkData.Blocks[GetIndexFromBlockLocalPosition(chunkData, localPosition)];
            return chunkData.World.BlockManager.GetBlockDataById(blockId).BlockId;
        }

        /// <summary>
        ///     Sets the block in the chunk.
        ///     If the block is out of bounds of the chunk, set the block in world position.
        ///     Not automatically update the chunk mesh.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="localPosition">The block local position in the chunk.</param>
        /// <param name="blockId">The block id.</param>
        /// <param name="replace">If true, replace the block if the block is already set.</param>
        public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, string blockId, bool replace = true) {
            if (blockId == null) {
                throw new ArgumentNullException(nameof(blockId));
            }

            if (!InBounds(chunkData, localPosition)) {
                WorldHelper.SetBlock(chunkData.World, localPosition + chunkData.WorldPosition, blockId, replace);
                return;
            }

            ushort shortBlockId = chunkData.World.BlockManager.ConvertToShortId(blockId);
            if (replace || chunkData.Blocks[GetIndexFromBlockLocalPosition(chunkData, localPosition)] == 0) {
                chunkData.Blocks[GetIndexFromBlockLocalPosition(chunkData, localPosition)] = shortBlockId;
            }
        }
    }
}
