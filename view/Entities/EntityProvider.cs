using System.Collections.Generic;
using CaravansCore.Entities.Components;
using Godot;

namespace CaravansView.Entities;

internal class EntityProvider
{
    private readonly Dictionary<EntityId, PackedScene> _entityIdToScene = [];

    internal EntityProvider()
    {
        _entityIdToScene.Add(EntityId.Player,
            GD.Load<PackedScene>("res://Entities/Player/Player.tscn"));
        _entityIdToScene.Add(EntityId.Caravan,
            GD.Load<PackedScene>("res://Entities/Caravan/Caravan.tscn"));
    }

    internal Node2D Provide(EntityId id)
    {
        _entityIdToScene.TryGetValue(id, out var scene);
        return (Node2D)scene?.Instantiate();
    }
}