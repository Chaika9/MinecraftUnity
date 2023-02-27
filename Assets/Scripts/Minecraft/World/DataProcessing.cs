using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.World {
    public static class DataProcessing {
        private readonly static List<Vector2Int> Directions = new List<Vector2Int> {
            new Vector2Int(0, 1),   // North
            new Vector2Int(1, 1),   // North-East
            new Vector2Int(1, 0),   // East
            new Vector2Int(1, -1),  // South-East
            new Vector2Int(-1, 0),  // South
            new Vector2Int(-1, -1), // South-West
            new Vector2Int(0, -1),  // West
            new Vector2Int(1, -1)   // North-West
        };

        public static List<Vector2Int> FindLocalMaxima(float[,] dataMatrix) {
            var localMaxima = new List<Vector2Int>();
            for (int x = 0; x < dataMatrix.GetLength(0); x++) {
                for (int y = 0; y < dataMatrix.GetLength(1); y++) {
                    float noiseValue = dataMatrix[x, y];
                    if (CheckNeighbours(dataMatrix, x, y, neighbourValue => neighbourValue < noiseValue)) {
                        localMaxima.Add(new Vector2Int(x, y));
                    }
                }
            }
            return localMaxima;
        }

        private static bool CheckNeighbours(float[,] dataMatrix, int x, int z, Func<float, bool> successCondition) {
            foreach (Vector2Int direction in Directions) {
                var newPos = new Vector2Int(x + direction.x, z + direction.y);
                if (newPos.x < 0 || newPos.x >= dataMatrix.GetLength(0) || newPos.y < 0 || newPos.y >= dataMatrix.GetLength(1)) {
                    continue;
                }

                if (!successCondition(dataMatrix[newPos.x, newPos.y])) {
                    return false;
                }
            }
            return true;
        }
    }
}
