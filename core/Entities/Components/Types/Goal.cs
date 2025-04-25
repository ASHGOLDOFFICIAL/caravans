namespace CaravansCore.Entities.Components.Types;

public readonly record struct Goal(
    short Priority,
    GoalType Type
);