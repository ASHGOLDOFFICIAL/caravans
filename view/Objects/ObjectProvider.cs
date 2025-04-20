using System.Collections.Generic;
using CaravansCore.Objects;
using Godot;

namespace CaravansView.Objects;

internal class ObjectProvider
{
    private readonly Dictionary<ObjectId, PackedScene> _objectIdToScene = [];

    internal ObjectProvider()
    {
        _objectIdToScene.Add(ObjectId.StoneWall,
            GD.Load<PackedScene>("res://Objects/StoneWall/StoneWall.tscn"));
    }

    internal Node2D Provide(ObjectId id)
    {
        _objectIdToScene.TryGetValue(id, out var scene);
        return (Node2D)scene?.Instantiate();
    }
}