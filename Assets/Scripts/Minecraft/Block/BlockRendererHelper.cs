using System;
using System.Collections.Generic;
using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.Block {
    public static class BlockHelper {
        private static readonly Direction[] Directions = {
            Direction.Up,
            Direction.Down,
            Direction.Right,
            Direction.Left,
            Direction.Forward,
            Direction.Back
        };

        /// <summary>
        ///     Gets the block side texture position by direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="blockData">The block data.</param>
        /// <returns>The block side texture position.</returns>
        private static Vector2Int GetBlockSideTexturePositionByDirection(Direction direction, BlockData blockData) {
            return direction switch {
                Direction.Up => blockData.Textures.TopTextureOffset,
                Direction.Down => blockData.Textures.BottomTextureOffset,
                _ => blockData.Textures.SideTextureOffset
            };
        }

        /// <summary>
        ///     Gets the block side UVs.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="blockData">The block data.</param>
        /// <returns>The block side UVs.</returns>
        private static IEnumerable<Vector2> GetBlockSideUVs(Direction direction, BlockData blockData) {
            var uvs = new Vector2[4];
            Vector2Int texturePosition = GetBlockSideTexturePositionByDirection(direction, blockData);
            const float tileSize = (float)BlockManager.AltasTileTextureSize / BlockManager.AltasTextureSize;

            uvs[0] = new Vector2(
                tileSize * texturePosition.x + tileSize - BlockManager.AltasTextureOffset,
                tileSize * texturePosition.y + BlockManager.AltasTextureOffset);

            uvs[1] = new Vector2(
                tileSize * texturePosition.x + tileSize - BlockManager.AltasTextureOffset,
                tileSize * texturePosition.y + tileSize - BlockManager.AltasTextureOffset);

            uvs[2] = new Vector2(tileSize * texturePosition.x + BlockManager.AltasTextureOffset,
                tileSize * texturePosition.y + tileSize - BlockManager.AltasTextureOffset);

            uvs[3] = new Vector2(tileSize * texturePosition.x + BlockManager.AltasTextureOffset,
                tileSize * texturePosition.y + BlockManager.AltasTextureOffset);

            return uvs;
        }

        /// <summary>
        ///     Gets the block side vertices.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="localPosition">The block local position in the chunk.</param>
        /// <returns>The block side vertices.</returns>
        private static IEnumerable<Vector3> GetBlockSideVertices(Direction direction, Vector3 localPosition) {
            var vertices = new Vector3[4];

            switch (direction) {
                case Direction.Right:
                    vertices[0] = new Vector3(localPosition.x + 0.5f, localPosition.y - 0.5f, localPosition.z - 0.5f);
                    vertices[1] = new Vector3(localPosition.x + 0.5f, localPosition.y + 0.5f, localPosition.z - 0.5f);
                    vertices[2] = new Vector3(localPosition.x + 0.5f, localPosition.y + 0.5f, localPosition.z + 0.5f);
                    vertices[3] = new Vector3(localPosition.x + 0.5f, localPosition.y - 0.5f, localPosition.z + 0.5f);
                    break;
                case Direction.Left:
                    vertices[0] = new Vector3(localPosition.x - 0.5f, localPosition.y - 0.5f, localPosition.z + 0.5f);
                    vertices[1] = new Vector3(localPosition.x - 0.5f, localPosition.y + 0.5f, localPosition.z + 0.5f);
                    vertices[2] = new Vector3(localPosition.x - 0.5f, localPosition.y + 0.5f, localPosition.z - 0.5f);
                    vertices[3] = new Vector3(localPosition.x - 0.5f, localPosition.y - 0.5f, localPosition.z - 0.5f);
                    break;
                case Direction.Down:
                    vertices[0] = new Vector3(localPosition.x - 0.5f, localPosition.y - 0.5f, localPosition.z - 0.5f);
                    vertices[1] = new Vector3(localPosition.x + 0.5f, localPosition.y - 0.5f, localPosition.z - 0.5f);
                    vertices[2] = new Vector3(localPosition.x + 0.5f, localPosition.y - 0.5f, localPosition.z + 0.5f);
                    vertices[3] = new Vector3(localPosition.x - 0.5f, localPosition.y - 0.5f, localPosition.z + 0.5f);
                    break;
                case Direction.Up:
                    vertices[0] = new Vector3(localPosition.x - 0.5f, localPosition.y + 0.5f, localPosition.z + 0.5f);
                    vertices[1] = new Vector3(localPosition.x + 0.5f, localPosition.y + 0.5f, localPosition.z + 0.5f);
                    vertices[2] = new Vector3(localPosition.x + 0.5f, localPosition.y + 0.5f, localPosition.z - 0.5f);
                    vertices[3] = new Vector3(localPosition.x - 0.5f, localPosition.y + 0.5f, localPosition.z - 0.5f);
                    break;
                case Direction.Forward:
                    vertices[0] = new Vector3(localPosition.x + 0.5f, localPosition.y - 0.5f, localPosition.z + 0.5f);
                    vertices[1] = new Vector3(localPosition.x + 0.5f, localPosition.y + 0.5f, localPosition.z + 0.5f);
                    vertices[2] = new Vector3(localPosition.x - 0.5f, localPosition.y + 0.5f, localPosition.z + 0.5f);
                    vertices[3] = new Vector3(localPosition.x - 0.5f, localPosition.y - 0.5f, localPosition.z + 0.5f);
                    break;
                case Direction.Back:
                    vertices[0] = new Vector3(localPosition.x - 0.5f, localPosition.y - 0.5f, localPosition.z - 0.5f);
                    vertices[1] = new Vector3(localPosition.x - 0.5f, localPosition.y + 0.5f, localPosition.z - 0.5f);
                    vertices[2] = new Vector3(localPosition.x + 0.5f, localPosition.y + 0.5f, localPosition.z - 0.5f);
                    vertices[3] = new Vector3(localPosition.x + 0.5f, localPosition.y - 0.5f, localPosition.z - 0.5f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            return vertices;
        }

        /// <summary>
        ///     Sets the block side mesh data.
        /// </summary>
        /// <param name="meshData">The chunk mesh data.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="blockData">The block data.</param>
        /// <param name="localPosition">The block local position in the chunk.</param>
        /// <returns>The updated mesh data.</returns>
        private static MeshData SetBlockMeshData(MeshData meshData, Direction direction, BlockData blockData, Vector3Int localPosition) {
            IEnumerable<Vector3> vertices = GetBlockSideVertices(direction, localPosition);
            foreach (Vector3 vertex in vertices) {
                meshData.AddVertex(vertex, blockData.IsCollidable);
            }
            meshData.AddQuadTriangles(blockData.IsCollidable);
            meshData.UVs.AddRange(GetBlockSideUVs(direction, blockData));
            return meshData;
        }

        /// <summary>
        ///     Updates the block mesh data. Updates in function of the neighbor blocks.
        ///     (Ex: If the neighbor block is solid, the block side will not be rendered.)
        /// </summary>
        /// <param name="meshData">The chunk mesh data.</param>
        /// <param name="chunkData">The chunk data.</param>
        /// <param name="blockId">The block id.</param>
        /// <param name="localPosition">The block local position in the chunk.</param>
        /// <returns>The updated mesh data.</returns>
        public static MeshData UpdateBlockMeshData(MeshData meshData, ChunkData chunkData, string blockId, Vector3Int localPosition) {
            // Check if the block is air or if the block it not null
            if (blockId is null or "air") {
                return meshData;
            }

            BlockData blockData = chunkData.World.BlockManager.GetBlockDataById(blockId);
            foreach (Direction direction in Directions) {
                Vector3Int neighborPosition = localPosition + direction.ToVector3Int();
                string neighborBlockId = ChunkHelper.GetBlockId(chunkData, neighborPosition);

                // Check if the neighbor block is in a loaded chunk
                if (neighborBlockId is null) {
                    // meshData = SetBlockMeshData(meshData, direction, blockData, localPosition); // DEBUG : Show block side if the neighbor block is not loaded
                    continue;
                }

                BlockData neighborBlockData = chunkData.World.BlockManager.GetBlockDataById(neighborBlockId);

                // Check if the block is transparent and if the neighbor block is not solid
                if (blockData.IsTransparent && !neighborBlockData.IsSolid) {
                    meshData.TransparentMeshData = SetBlockMeshData(meshData.TransparentMeshData, direction, blockData, localPosition);
                    continue;
                }

                // Check if the neighbor block is not solid
                if (!neighborBlockData.IsSolid || !blockData.IsTransparent && neighborBlockData.IsTransparent) {
                    meshData = SetBlockMeshData(meshData, direction, blockData, localPosition);
                }
            }
            return meshData;
        }
    }
}
