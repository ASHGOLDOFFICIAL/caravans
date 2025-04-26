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

    internal bool TryGetComponent<T>(Entity entity, out T? component) where T : IComponent
    {
        if (!_entities.Contains(entity))
        {
            component = default;
            return false;
        }

        _components.TryGetValue(typeof(T), out var entities);
        if (entities is null)
        {
            component = default;
            return false;
        }

        entities.TryGetValue(entity.Uuid, out var value);
        if (value is null)
        {
            component = default;
            return false;
        }

        component = (T)value;
        return true;
    }

    internal T GetComponentOrSet<T>(Entity entity, T ifAbsent) where T : IComponent
    {
        TryGetComponent<T>(entity, out var component);
        if (component is not null) return component;
        
        SetComponent(entity, ifAbsent);
        return ifAbsent;
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
        _components.TryGetValue(type, out var forType);
        forType?.Remove(entity.Uuid);
    }

    internal IEnumerable<(Entity, T)> GetAllEntitiesWith<T>() where T : IComponent
    {
        _components.TryGetValue(typeof(T), out var entities);
        if (entities is null)
            yield break;

        foreach (var (entity, component) in entities)
            yield return (new Entity(entity), (T)component);
    }

    internal IEnumerable<(Entity, Dictionary<Type, object>)> GetAllEntitiesWith(List<Type> types)
    {
        if (types.Count == 0)
            yield break;

        foreach (var (entity, firstComponent) in GetAllEntitiesWith(types.First()))
        {
            var hasAll = true;
            var components = new Dictionary<Type, object>
            {
                [types.First()] = firstComponent
            };

            foreach (var type in types.Skip(1))
            {
                var component = GetComponent(type, entity);
                if (component is null)
                {
                    hasAll = false;
                    break;
                }

                components[type] = component;
            }

            if (hasAll) yield return (entity, components);
        }
    }


    private IEnumerable<(Entity, object)> GetAllEntitiesWith(Type type)
    {
        _components.TryGetValue(type, out var entities);
        if (entities is null) yield break;

        foreach (var (uuid, component) in entities)
            yield return (new Entity(uuid), component);
    }

    private object? GetComponent(Type type, Entity entity)
    {
        _components.TryGetValue(type, out var componentForType);
        if (componentForType is null) return null;
        componentForType.TryGetValue(entity.Uuid, out var component);
        return component;
    }

    private bool HasComponent(Type type, Entity entity)
    {
        return _components.TryGetValue(type, out var map) && map.ContainsKey(entity.Uuid);
    }
}