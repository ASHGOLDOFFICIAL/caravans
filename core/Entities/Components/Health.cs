namespace CaravansCore.Entities.Components;

public record Health(int Max) : IComponent
{
    public int Current { get; private set; } = Max;

    public Health Damage(int amount)
    {
        Current -= amount;
        Current = Current < 0 ? 0 : Current;
        return this;
    }

    public Health Full()
    {
        Current = Max;
        return this;
    }

    public bool IsAlive()
    {
        return Current > 0;
    }
}