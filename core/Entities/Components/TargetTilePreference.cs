using CaravansCore.Entities.Components.Types;
using CaravansCore.Level.Content;

namespace CaravansCore.Entities.Components;

public record TargetTilePreference(
    TargetingPolicy Policy,
    HashSet<TerrainId> Preferred
) : IComponent;