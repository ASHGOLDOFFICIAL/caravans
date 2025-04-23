using CaravansCore;
using CaravansCore.Networking;
using CaravansView.Levels;
using CaravansView.UI;
using Godot;

namespace CaravansView;

internal partial class Game : Node, IClient
{
    private readonly GameServer _server = new();

    [Export] private InputController _inputController;
    [Export] private Level _levelScene;
    [Export] private InGamePlayerUi _playerUi;

    public void Receive(Snapshot snapshot)
    {
        if (snapshot.Player is { } player)
        {
            _levelScene.SubmitPlayerSnapshot(player);
            _playerUi.Update(player);
        }

        if (snapshot.World is { } world)
            _levelScene.SubmitWorldSnapshot(world);
    }

    public override void _Ready()
    {
        var controller = _server.Connect(this);
        _inputController.Controller = controller;
    }
}