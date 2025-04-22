using CaravansCore.Level.Content;

namespace CaravansCore.Entities.Components;

public record PreferredSpawnTerrain(HashSet<TerrainId> Tiles) : IComponent;