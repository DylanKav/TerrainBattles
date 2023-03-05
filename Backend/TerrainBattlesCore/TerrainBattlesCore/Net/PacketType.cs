using System;
using TerrainBattlesCore.Core;

namespace TerrainBattlesCore.Net
{
    public enum PacketType
    {
        PlayerState,
        PlayerPosition,
        DebugMessage,
        TerrainManipulation,
        ChunkGeneration
    }
}