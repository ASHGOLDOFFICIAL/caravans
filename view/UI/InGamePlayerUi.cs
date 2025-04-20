using CaravansCore.Networking;
using Godot;

namespace CaravansView.UI;

public partial class InGamePlayerUi : CanvasLayer
{
    [Export] private Label _scoreLabel;
    [Export] private string _scorePrefix = "Score: ";

    internal void Update(PlayerSnapshot player)
    {
        _scoreLabel.CallDeferred(Label.MethodName.SetText, _scorePrefix + player.Score);
    }
}