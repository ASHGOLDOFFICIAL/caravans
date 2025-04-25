using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Components;

public record CurrentGoal(GoalType Goal) : IComponent;