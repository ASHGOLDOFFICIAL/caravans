using CaravansCore.Entities.Components;
using CaravansCore.Level;
using CaravansCore.Terrain;
using CaravansCore.Utils;
using Godot;

namespace CaravansCore.Entities.Systems;

internal class TargetChoosingSystem(Layout level) : ISystem
{
    private readonly Random _random = new();
    private readonly HashSet<Type> _requiredComponentTypes = [typeof(Position), typeof(TargetTilePreference)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<TargetTile>(entity, out var target);
            if (target is not null) continue;

            em.TryGetComponent<Position>(entity, out var position);
            if (position is null) continue;

            em.TryGetComponent<TargetTilePreference>(entity, out var preference);
            if (preference is null) continue;

            TargetTile? targetTile = null;
            if (preference.Policy == TargetingPolicy.Random)
                TryFindRandomTarget(preference.Preferred, out targetTile);
            if (targetTile is null) continue;
            em.SetComponent(entity, targetTile);
            GD.Print($"Target chosen for {entity}: {targetTile}");
        }
    }

    private void TryFindRandomTarget(HashSet<TerrainId> preference, out TargetTile? target)
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