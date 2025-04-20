using CaravansCore.Entities.Components;

namespace CaravansCore.Entities;

public class EntityManager
{
    private readonly Dictionary<Type, Dictionary<Guid, object>> _components = [];
    private readonly HashSet<Entity> _entities = [];

    internal Entity CreateEntity()
    {
        var entity = new Entity(Guid.NewGuid());
        _entities.Add(entity);
        return entity;
    }

    internal void RemoveEntity(Entity entity)
    {
        if (!_entities.Contains(entity))
            return;
        foreach (var (_, dict) in _components)
        {
            dict.Remove(entity.Uuid);
            _entities.Remove(entity);
        }
    }

    internal void TryGetComponent<T>(Entity entity, out T? component) where T : IComponent
    {
        if (!_entities.Contains(entity))
        {
            component = default;
            return;
        }

        _components.TryGetValue(typeof(T), out var entities);
        if (entities is null)
        {
            component = default;
            return;
        }

        entities.TryGetValue(entity.Uuid, out var value);
        if (value is null)
        {
            component = default;
            return;
        }

        component = (T)value;
    }

    internal void SetComponent<T>(Entity entity, T component) where T : IComponent
    {
        if (!_entities.Contains(entity))
            return;
        var type = typeof(T);
        _components.TryAdd(type, new Dictionary<Guid, object>());
        _components[typeof(T)][entity.Uuid] = component;
    }

    internal void RemoveComponent<T>(Entity entity) where T : IComponent
    {
        if (!_entities.Contains(entity))
            return;
        var type = typeof(T);
        _components[type].Remove(entity.Uuid);
    }

    internal IEnumerable<Entity> GetAllEntitiesWith<T>() where T : IComponent
    {
        _components.TryGetValue(typeof(T), out var entities);
        if (entities is null) yield break;
        foreach (var (entity, _) in entities) yield return new Entity(entity);
    }

    internal IEnumerable<Entity> GetAllEntitiesWith(HashSet<Type> types)
    {
        if (types.Count == 0) yield break;
        foreach (var entity in GetAllEntitiesWith(types.First()))
        {
            var hasAll = types
                .Skip(1)
                .Aggregate(true, (current, type) => current & HasComponent(type, entity));
            if (hasAll) yield return entity;
        }
    }

    private IEnumerable<Entity> GetAllEntitiesWith(Type type)
    {
        _components.TryGetValue(type, out var entities);
        if (entities is null) yield break;

        foreach (var (uuid, _) in entities)
            yield return new Entity(uuid);
    }

    private bool HasComponent(Type type, Entity entity)
    {
        return _components.TryGetValue(type, out var map) && map.ContainsKey(entity.Uuid);
    }
}