using System.Collections.Immutable;
using CaravansCore.Utils;

namespace CaravansCore.Entities.Components;

public record Path(ImmutableQueue<Point2D> Tiles) : IComponent;