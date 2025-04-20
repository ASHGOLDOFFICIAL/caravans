using CaravansCore.Objects;
using CaravansCore.Terrain;
using CaravansCore.Utils;
using Godot;

namespace CaravansCore.Level;

internal class Layout(int width, int height)
{
    private readonly IObject?[] _objectsLayer = new IObject[width * height];
    private readonly TerrainId[] _terrainLayer = new TerrainId[width * height];

    public int Width { get; } = width;
    public int Height { get; } = height;

    public TerrainId[] GetTerrainLayer()
    {
        var terrain = new TerrainId[_terrainLayer.Length];
        _terrainLayer.CopyTo(terrain, 0);
        return terrain;
    }

    public TerrainId? GetTerrain(Point2D position)
    {
        if (!InsideBoundaries(position)) return null;
        return _terrainLayer[Point2DToIndex(position)];
    }

    public Dictionary<Point2D, ObjectId> GetObjects()
    {
        var objects = new Dictionary<Point2D, ObjectId>();
        for (var i = 0; i < _objectsLayer.Length; ++i)
        {
            var objectId = _objectsLayer[i];
            if (objectId == null) continue;
            objects.Add(IndexToPoint2D(i), objectId.Id);
        }

        return objects;
    }

    public IObject? GetObject(Point2D position)
    {
        return !InsideBoundaries(position) ? null : _objectsLayer[Point2DToIndex(position)];
    }

    public void PlaceTerrain(TerrainId terrain, Point2D position)
    {
        if (!InsideBoundaries(position)) return;
        _terrainLayer[Point2DToIndex(position)] = terrain;
    }

    public void PlaceObject(IObject @object, Point2D position)
    {
        if (!CanPlaceObject(position)) return;
        _objectsLayer[Point2DToIndex(position)] = @object;
        GD.Print("Place");
    }

    public void RemoveObject(Point2D position)
    {
        var index = Point2DToIndex(position);

        if (!InsideBoundaries(position) || _objectsLayer[index] is null) return;

        _objectsLayer[index] = null;
        GD.Print("Remove");
    }

    private bool CanPlaceObject(Point2D position)
    {
        return InsideBoundaries(position) && _objectsLayer[Point2DToIndex(position)] is null;
    }

    private bool InsideBoundaries(Point2D point)
    {
        return point.X >= 0 && point.X < Width && point.Y >= 0 && point.Y < Height;
    }

    private int Point2DToIndex(Point2D point)
    {
        return point.Y * Width + point.X;
    }

    private Point2D IndexToPoint2D(int index)
    {
        return new Point2D(index % Width, index / Width);
    }
}