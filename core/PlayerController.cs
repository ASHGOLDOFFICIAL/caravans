using CaravansCore.Entities;
using CaravansCore.Entities.Components;
using CaravansCore.Level;
using CaravansCore.Level.Content;
using CaravansCore.Utils;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore;

public class PlayerController
{
    private readonly Entity _controlledPlayer;
    private readonly World _level;

    internal PlayerController(Entity player, World level)
    {
        _level = level;
        _controlledPlayer = player;
    }

    public void MovePlayer(Vector2 to)
    {
        _level.EntityManager.SetComponent(_controlledPlayer, new Position(to));
    }

    public void RemoveObject(Point2D position)
    {
        _level.Layout.RemoveObject(position);
    }

    public void PlaceObject(ObjectId @object, Point2D position)
    {
        _level.Layout.PlaceObject(@object, position);
    }

    public void Interact(Guid entityUuid)
    {
        var request = new InteractionRequest(new Entity(entityUuid), InteractionType.Default);
        _level.EntityManager.SetComponent(_controlledPlayer, request);
    }
}