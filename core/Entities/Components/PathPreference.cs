using CaravansCore.Level.Content;

namespace CaravansCore.Entities.Components;

public record PathPreference(HashSet<TerrainId> Tiles) : IComponent;