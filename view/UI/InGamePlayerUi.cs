using CaravansCore.Networking;
using Godot;

namespace CaravansView.UI;

public partial class InGamePlayerUi : CanvasLayer
{
    [Export] private Label _scoreLabel;
    [Export] private string _scorePrefix = "Score: ";
    
    [Export] private Label _healthLabel;
    [Export] private string _healthPrefix = "Health: ";

    internal void Update(PlayerSnapshot player)
    {
        _scoreLabel.CallDeferred(Label.MethodName.SetText, _scorePrefix + player.Score);
        _healthLabel.CallDeferred(
            Label.MethodName.SetText,
            _healthPrefix + player.AttachedEntity.CurrentHealth + "/" + player.AttachedEntity.MaxHealth);
    }
}