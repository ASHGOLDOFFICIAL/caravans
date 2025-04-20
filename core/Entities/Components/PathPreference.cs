using CaravansCore.Terrain;

namespace CaravansCore.Entities.Components;

public record PathPreference(HashSet<TerrainId> Tiles) : IComponent;