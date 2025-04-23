using CaravansCore;
using CaravansCore.Level.Content;
using CaravansView.Entities;
using CaravansView.Entities.Player;
using Godot;
using Level = CaravansView.Levels.Level;

namespace CaravansView;

internal partial class InputController : Node2D
{
    private const string MoveLeft = "move_left";
    private const string MoveRight = "move_right";
    private const string MoveDown = "move_down";
    private const string MoveUp = "move_up";
    private const string PrimeAction = "primary_action";
    private const string SecondAction = "secondary_action";

    private const string Collider = "collider";

    [Export] private Level _level;
    [Export] private Player _player;

    internal PlayerController Controller { get; set; }

    public override void _Process(double delta)
    {
        ProcessMovementInput();
        ProcessMouseInput();
    }

    private void ProcessMovementInput()
    {
        var inputDirection = Input.GetVector(
            MoveLeft, MoveRight,
            MoveUp, MoveDown);
        if (inputDirection == Vector2.Zero) return;
        _level.MovePlayer(inputDirection);
        // TODO: Send only after player was bound
        Controller?.MovePlayer(Level.TilePosition(_player.Position));
    }

    private void ProcessMouseInput()
    {
        var mousePosition = GetGlobalMousePosition();
        var selectedTile = Level.TileAt(mousePosition);

        if (Input.IsActionJustPressed(PrimeAction))
        {
            Controller.RemoveObject(selectedTile);
        }
        else if (Input.IsActionJustPressed(SecondAction))
        {
            var interacted = TryToInteract(GetGlobalMousePosition());
            if (interacted) return;
            Controller.PlaceObject(ObjectId.StoneWall, selectedTile);
        }
    }

    private bool TryToInteract(Vector2 position)
    {
        PhysicsPointQueryParameters2D query = new()
        {
            Position = position,
            CollideWithAreas = false,
            CollideWithBodies = true
        };

        var spaceState = GetWorld2D().DirectSpaceState;
        var result = spaceState.IntersectPoint(query);

        foreach (var collision in result)
        {
            var clicked = (Node2D)collision[Collider];
            if (clicked is not IEntityScene) continue;
            var entityGuid = _level.GetEntityGuid(clicked);
            Controller.Interact(entityGuid);
            return true;
        }

        return false;
    }
}