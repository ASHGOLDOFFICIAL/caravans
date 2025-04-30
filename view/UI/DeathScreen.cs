using Godot;

namespace CaravansView.UI;

public partial class DeathScreen : Control
{
    [Export] private Label _scoreLabel;
    [Export] private string _scorePrefix = "Score: ";
    
    internal void Show(int score)
    {
        _scoreLabel.SetText(_scorePrefix + score);
        SetVisible(true);
    }
}