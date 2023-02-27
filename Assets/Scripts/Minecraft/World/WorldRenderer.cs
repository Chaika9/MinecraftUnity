using System;
using System.Collections.Generic;
using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World {
    public class WorldRenderer : MonoBehaviour {
        [Header("Chunk Settings")]
        [SerializeField] private GameObject chunkPrefab;

        [SerializeField] private Transform chunkParent;

        private readonly Queue<ChunkRenderer> renderQueue = new Queue<ChunkRenderer>();

        /// <summary>
        ///     Clear world.
        /// </summary>
        /// <param name="world">World to clear.</param>
        public void Clear(World world) {
            foreach (ChunkRenderer chunkRenderer in world.WorldData.ChunkRenderers.Values) {
                Destroy(chunkRenderer.gameObject);
            }
            renderQueue.Clear();
        }

        /// <summary>
        ///     Remove chunk renderer from world.
        /// </summary>
        /// <param name="chunkRenderer">Chunk renderer to remove.</param>
        public void RemoveChunkRenderer(ChunkRenderer chunkRenderer) {
            chunkRenderer.gameObject.SetActive(false);
            renderQueue.Enqueue(chunkRenderer);
        }

        /// <summary>
        ///     Render chunk.
        /// </summary>
        /// <param name="world">World to render chunk in.</param>
        /// <param name="chunkPosition">Position of chunk to render.</param>
        /// <param name="chunkMeshData">Mesh data of chunk to render.</param>
        /// <returns>Chunk renderer of rendered chunk.</returns>
        public ChunkRenderer RenderChunk(World world, Vector3Int chunkPosition, MeshData chunkMeshData) {
            ChunkRenderer chunkRenderer;
            if (renderQueue.Count > 0) {
                chunkRenderer = renderQueue.Dequeue();
                chunkRenderer.transform.position = chunkPosition;
            } else {
                GameObject chunkObject = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity, chunkParent);
                chunkObject.name = "Chunk " + chunkPosition;
                chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            }

            ChunkData chunkData = world.WorldData.ChunkDatas[chunkPosition];
            if (chunkData == null) {
                throw new Exception($"Chunk data not found at {chunkPosition}");
            }

            chunkRenderer.Initialize(chunkData);
            chunkRenderer.UpdateMesh(chunkMeshData);
            chunkRenderer.gameObject.SetActive(true);
            return chunkRenderer;
        }
    }
}
