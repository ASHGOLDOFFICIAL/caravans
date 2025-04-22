using CaravansCore.Level.Content;

namespace CaravansCore.Entities.Components;

public enum TargetingPolicy
{
    Random
}

public record TargetTilePreference(TargetingPolicy Policy, HashSet<TerrainId> Preferred) : IComponent;