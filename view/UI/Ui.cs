using System.Collections.Generic;
using CaravansCore.Networking;
using Godot;

namespace CaravansView.UI;

public partial class Ui : CanvasLayer
{
    [Export] private Label _deathScoreLabel;
    [Export] private Control _deathScreen;
    [Export] private Label _healthLabel;
    [Export] private string _healthPrefix = "Health: ";
    [Export] private Container _inGame;
    [Export] private Label _inGameScoreLabel;
    [Export] private InputController _inputController;
    [Export] private string _scorePrefix = "Score: ";
    [Export] private Button _restartButton;

    private readonly Queue<PlayerSnapshot> _snapshots = [];

    public override void _Ready()
    {
        _inGame.SetVisible(true);
        _deathScreen.SetVisible(false);
        _restartButton.Pressed += _inputController.RequestRespawn;
    }

    public override void _Process(double delta)
    {
        if (!_snapshots.TryDequeue(out var player)) return;

        _inGameScoreLabel.SetText(_scorePrefix + player.Score);
        _healthLabel.SetText(
            _healthPrefix + player.AttachedEntity.CurrentHealth + "/" + player.AttachedEntity.MaxHealth);

        if (player.IsDead)
            Died(player);
        else Alive();
    }

    internal void SubmitPlayerSnapshot(PlayerSnapshot player)
    {
        _snapshots.Enqueue(player);
    }

    private void Died(PlayerSnapshot player)
    {
        _inputController.Disable();
        _inGame.SetVisible(false);
        _deathScoreLabel.SetText(_scorePrefix + player.Score);
        _deathScreen.SetVisible(true);
    }

    private void Alive()
    {
        _deathScreen.SetVisible(false);
        _inGame.SetVisible(true);
        _inputController.Enable();
    }
}