using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Minecraft.World.Chunk {
    [CustomEditor(typeof(ChunkRenderer))]
    [CanEditMultipleObjects]
    public class ChunkEditor : Editor {
        private ChunkRenderer[] chunkRenderers;
        private bool showChunkData = true;

        private void OnEnable() {
            Object[] monoObjects = targets;
            chunkRenderers = new ChunkRenderer[monoObjects.Length];
            for (int i = 0; i < monoObjects.Length; i++) chunkRenderers[i] = monoObjects[i] as ChunkRenderer;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            // Check if the game is running
            if (!Application.isPlaying) {
                EditorGUILayout.HelpBox("Chunk data is only available in play mode", MessageType.Info);
                return;
            }

            showChunkData = EditorGUILayout.Foldout(showChunkData, "Chunk Data", true, EditorStyles.foldoutHeader);
            if (!showChunkData) {
                return;
            }

            // Don't draw chunk data if there are too many chunks selected (performance)
            if (chunkRenderers.Length > 100) {
                EditorGUILayout.HelpBox("Too many chunks selected to display data", MessageType.Warning);
                return;
            }

            foreach (ChunkRenderer chunkRenderer in chunkRenderers) {
                DrawChunkDataGUI(chunkRenderer);
                GUILayout.Space(10);
            }
        }

        private static void DrawChunkDataGUI(ChunkRenderer chunkRenderer) {
            // Check if the chunk is null or has no chunk data
            if (chunkRenderer.ChunkData == null) {
                EditorGUILayout.HelpBox("Chunk data is null", MessageType.Error);
                return;
            }

            GUILayout.Label("World Position: " + chunkRenderer.ChunkData.WorldPosition);
            GUILayout.Label("Is Modified: " + chunkRenderer.IsModified);

            GUILayout.Label("Blocks: " + chunkRenderer.ChunkData.Blocks.Count(id => id != 0));

            GUILayout.Label("Vertices: " + chunkRenderer.Mesh.vertexCount);
            GUILayout.Label("Triangles: " + chunkRenderer.Mesh.triangles.Length);
            GUILayout.Label("UVs: " + chunkRenderer.Mesh.uv.Length);
        }
    }
}
