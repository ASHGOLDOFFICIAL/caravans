namespace CaravansCore.Entities.Components;

public record Score(int Value) : IComponent
{
    public Score Add(int delta) => new(Value + delta);
}