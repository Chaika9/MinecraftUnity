using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Minecraft.Block;
using Minecraft.World.Chunk;
using UnityEngine;
using UnityEngine.Events;

namespace Minecraft.World {
    [RequireComponent(typeof(WorldRenderer))]
    public class World : MonoBehaviour {
        [Header("World Settings")]
        [SerializeField] private int seed = 1;

        [SerializeField] private TerrainGenerator terrainGenerator;

        [Header("Chunk Settings")]
        [SerializeField] private int chunkSize = 16;

        [SerializeField] private int chunkHeight = 100;
        [SerializeField] private int chunkRenderDistance = 8;

        [Header("Events")]
        public UnityEvent onWorldLoaded;
        public UnityEvent onChunkLoaded;

        private readonly CancellationTokenSource taskTokenSource = new CancellationTokenSource();

        public BlockManager BlockManager { get; private set; }

        public WorldData WorldData { get; private set; }

        public TerrainGenerator TerrainGenerator => terrainGenerator;

        public int Seed {
            get => seed;
            set => seed = value;
        }

        public int ChunkSize => chunkSize;

        public int ChunkHeight => chunkHeight;

        public int ChunkRenderDistance {
            get => chunkRenderDistance;
            set => chunkRenderDistance = value;
        }

        public WorldRenderer WorldRenderer { get; private set; }

        public bool IsWorldLoaded { get; private set; }

        private void Awake() {
            if (terrainGenerator == null) throw new Exception("TerrainGenerator is not set.");
            WorldData = new WorldData();
            WorldRenderer = GetComponent<WorldRenderer>();
        }

        private void OnDisable() {
            // Cancel all generation tasks
            taskTokenSource.Cancel();
        }

        /// <summary>
        ///     Sets block manager for world.
        /// </summary>
        /// <param name="blockManager">Block manager to set.</param>
        public void SetBlockManager(BlockManager blockManager) {
            Debug.Assert(blockManager != null, "BlockManager is null.");
            BlockManager = blockManager;
        }

        /// <summary>
        ///     Generates new world.
        ///     With default render distance.
        /// </summary>
        public async void GenerateWorldAsync() {
            if (BlockManager == null) {
                throw new Exception("BlockManager is not set.");
            }

            Debug.Log("Generating world...");
            await HandleWorldGeneration(Vector3Int.zero, ChunkRenderDistance);
        }

        /// <summary>
        ///     Generates new world.
        /// </summary>
        /// <param name="forceLoadChunksAround">Force load chunks around this position.</param>
        public async void GenerateWorldAsync(int forceLoadChunksAround) {
            if (BlockManager == null) {
                throw new Exception("BlockManager is not set.");
            }

            Debug.Log("Generating world with force load chunks around " + forceLoadChunksAround + "...");
            await HandleWorldGeneration(Vector3Int.zero, forceLoadChunksAround);
        }

        private async Task HandleWorldGeneration(Vector3Int position, int renderDistance) {
            Debug.Assert(renderDistance >= 0, "Render distance must be greater than or equal to 0.");
            Debug.Assert(renderDistance <= 16, "Render distance must be less than or equal to 16.");

            WorldGenerationData worldGenerationData =
                await Task.Run(() => GetChunkPositionByRenderDistance(position, renderDistance), taskTokenSource.Token);

            // Remove old chunks
            foreach (Vector3Int chunkPosition in worldGenerationData.ChunkPositionsToDestroy) {
                WorldHelper.RemoveChunk(this, chunkPosition);
            }

            foreach (Vector3Int chunkPosition in worldGenerationData.ChunkDataPositionsToDestroy) {
                WorldHelper.RemoveChunkData(this, chunkPosition);
            }

            ConcurrentDictionary<Vector3Int, ChunkData> chunkDatas;
            try {
                // Generate new chunks
                chunkDatas = await GenerateChunkDatas(worldGenerationData.ChunkDataPositionsToGenerate);
            } catch (OperationCanceledException) {
                // If task is cancelled, return
                return;
            }

            // Add new chunk data's to world data
            foreach (KeyValuePair<Vector3Int, ChunkData> item in chunkDatas) {
                WorldData.ChunkDatas.Add(item.Key, item.Value);
            }
            
            try {
                await GenerateAdditionalChunkDatas(chunkDatas);
            } catch (OperationCanceledException) {
                // If task is cancelled, return
                return;
            }

            // Select chunk data to render
            List<ChunkData> chunkDataToRender = WorldData.ChunkDatas
                .Where(item => worldGenerationData.ChunkPositionsToGenerate.Contains(item.Key))
                .Select(item => item.Value)
                .ToList();

            ConcurrentDictionary<Vector3Int, MeshData> chunkMeshDatas;
            try {
                // Create chunk meshes
                chunkMeshDatas = await CreateChunkMeshDatas(chunkDataToRender);
            } catch (OperationCanceledException) {
                // If task is cancelled, return
                return;
            }

            // Start to generate needed chunks
            StartCoroutine(ChunkGenerationCoroutine(chunkMeshDatas));
        }

        /// <summary>
        ///     Destroys world without saving.
        /// </summary>
        public void DestroyWorld() {
            WorldRenderer.Clear(this);
            WorldData.ChunkDatas.Clear();
            WorldData.ChunkRenderers.Clear();
        }

        /// <summary>
        ///     Loads additional chunks around the position. (Calculates with render distance)
        /// </summary>
        /// <param name="position">Position to load chunks around.</param>
        public async void LoadAdditionalChunksAsync(Vector3Int position) {
            if (!IsWorldLoaded) {
                Debug.LogWarning("World is not loaded yet.");
                return;
            }

            Debug.Log("Loading additional chunks around " + position + "...");
            await HandleWorldGeneration(position, ChunkRenderDistance);
            onChunkLoaded?.Invoke();
        }

        private WorldGenerationData GetChunkPositionByRenderDistance(Vector3Int position, int renderDistance) {
            List<Vector3Int> chunkPositionsNeeded =
                WorldHelper.GetChunkPositionsByRenderDistance(this, position, renderDistance);

            List<Vector3Int> chunkPositionsToGenerate =
                WorldHelper.SelectChunkPositionsToGenerate(this, chunkPositionsNeeded, position);
            List<Vector3Int> chunkDataPositionsToGenerate =
                WorldHelper.SelectChunkDataPositionsToGenerate(this, chunkPositionsNeeded, position);

            List<Vector3Int> chunkPositionsToDestroy =
                WorldHelper.SelectChunkPositionsToDestroy(this, chunkPositionsNeeded);
            List<Vector3Int> chunkDataPositionsToDestroy =
                WorldHelper.SelectChunkDataPositionsToDestroy(this, chunkPositionsNeeded);

            return new WorldGenerationData {
                ChunkPositionsToGenerate = chunkPositionsToGenerate,
                ChunkDataPositionsToGenerate = chunkDataPositionsToGenerate,
                ChunkPositionsToDestroy = chunkPositionsToDestroy,
                ChunkDataPositionsToDestroy = chunkDataPositionsToDestroy
            };
        }

        private Task<ConcurrentDictionary<Vector3Int, ChunkData>> GenerateChunkDatas(List<Vector3Int> chunkDataPositionsToGenerate) {
            return Task.Run(() => {
                var chunkDatas = new ConcurrentDictionary<Vector3Int, ChunkData>();
                foreach (Vector3Int chunkPosition in chunkDataPositionsToGenerate) {
                    // Check if task is cancelled
                    if (taskTokenSource.IsCancellationRequested) {
                        // Throw exception if task is cancelled
                        taskTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    var chunkData = new ChunkData(this, chunkPosition, ChunkSize, ChunkHeight);
                    // Generate chunk data with terrain generator (procedural terrain)
                    chunkData = terrainGenerator.GenerateChunkData(this, chunkData);
                    chunkDatas.TryAdd(chunkPosition, chunkData);
                }
                return chunkDatas;
            }, taskTokenSource.Token);
        }

        private Task GenerateAdditionalChunkDatas(ConcurrentDictionary<Vector3Int, ChunkData> chunkDatas) {
            return Task.Run(() => {
                foreach (ChunkData chunkData in chunkDatas.Values) {
                    // Check if task is cancelled
                    if (taskTokenSource.IsCancellationRequested) {
                        // Throw exception if task is cancelled
                        taskTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    terrainGenerator.GenerateAdditionalChunkData(this, chunkData);
                }
            }, taskTokenSource.Token);
        }

        private Task<ConcurrentDictionary<Vector3Int, MeshData>> CreateChunkMeshDatas(
            List<ChunkData> chunkDatasToRender) {
            return Task.Run(() => {
                var chunkMeshDatas = new ConcurrentDictionary<Vector3Int, MeshData>();
                foreach (ChunkData chunkData in chunkDatasToRender) {
                    // Check if task is cancelled
                    if (taskTokenSource.IsCancellationRequested) {
                        // Throw exception if task is cancelled
                        taskTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    // Update chunk mesh data
                    MeshData meshData = ChunkHelper.UpdateMeshData(chunkData);
                    chunkMeshDatas.TryAdd(chunkData.WorldPosition, meshData);
                }
                return chunkMeshDatas;
            }, taskTokenSource.Token);
        }

        private IEnumerator ChunkGenerationCoroutine(ConcurrentDictionary<Vector3Int, MeshData> chunkMeshDatas) {
            foreach (KeyValuePair<Vector3Int, MeshData> item in chunkMeshDatas) {
                CreateChunk(item.Key, item.Value);
                yield return new WaitForEndOfFrame();
            }

            if (IsWorldLoaded) {
                yield break;
            }

            IsWorldLoaded = true;
            onWorldLoaded?.Invoke();
        }

        private void CreateChunk(Vector3Int chunkPosition, MeshData chunkMeshData) {
            ChunkRenderer chunkRenderer = WorldRenderer.RenderChunk(this, chunkPosition, chunkMeshData);
            WorldData.ChunkRenderers.Add(chunkPosition, chunkRenderer);
        }

        /// <summary>
        ///     Sets block at the world position.
        /// </summary>
        /// <param name="blockPosition">World position to set block.</param>
        /// <param name="blockId">Block id to set.</param>
        /// <returns>True if block is set, false if not.</returns>
        public bool SetBlock(Vector3Int blockPosition, string blockId) {
            if (!IsWorldLoaded) {
                Debug.LogWarning("World is not loaded yet.");
                return false;
            }

            ChunkRenderer chunk = WorldHelper.GetChunk(this, blockPosition);
            if (chunk == null) {
                return false;
            }

            WorldHelper.SetBlock(this, blockPosition, blockId);
            chunk.IsModified = true;

            // Update neighbor chunks if block is on the edge of the chunk
            if (WorldHelper.IsOnEdgeOfTheChunk(chunk.ChunkData, blockPosition)) {
                IEnumerable<ChunkData> neighborChunks = WorldHelper.GetNeighbourChunks(chunk.ChunkData, blockPosition);
                foreach (ChunkData neighborChunk in neighborChunks) {
                    ChunkRenderer neighborChunkRenderer = WorldHelper.GetChunk(this, neighborChunk.WorldPosition);
                    if (neighborChunkRenderer != null) {
                        neighborChunkRenderer.UpdateMesh();
                    }
                }
            }

            chunk.UpdateMesh();
            return true;
        }

        /// <summary>
        ///     Gets block id at the world position.
        /// </summary>
        /// <param name="blockPosition">The world position of the block.</param>
        /// <returns>Block id at the world position. Return null if world is not loaded or block is not found.</returns>
        public string GetBlockId(Vector3Int blockPosition) {
            if (!IsWorldLoaded) {
                throw new Exception("World is not loaded yet.");
            }

            ChunkRenderer chunk = WorldHelper.GetChunk(this, blockPosition);
            if (chunk == null) {
                return null;
            }
            return WorldHelper.GetBlockId(this, blockPosition);
        }

        private class WorldGenerationData {
            public List<Vector3Int> ChunkPositionsToGenerate { get; set; }
            public List<Vector3Int> ChunkDataPositionsToGenerate { get; set; }
            public List<Vector3Int> ChunkPositionsToDestroy { get; set; }
            public List<Vector3Int> ChunkDataPositionsToDestroy { get; set; }
        }
    }
}
