using System.Collections.Immutable;
using CaravansCore.Level.Content;

namespace CaravansCore.Entities.Components;

public record PreferredSpawnTerrain(ImmutableHashSet<TerrainId> Tiles) : IComponent;