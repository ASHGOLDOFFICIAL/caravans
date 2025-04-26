namespace CaravansCore.Entities.Components;

public record Cooldown(float Total) : IComponent
{
    private float _remains;

    public bool IsEnded()
    {
        return _remains == 0;
    }

    public Cooldown Reset()
    {
        _remains = Total;
        return this;
    }

    public Cooldown Update(float deltaTime)
    {
        var newValue = _remains - deltaTime;
        _remains = newValue < 0 ? 0 : newValue;
        return this;
    }
}