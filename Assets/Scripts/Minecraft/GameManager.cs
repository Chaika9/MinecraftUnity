using System;
using System.Collections;
using Minecraft.Block;
using Minecraft.World;
using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft {
    [RequireComponent(typeof(BlockManager))]
    public class GameManager : MonoBehaviour {
        [SerializeField] private World.World world;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float loadCheckTime = 1f;
        
        private Vector3Int currentChunkCenterPosition = Vector3Int.zero;
        private Vector3Int currentPlayerChunkPosition = Vector3Int.zero;

        private BlockManager BlockManager { get; set; }

        private void Awake() {
            if (world == null) {
                throw new Exception("World is null");
            }
            if (playerTransform == null) {
                throw new Exception("Player transform is null");
            }

            BlockManager = GetComponent<BlockManager>();
            world.SetBlockManager(BlockManager);
        }

        /// <summary>
        ///     Spawns the player in the world.
        /// </summary>
        public void SpawnPlayer() {
            var startPosition = new Vector3Int(world.ChunkSize / 2, 100, world.ChunkSize / 2);
            if (Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, 120)) {
                playerTransform.position = hit.point + Vector3.up;
                StartLoadingChunks();
            }
        }

        public void StartLoadingChunks() {
            SetCurrentChunkCoordinates();
            StopAllCoroutines();
            StartCoroutine(CheckIfShouldLoadNextPosition());
        }

        private void SetCurrentChunkCoordinates() {
            currentPlayerChunkPosition = WorldHelper.GetChunkPositionFromWorldPosition(world, Vector3Int.RoundToInt(playerTransform.position));
            currentChunkCenterPosition.x = currentPlayerChunkPosition.x + world.ChunkSize / 2;
            currentChunkCenterPosition.z = currentPlayerChunkPosition.z + world.ChunkSize / 2;
        }

        private IEnumerator CheckIfShouldLoadNextPosition() {
            yield return new WaitForSeconds(loadCheckTime);
            if (Mathf.Abs(currentChunkCenterPosition.x - playerTransform.position.x) > world.ChunkSize
                || Mathf.Abs(currentChunkCenterPosition.z - playerTransform.position.z) > world.ChunkSize
                || Mathf.Abs(currentPlayerChunkPosition.y - playerTransform.position.y) > world.ChunkHeight) {
                world.LoadAdditionalChunksAsync(Vector3Int.RoundToInt(playerTransform.position));
            } else {
                StartCoroutine(CheckIfShouldLoadNextPosition());
            }
        }

        // TODO: Add SetBlock method with raycast
        
        // TODO: Add GetBlock method with raycast
    }
}
