using System.Collections.Immutable;
using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
using CaravansCore.Level;
using CaravansCore.Level.Content;
using CaravansCore.Utils;
using Godot;

namespace CaravansCore.Entities.Systems;

internal class TargetPositionSelectionSystem(Layout level) : ISystem
{
    private readonly Random _random = new();

    private readonly List<Type> _requiredComponentTypes =
        [typeof(NeedsTargetTile), typeof(TargetPositionPreference), typeof(Position)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, components) in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<TargetPosition>(entity, out var target);
            if (target is not null)
            {
                em.RemoveComponent<NeedsTargetTile>(entity);
                continue;
            }
            
            var preference = (TargetPositionPreference)components[typeof(TargetPositionPreference)];
            var position = (Position)components[typeof(Position)];

            TargetPosition? targetTile = null;
            if (preference.Policy == TargetingPolicy.Random)
                TryFindRandomTarget(preference.Preferred, out targetTile);
            
            if (targetTile is null) continue;
            
            em.SetComponent(entity, targetTile);
            em.RemoveComponent<NeedsTargetTile>(entity);
            GD.Print($"Target chosen for {entity}: {targetTile}");
        }
    }

    private void TryFindRandomTarget(
        ImmutableHashSet<TerrainId> preference,
        out TargetPosition? target)
    {
        var startX = _random.Next(0, level.Width);
        var startY = _random.Next(0, level.Height);

        foreach (var tile in preference)
        foreach (var x in Enumerable.Range(startX, level.Width))
        foreach (var y in Enumerable.Range(startY, level.Height))
        {
            var point = new Point2D(x, y);
            if (level.GetTerrain(point) != tile) continue;
            target = new TargetPosition(point.ToVector2());
            return;
        }

        target = null;
    }
}