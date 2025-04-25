using CaravansCore.Entities.Components;
using CaravansCore.Level;
using CaravansCore.Utils;
using Aabb = CaravansCore.Utils.Aabb;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Entities.Systems;

internal class VisualSensingSystem(Layout level) : ISystem
{
    private readonly HashSet<Type> _requiredComponentTypes =
        [typeof(FieldOfView), typeof(Position), typeof(Rotation)];

    private readonly HashSet<Type> _requiredComponentTypesToBeSeen =
        [typeof(CollisionBox), typeof(Position)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<FieldOfView>(entity, out var vision);
            if (vision is null) continue;
            em.TryGetComponent<Position>(entity, out var position);
            if (position is null) continue;
            em.TryGetComponent<Rotation>(entity, out var rotation);
            if (rotation is null) continue;
        
            var visionAabb = PossibleCollisionArea(vision, position.Coordinates); 
            var seenObjects = SeenObjects(vision, rotation.Direction, visionAabb);
            var seenEntities = new List<Entity>();

            var otherEntities = em
                .GetAllEntitiesWith(_requiredComponentTypesToBeSeen)
                .Where(e => e != entity);
            foreach (var second in otherEntities)
            {
                em.TryGetComponent<CollisionBox>(second, out var secondBox);
                if (secondBox is null) continue;
                em.TryGetComponent<Position>(second, out var positionB);
                if (positionB is null) continue;

                var bAabb = new Aabb(positionB.Coordinates, secondBox);
                if (!AabbCollisionTools.AabbOverlapsAabb(visionAabb, bAabb))
                    continue;
                
                if (IsOutOfFov(vision, visionAabb.Center, rotation.Direction, bAabb))
                    continue;
                if (IsObstructed(seenObjects,visionAabb.Center, bAabb.Center))
                    continue;

                seenEntities.Add(second);
            }

            if (seenEntities.Count <= 0 && seenObjects.Count <= 0)
                continue;
            em.SetComponent(entity, new LookAroundResult(seenEntities, seenObjects));
        }
    }

    private List<Point2D> SeenObjects(FieldOfView fieldOfView, Vector2 direction, Aabb collisionArea)
    {
        var seenObjects = new List<Point2D>();
        
        var objects = AabbCollisionTools
            .PossibleCollisionTiles(collisionArea)
            .Where(p => level.GetObject(p) != null)
            .ToList();

        foreach (var point in objects)
        {
            var objectPos = point.ToVector2();
            var bAabb = new Aabb(objectPos, new CollisionBox(1, 1));

            if (IsOutOfFov(fieldOfView, collisionArea.Center, direction, bAabb))
                continue;
                
            if (IsObstructed(objects.Where(o => o != point), 
                    collisionArea.Center, objectPos))
                continue;
            
            seenObjects.Add(point);
        }
        
        return seenObjects;
    }

    private Aabb PossibleCollisionArea(FieldOfView fieldOfView, Vector2 origin)
    {
        var diameter = fieldOfView.Radius + fieldOfView.Radius;
        var box = new CollisionBox(diameter, diameter);
        return new Aabb(origin, box);
    }

    private static bool IsOutOfFov(FieldOfView fieldOfView, Vector2 origin, Vector2 direction, Aabb other)
    {
        if (IsOutOfRadius(origin, fieldOfView.Radius, other.Center))
            return true;
        
        var lineOfSight = other.Center - origin;
        var cos = Vector2
            .Dot(Vector2.Normalize(direction),Vector2.Normalize(lineOfSight));
        
        var angleTo = Math.Acos(cos) * 180 / Math.PI;
        return angleTo > fieldOfView.Angle;
    }
    
    private static bool IsOutOfRadius(Vector2 origin, float radius, Vector2 otherPosition)
    {
        var distanceSquared = Vector2
            .DistanceSquared(origin, otherPosition);
        return distanceSquared > radius * radius;
    }

    private static bool IsObstructed(IEnumerable<Point2D> obstacles, Vector2 visionOrigin, Vector2 otherPosition)
    {
        var lineOfSight = otherPosition - visionOrigin;
        var isObstructed = false;

        foreach (var other in obstacles)
        {
            var objectAabb = new Aabb(other.ToVector2(), new CollisionBox(1, 1));
            var onTheWay = AabbCollisionTools
                .RayIntersectsAabb(visionOrigin, lineOfSight,
                    objectAabb, out _, out var hitTime);
            isObstructed |= onTheWay && hitTime is >= 0 or <= 1;
            if (isObstructed) break;
        }

        return isObstructed;
    }
}