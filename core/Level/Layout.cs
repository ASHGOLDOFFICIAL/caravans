using CaravansCore.Level.Content;
using CaravansCore.Utils;

namespace CaravansCore.Level;

internal class Layout(int width, int height)
{
    private readonly ObjectId?[] _objectsLayer = new ObjectId?[width * height];
    private readonly TerrainId[] _terrainLayer = new TerrainId[width * height];

    public int Width { get; } = width;
    public int Height { get; } = height;

    public TerrainId[] GetTerrainLayer()
    {
        return _terrainLayer;
    }

    public TerrainId? GetTerrain(Point2D position)
    {
        return InsideBoundaries(position)
            ? _terrainLayer[Point2DToIndex(position)]
            : null;
    }

    public ObjectId?[] GetObjectLayer()
    {
        return _objectsLayer;
    }

    public ObjectId? GetObject(Point2D position)
    {
        return InsideBoundaries(position)
            ? _objectsLayer[Point2DToIndex(position)]
            : null;
    }

    public void PlaceTerrain(TerrainId terrain, Point2D position)
    {
        if (!InsideBoundaries(position)) return;
        _terrainLayer[Point2DToIndex(position)] = terrain;
    }

    public void PlaceObject(ObjectId @object, Point2D position)
    {
        if (!CanPlaceObject(position)) return;
        _objectsLayer[Point2DToIndex(position)] = @object;
    }

    public void RemoveObject(Point2D position)
    {
        var index = Point2DToIndex(position);
        if (!InsideBoundaries(position) || _objectsLayer[index] is null) return;
        _objectsLayer[index] = null;
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