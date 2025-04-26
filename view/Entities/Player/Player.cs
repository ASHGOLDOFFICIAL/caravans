using System.Collections.Generic;
using CaravansView.Utils;
using Godot;

namespace CaravansView.Entities.Player;

internal partial class Player : CharacterBody2D, IEntityScene
{
    private readonly Dictionary<Direction4, string> _directionString = new()
    {
        [Direction4.Down] = "down",
        [Direction4.Left] = "left",
        [Direction4.Right] = "right",
        [Direction4.Up] = "up"
    };

    [Export] private AnimatedSprite2D _animation;

    private Direction4 _direction;

    public override void _Process(double delta)
    {
        ChooseAnimation();
        SetVelocity(Vector2.Zero);
    }

    public void Move(Vector2 velocity)
    {
        SetVelocity(velocity);
        MoveAndSlide();
        UpdateDirection(velocity);
    }

    private void ChooseAnimation()
    {
        if (GetVelocity() != Vector2.Zero)
            _animation.Play("walk_" + _directionString[_direction]);
        else
            _animation.Play("idle_" + _directionString[_direction]);
    }

    private void UpdateDirection(Vector2 velocity)
    {
        _direction = velocity.X switch
        {
            > 0 => Direction4.Right,
            < 0 => Direction4.Left,
            _ => velocity.Y switch
            {
                < 0 => Direction4.Up,
                > 0 => Direction4.Down,
                _ => _direction
            }
        };
    }
}