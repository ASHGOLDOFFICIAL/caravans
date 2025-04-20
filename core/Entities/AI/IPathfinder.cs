using CaravansCore.Level;
using CaravansCore.Utils;

namespace CaravansCore.Entities.AI;

internal interface IPathfinder
{
    public IEnumerable<Point2D> FindPath(Layout level, Point2D start, Point2D end);
}