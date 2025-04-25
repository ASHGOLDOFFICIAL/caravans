using System.Collections.Generic;
using CaravansCore.Entities.Components.Types;
using Godot;

namespace CaravansView.Entities;

internal class EntityProvider
{
    private readonly Dictionary<EntityId, PackedScene> _entityIdToScene = new()
    {
        [EntityId.Player] = GD.Load<PackedScene>("res://Entities/Player/Player.tscn"),
        [EntityId.Caravan] = GD.Load<PackedScene>("res://Entities/Caravan/Caravan.tscn"),
        [EntityId.Guardian] = GD.Load<PackedScene>("res://Entities/Guardian/Guardian.tscn"),
    };

    internal Node2D Provide(EntityId id)
    {
        _entityIdToScene.TryGetValue(id, out var scene);
        return (Node2D)scene?.Instantiate();
    }
}