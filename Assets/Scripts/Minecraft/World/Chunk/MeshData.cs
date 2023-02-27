using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.World.Chunk {
    public class MeshData {
        private readonly bool isTransparentMesh;

        public MeshData() {
            Vertices = new List<Vector3>();
            Triangles = new List<int>();
            UVs = new List<Vector2>();
            ColliderVertices = new List<Vector3>();
            ColliderTriangles = new List<int>();
            if (!isTransparentMesh) {
                TransparentMeshData = new MeshData(true);
            }
        }

        private MeshData(bool isTransparentMesh) {
            Vertices = new List<Vector3>();
            Triangles = new List<int>();
            UVs = new List<Vector2>();
            ColliderVertices = new List<Vector3>();
            ColliderTriangles = new List<int>();
            this.isTransparentMesh = isTransparentMesh;
        }

        public List<Vector3> Vertices { get; }
        public List<int> Triangles { get; }
        public List<Vector2> UVs { get; }
        public List<Vector3> ColliderVertices { get; }
        public List<int> ColliderTriangles { get; }

        public MeshData TransparentMeshData { get; set; }

        public void AddVertex(Vector3 vertex, bool isCollider) {
            Vertices.Add(vertex);
            if (isCollider) {
                ColliderVertices.Add(vertex);
            }
        }

        public void AddQuadTriangles(bool isCollider) {
            Triangles.Add(Vertices.Count - 4);
            Triangles.Add(Vertices.Count - 3);
            Triangles.Add(Vertices.Count - 2);

            Triangles.Add(Vertices.Count - 4);
            Triangles.Add(Vertices.Count - 2);
            Triangles.Add(Vertices.Count - 1);

            // Check if the quad is collider
            if (!isCollider) {
                return;
            }

            ColliderTriangles.Add(ColliderVertices.Count - 4);
            ColliderTriangles.Add(ColliderVertices.Count - 3);
            ColliderTriangles.Add(ColliderVertices.Count - 2);

            ColliderTriangles.Add(ColliderVertices.Count - 4);
            ColliderTriangles.Add(ColliderVertices.Count - 2);
            ColliderTriangles.Add(ColliderVertices.Count - 1);
        }
    }
}
