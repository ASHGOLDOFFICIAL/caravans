using CaravansCore.Entities.Components;

namespace CaravansCore.Utils;

public static class CollisionTools
{
    public static bool AabbOverlap(Position a, CollisionBox boxA, Position b, CollisionBox boxB)
    {
        var aOriginX = a.Value.X - boxA.Width / 2;
        var aOriginY = a.Value.Y - boxA.Height / 2;
        var bOriginX = b.Value.X - boxB.Width / 2;
        var bOriginY = b.Value.Y - boxB.Height / 2;

        var xInside = aOriginX < bOriginX + boxB.Width &&
                      aOriginX + boxA.Width > bOriginX;
        var yInside = aOriginY < bOriginY + boxB.Height &&
                      aOriginY + boxA.Height > bOriginY;
        return xInside && yInside;
    }
}