using System;
using System.Collections.Generic;
using CaravansCore.Level.Content;
using CaravansCore.Networking;
using CaravansCore.Utils;
using CaravansView.Entities;
using CaravansView.Entities.Player;
using CaravansView.Utils;
using Godot;

namespace CaravansView.Levels;

internal partial class Level : Node2D
{
    private const int TileSize = 16;

    private readonly Queue<Guid> _died = [];
    private readonly BidirectionalDictionary<Guid, Node2D> _entities = [];

    private readonly EntityProvider _entityProvider = new();

    private readonly object _layerSetupLock = new();
    private readonly Queue<WorldSnapshot> _worldSnapshots = [];
    private (bool, PlayerSnapshot) _lastPlayerSnapshot = (false, default);

    [Export] private TileMapLayer _objectLayer;

    [Export] private Player _player;
    private bool _playerBound;
    private Guid _playerGuid;
    private int _playerSpeed;
    [Export] private TileMapLayer _terrainLayer;
    private bool _terrainSet;

    public override void _Ready()
    {
        RemoveChild(_player);
    }

    public override void _Process(double delta)
    {
        while (_died.TryDequeue(out var uuid))
        {
            if (uuid == _playerGuid)
            {
                if (IsAncestorOf(_player))
                {
                    _playerBound = false;
                    RemoveChild(_player);
                }
                continue;
            }

            RemoveEntity(uuid);
        }

        if (_lastPlayerSnapshot.Item1 && !_playerBound)
            BindPlayer(_lastPlayerSnapshot.Item2);

        if (_worldSnapshots.TryDequeue(out var worldSnapshot))
            SetupLevel(worldSnapshot);
    }

    internal void SubmitPlayerSnapshot(PlayerSnapshot playerSnapshot)
    {
        _lastPlayerSnapshot = (true, playerSnapshot);
    }

    internal void SubmitWorldSnapshot(WorldSnapshot snapshot)
    {
        _worldSnapshots.Enqueue(snapshot);
    }

    internal void SubmitDied(Guid[] died)
    {
        foreach (var entity in died)
            _died.Enqueue(entity);
    }

    internal Guid GetEntityGuid(Node2D scene)
    {
        return _entities.Inverse.TryGetValue(scene, out var uuid) ? uuid : Guid.Empty;
    }

    internal void MovePlayer(Vector2 direction)
    {
        if (!_playerBound) return;
        _player.Move(direction * _playerSpeed * TileSize);
    }

    private void BindPlayer(PlayerSnapshot player)
    {
        var entity = player.AttachedEntity;
        _playerSpeed = entity.Speed ?? 1;
        _playerGuid = entity.Uuid;
        _entities[entity.Uuid] = _player;
        _player.SetPosition(GamePosition(Converter.ToGodotVector(entity.Position)));
        AddChild(_player);
        _playerBound = true;
    }

    private void SetupLevel(WorldSnapshot world)
    {
        SetupTerrainLayer(world);
        SetupObjectLayer(world);

        var entities = world.Entities;
        if (entities is null) return;
        foreach (var entity in entities)
        {
            _entities.TryGetValue(entity.Uuid, out var node);
            if (node is null)
            {
                AddEntity(entity);
                continue;
            }

            node.CallDeferred(Node2D.MethodName.SetPosition,
                GamePosition(Converter.ToGodotVector(entity.Position)));
        }
    }

    private void AddEntity(EntitySnapshot info)
    {
        var instance = _entityProvider.Provide(info.Id);
        if (instance is null) return;
        instance.Position = GamePosition(Converter.ToGodotVector(info.Position));
        if (_entities.TryAdd(info.Uuid, instance))
            CallDeferred(Node.MethodName.AddChild, instance);
    }

    private void RemoveEntity(Guid uuid)
    {
        _entities.TryGetValue(uuid, out var scene);
        if (scene is null) return;
        _entities.Remove(uuid);
        scene.QueueFree();
    }

    private void SetupTerrainLayer(WorldSnapshot world)
    {
        lock (_layerSetupLock)
        {
            if (_terrainSet)
                return;

            _terrainSet = true;
            var terrain = world.Terrain;
            if (terrain is null) return;

            for (var i = 0; i < world.Width * world.Height; ++i)
            {
                var x = i % world.Width;
                var y = i / world.Width;
                var terrainId = terrain[i];
                _terrainLayer.SetCell(new Vector2I(x, y), 1, TerrainAtlasPosition(terrainId));
            }
        }
    }

    private void SetupObjectLayer(WorldSnapshot world)
    {
        var objects = world.Objects;
        for (var i = 0; i < world.Width * world.Height; ++i)
        {
            var x = i % world.Width;
            var y = i / world.Width;
            var objectId = objects[i];
            var position = new Vector2I(x, y);
            if (objectId is not { } nonNullObjectId)
            {
                _objectLayer.EraseCell(position);
                continue;
            }

            _objectLayer.SetCell(position, 0, ObjectsAtlasPosition(nonNullObjectId));
        }
    }

    public static Point2D TileAt(Vector2 globalPosition)
    {
        var x = (int)globalPosition.X / TileSize;
        var y = (int)globalPosition.Y / TileSize;
        return new Point2D(x, y);
    }

    public static System.Numerics.Vector2 TilePosition(Vector2 gamePosition)
    {
        var x = gamePosition.X / TileSize - 0.5f;
        var y = gamePosition.Y / TileSize - 0.5f;
        return new System.Numerics.Vector2(x, y);
    }

    private static Vector2 GamePosition(Vector2 tilePosition)
    {
        var x = (tilePosition.X + 0.5f) * TileSize;
        var y = (tilePosition.Y + 0.5f) * TileSize;
        return new Vector2(x, y);
    }

    private static Vector2I TerrainAtlasPosition(TerrainId id)
    {
        return id switch
        {
            TerrainId.Grass => new Vector2I(1, 0),
            TerrainId.Path => new Vector2I(2, 0),
            TerrainId.Water => new Vector2I(3, 0),
            TerrainId.City => new Vector2I(4, 0),
            _ => Vector2I.Zero
        };
    }

    private static Vector2I ObjectsAtlasPosition(ObjectId id)
    {
        return id switch
        {
            ObjectId.StoneWall => new Vector2I(1, 0),
            _ => Vector2I.Zero
        };
    }
}