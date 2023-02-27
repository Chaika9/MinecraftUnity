using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.Block {
    // TODO: Rework this class
    [CreateAssetMenu(fileName = "Block List", menuName = "Minecraft/Block List")]
    public class BlockList : ScriptableObject {
        [SerializeField] private List<BlockData> blocks;

        /// <summary>
        ///     Gets the list of blocks.
        /// </summary>
        public List<BlockData> Blocks => blocks;
    }
}
