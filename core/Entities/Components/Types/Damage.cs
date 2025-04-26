namespace CaravansCore.Entities.Components.Types;

internal record Damage(
    Entity From,
    int Amount,
    DamageType Type
);