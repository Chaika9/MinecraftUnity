using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Minecraft.World.Chunk {
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class ChunkRenderer : MonoBehaviour {
        [SerializeField] private bool format32BitIndices = true;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public Mesh Mesh { get; private set; }

        public ChunkData ChunkData { get; private set; }

        public bool IsModified {
            get => ChunkData.IsModified;
            set => ChunkData.IsModified = value;
        }

        private void Awake() {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            meshRenderer = GetComponent<MeshRenderer>();
            Mesh = meshFilter.mesh;
        }

    #if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (ChunkData == null || Selection.activeGameObject != gameObject) {
                return;
            }

            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawWireCube(
                transform.position + new Vector3(ChunkData.ChunkSize / 2.0f - 0.5f, ChunkData.ChunkHeight / 2.0f - 0.5f,
                    ChunkData.ChunkSize / 2.0f - 0.5f),
                new Vector3(ChunkData.ChunkSize, ChunkData.ChunkHeight, ChunkData.ChunkSize));
        }
    #endif

        /// <summary>
        ///     Initializes the chunk renderer.
        /// </summary>
        /// <param name="chunkData">The chunk data.</param>
        public void Initialize(ChunkData chunkData) {
            Debug.Assert(chunkData != null, "Chunk data is null");
            ChunkData = chunkData;
        }

        private void RecalculateMesh(MeshData chunkMeshData) {
            Debug.Assert(chunkMeshData != null, "Chunk mesh data is null");

            // Clear mesh before updating
            Mesh.Clear();

            // Add support for 32-bit indices (for large meshes)
            if (format32BitIndices) {
                Mesh.indexFormat = IndexFormat.UInt32;
            }

            // Set mesh materials
            var materials = new Material[2];
            materials[0] = ChunkData.World.BlockManager.BlockMaterial;
            materials[1] = ChunkData.World.BlockManager.BlockTransparentMaterial;
            meshRenderer.materials = materials;

            Mesh.subMeshCount = 2;
            Mesh.vertices = chunkMeshData.Vertices
                .Concat(chunkMeshData.TransparentMeshData.Vertices)
                .ToArray();

            Mesh.SetTriangles(chunkMeshData.Triangles.ToArray(), 0);
            Mesh.SetTriangles(chunkMeshData.TransparentMeshData.Triangles
                .Select(val => val + chunkMeshData.Vertices.Count())
                .ToArray(), 1);

            Mesh.uv = chunkMeshData.UVs.Concat(chunkMeshData.TransparentMeshData.UVs).ToArray();

            Mesh.RecalculateNormals();

            // Update mesh collider
            meshCollider.sharedMesh = null;
            var collisionMesh = new Mesh {
                vertices = chunkMeshData.ColliderVertices.Concat(chunkMeshData.TransparentMeshData.ColliderVertices).ToArray(),
                triangles = chunkMeshData.ColliderTriangles.Concat(chunkMeshData.TransparentMeshData.ColliderTriangles).ToArray()
            };
            collisionMesh.RecalculateNormals();
            meshCollider.sharedMesh = collisionMesh;
        }

        /// <summary>
        ///     Creates or updates the chunk mesh.
        /// </summary>
        public void UpdateMesh() {
            RecalculateMesh(ChunkHelper.UpdateMeshData(ChunkData));
        }

        /// <summary>
        ///     Creates or updates the chunk mesh.
        /// </summary>
        /// <param name="meshData">The updated mesh data.</param>
        public void UpdateMesh(MeshData meshData) {
            Debug.Assert(meshData != null, "Mesh data is null");
            RecalculateMesh(meshData);
        }
    }
}
