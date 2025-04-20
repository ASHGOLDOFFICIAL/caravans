using CaravansCore.Utils;

namespace CaravansCore.Entities.Components;

public record Path(Queue<Point2D> Value) : IComponent;