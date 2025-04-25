using System.Collections.Immutable;
using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
using CaravansCore.Level;
using CaravansCore.Level.Content;
using CaravansCore.Utils;
using Godot;

namespace CaravansCore.Entities.Systems;

internal class TargetTileSelectionSystem(Layout level) : ISystem
{
    private readonly Random _random = new();

    private readonly HashSet<Type> _requiredComponentTypes =
        [typeof(NeedsTargetTile), typeof(TargetTilePreference), typeof(Position)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<NeedsTargetTile>(entity, out var marker);
            if (marker is null) continue;
            em.TryGetComponent<TargetTile>(entity, out var target);
            if (target is not null)
            {
                em.RemoveComponent<NeedsTargetTile>(entity);
                continue;
            }
            
            em.TryGetComponent<TargetTilePreference>(entity, out var preference);
            if (preference is null) continue;
            em.TryGetComponent<Position>(entity, out var position);
            if (position is null) continue;

            TargetTile? targetTile = null;
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
        out TargetTile? target)
    {
        var startX = _random.Next(0, level.Width);
        var startY = _random.Next(0, level.Height);

        foreach (var tile in preference)
        foreach (var x in Enumerable.Range(startX, level.Width))
        foreach (var y in Enumerable.Range(startY, level.Height))
        {
            var point = new Point2D(x, y);
            if (level.GetTerrain(point) != tile) continue;
            target = new TargetTile(point);
            return;
        }

        target = null;
    }
}