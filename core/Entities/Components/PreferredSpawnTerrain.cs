using CaravansCore.Terrain;

namespace CaravansCore.Entities.Components;

public record PreferredSpawnTerrain(HashSet<TerrainId> Tiles) : IComponent;