using System.Collections.Immutable;
using CaravansCore.Level.Content;

namespace CaravansCore.Entities.Components;

public record PathPreference(ImmutableHashSet<TerrainId> Tiles) : IComponent;