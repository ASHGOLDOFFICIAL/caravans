using System;
using System.Collections.Generic;
using CaravansCore.Networking;
using CaravansCore.Objects;
using CaravansCore.Terrain;
using CaravansCore.Utils;
using CaravansView.Entities;
using CaravansView.Entities.Player;
using CaravansView.Objects;
using CaravansView.Utils;
using Godot;

namespace CaravansView.Levels;

internal partial class Level : Node2D
{
    private const int TileSize = 16;
    private readonly object _bindingLock = new();
    private readonly Dictionary<Guid, Node2D> _entities = [];

    private readonly EntityProvider _entityProvider = new();
    private readonly ObjectProvider _objectProvider = new();
    private readonly Dictionary<Point2D, Node2D> _objects = [];

    private readonly Queue<PlayerSnapshot> _playerSnapshots = [];

    private readonly object _terrainLock = new();
    private readonly Dictionary<Node2D, Guid> _uuids = [];
    private readonly Queue<WorldSnapshot> _worldSnapshots = [];

    [Export] private Player _player;
    private Guid _playerGuid;
    private int _playerSpeed;
    private bool _terrainSet;
    [Export] private TileMapLayer _tileMap;

    public override void _Process(double delta)
    {
        if (_playerSnapshots.TryDequeue(out var playerSnapshot))
            lock (_bindingLock)
            {
                if (_playerGuid == Guid.Empty)
                    BindPlayer(playerSnapshot);
            }

        if (_worldSnapshots.TryDequeue(out var worldSnapshot))
            SetupLevel(worldSnapshot);
    }

    internal void SubmitPlayerSnapshot(PlayerSnapshot playerSnapshot)
    {
        _playerSnapshots.Enqueue(playerSnapshot);
    }

    internal void SubmitWorldSnapshot(WorldSnapshot snapshot)
    {
        _worldSnapshots.Enqueue(snapshot);
    }

    internal Guid GetEntityGuid(Node2D scene)
    {
        return _uuids[scene];
    }

    internal void MovePlayer(Vector2 direction)
    {
        _player.Move(direction * _playerSpeed * TileSize);
    }

    private void BindPlayer(PlayerSnapshot player)
    {
        var entity = player.AttachedEntity;
        _playerSpeed = entity.Speed ?? 1;
        _playerGuid = entity.Uuid;
        _entities[entity.Uuid] = _player;
        _uuids[_player] = entity.Uuid;
        _player.CallDeferred(Node2D.MethodName.SetPosition,
            GamePosition(Converter.ToGodotVector(entity.Position)));
    }

    private void SetupLevel(WorldSnapshot world)
    {
        SetupTerrain(world);

        var objects = world.Objects;
        if (objects is not null)
            foreach (var (pos, id) in objects)
                AddObject(id, pos);

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
        _entities.Add(info.Uuid, instance);
        if (_uuids.TryAdd(instance, info.Uuid))
            CallDeferred(Node.MethodName.AddChild, instance);
    }

    private void AddObject(ObjectId @object, Point2D point)
    {
        var instance = _objectProvider.Provide(@object);
        if (instance is null) return;
        instance.Position = GamePosition(Converter.ToGodotVector(point));
        if (_objects.TryAdd(point, instance))
            CallDeferred(Node.MethodName.AddChild, instance);
    }

    private void RemoveObject(Point2D point)
    {
        _objects.TryGetValue(point, out var node);
        _objects.Remove(point);
        node?.QueueFree();
    }

    private void SetupTerrain(WorldSnapshot world)
    {
        lock (_terrainLock)
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
                _tileMap.SetCell(new Vector2I(x, y), 1, TileAtlasPosition(terrainId));
            }
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

    private static Vector2I TileAtlasPosition(TerrainId id)
    {
        return id switch
        {
            TerrainId.Grass => new Vector2I(1, 0),
            TerrainId.Path => new Vector2I(2, 0),
            TerrainId.Water => new Vector2I(3, 0),
            _ => Vector2I.Zero
        };
    }
}