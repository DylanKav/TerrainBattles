using System;
using TerrainBattlesCore.Core;

namespace TerrainBattlesCore.Net
{
    public enum PacketType
    {
        PlayerState,
        DebugMessage,
        TerrainManipulation,
        ChunkGeneration
    }
}