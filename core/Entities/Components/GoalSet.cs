using System.Collections.Immutable;
using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Components;


public record GoalSet : IComponent
{
    public ImmutableList<Goal> Goals { get; }

    public GoalSet(IList<Goal> goals)
    {
        Goals = goals.OrderBy(goal => goal.Priority).ToImmutableList();
    }
}

