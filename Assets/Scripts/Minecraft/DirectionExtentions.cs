using System;
using UnityEngine;

namespace Minecraft {
    public static class DirectionExtentions {
        /// <summary>
        ///     Transforms the direction to a Vector3Int.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The corresponding Vector3Int.</returns>
        public static Vector3Int ToVector3Int(this Direction direction) {
            return direction switch {
                Direction.Right => Vector3Int.right,
                Direction.Left => Vector3Int.left,
                Direction.Down => Vector3Int.down,
                Direction.Up => Vector3Int.up,
                Direction.Forward => Vector3Int.forward,
                Direction.Back => Vector3Int.back,
                _ => throw new Exception("Invalid direction")
            };
        }
    }
}
