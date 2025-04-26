using CaravansCore.Entities;
using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
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

    public void Attack()
    {
        _level.EntityManager.SetComponent(_controlledPlayer, new AttackIntent());
    }

    public void MovePlayer(Vector2 direction)
    {
        _level.EntityManager.SetComponent(_controlledPlayer, new PlayerSubmittedPosition(direction));
        _level.EntityManager.SetComponent(_controlledPlayer, new Rotation(direction));
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