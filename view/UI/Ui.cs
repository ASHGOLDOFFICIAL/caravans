using CaravansCore.Networking;
using Godot;

namespace CaravansView.UI;

public partial class Ui : CanvasLayer
{
    [Export] private DeathScreen _deathScreen;

    [Export] private Label _healthLabel;
    [Export] private string _healthPrefix = "Health: ";
    [Export] private Label _scoreLabel;
    [Export] private string _scorePrefix = "Score: ";

    internal void Update(PlayerSnapshot player)
    {
        _scoreLabel.CallDeferred(Label.MethodName.SetText, _scorePrefix + player.Score);
        _healthLabel.CallDeferred(
            Label.MethodName.SetText,
            _healthPrefix + player.AttachedEntity.CurrentHealth + "/" + player.AttachedEntity.MaxHealth);
        if (player.IsDead)
            Died(player);
    }

    private void Died(PlayerSnapshot player)
    {
        _deathScreen.CallDeferred(DeathScreen.MethodName.Show, player.Score);
    }
}