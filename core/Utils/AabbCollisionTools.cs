using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Utils;

public static class AabbCollisionTools
{
    public static Aabb PossibleCollisionArea(Aabb moving, Aabb desired)
    {
        var originX = Math.Min(moving.MinX, desired.MinX);
        var originY = Math.Min(moving.MinY, desired.MinY);
        var origin = new Vector2(originX, originY);

        var width = Math.Max(moving.MaxX, desired.MaxX) - originX;
        var height = Math.Max(moving.MaxY, desired.MaxY) - originY;
        var size = new Vector2(width, height);

        return new Aabb(origin, size);
    }

    public static IEnumerable<Point2D> PossibleCollisionTiles(Aabb box)
    {
        var xStart = (int)Math.Floor(box.MinX);
        var yStart = (int)Math.Floor(box.MinY);
        var xStop = (int)Math.Ceiling(box.MaxX);
        var yStop = (int)Math.Ceiling(box.MaxY);

        for (var x = xStart; x <= xStop; ++x)
        for (var y = yStart; y <= yStop; ++y)
            yield return new Point2D(x, y);
    }

    public static IEnumerable<Point2D> PossibleCollisionTiles(Aabb moving, Aabb desired)
    {
        var originX = Math.Min(moving.MinX, desired.MinX);
        var xStart = (int)Math.Floor(originX);

        var originY = Math.Min(moving.MinY, desired.MinY);
        var yStart = (int)Math.Floor(originY);

        var oppositeX = Math.Max(moving.MaxX, desired.MaxX);
        var xStop = (int)Math.Ceiling(oppositeX);

        var oppositeY = Math.Max(moving.MaxY, desired.MaxY);
        var yStop = (int)Math.Ceiling(oppositeY);

        for (var x = xStart; x <= xStop; ++x)
        for (var y = yStart; y <= yStop; ++y)
            yield return new Point2D(x, y);
    }

    public static bool AabbOverlapsAabb(Aabb a, Aabb b)
    {
        var xInside = a.MinX < b.MaxX && a.MaxX > b.MinX;
        var yInside = a.MinY < b.MaxY && a.MaxY > b.MinY;
        return xInside && yInside;
    }

    public static bool RayIntersectsAabb(
        Vector2 origin, Vector2 direction, Aabb rect,
        out Vector2 intersectionNormal,
        out float enterTime)
    {
        intersectionNormal = Vector2.Zero;
        enterTime = float.NaN;

        var tEnter = (rect.Origin - origin) / direction;
        var tExit = (rect.Origin + rect.Dimensions - origin) / direction;

        if (float.IsNaN(tEnter.X) || float.IsNaN(tEnter.Y)) return false;
        if (float.IsNaN(tExit.X) || float.IsNaN(tExit.Y)) return false;

        if (tEnter.X > tExit.X)
            (tEnter.X, tExit.X) = (tExit.X, tEnter.X);
        if (tEnter.Y > tExit.Y)
            (tEnter.Y, tExit.Y) = (tExit.Y, tEnter.Y);

        if (tEnter.X > tExit.Y || tEnter.Y > tExit.X)
            return false;

        enterTime = Math.Max(tEnter.X, tEnter.Y);
        var exitTime = Math.Min(tExit.X, tExit.Y);
        if (exitTime < 0) return false;

        if (tEnter.X > tEnter.Y)
            intersectionNormal = new Vector2(-Math.Sign(direction.X), 0);
        else if (tEnter.X < tEnter.Y)
            intersectionNormal = new Vector2(0, -Math.Sign(direction.Y));

        return true;
    }

    public static bool MovingAabbOverlapsStaticAabb(
        Aabb movingRect, Vector2 velocity,
        Aabb staticRect, out float hitTime)
    {
        return MovingAabbOverlapsStaticAabb(
            movingRect, velocity, staticRect, out _, out hitTime);
    }

    public static Vector2 MovingAabbOverlapResolution(Aabb moving, Vector2 velocity, Aabb staticAabb)
    {
        var collision = MovingAabbOverlapsStaticAabb(
            moving, velocity, staticAabb,
            out var intersectionNormal, out var hitTime);

        if (!collision) return velocity;

        var absVelocity = Vector2.Abs(velocity);
        var newVelocity = velocity + intersectionNormal * absVelocity * (1 - hitTime);
        return newVelocity;
    }

    private static bool MovingAabbOverlapsStaticAabb(
        Aabb movingRect, Vector2 velocity, Aabb staticRect,
        out Vector2 intersectionNormal,
        out float hitTime)
    {
        intersectionNormal = Vector2.Zero;
        hitTime = float.PositiveInfinity;
        if (velocity == Vector2.Zero)
            return false;

        var origin = staticRect.Origin - movingRect.Dimensions / 2;
        var dimensions = staticRect.Dimensions + movingRect.Dimensions;
        var expandedTarget = new Aabb(origin, dimensions);

        var centerRayIntersects = RayIntersectsAabb(
            movingRect.Center, velocity, expandedTarget,
            out intersectionNormal, out hitTime);

        if (centerRayIntersects)
            return hitTime is >= 0f and <= 1.0f;
        return false;
    }
}