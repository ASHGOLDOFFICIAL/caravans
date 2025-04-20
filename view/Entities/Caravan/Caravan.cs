using Godot;

namespace CaravansView.Entities.Caravan;

public partial class Caravan : CharacterBody2D, IEntityScene
{
    private const string Width = "width";
    private ShaderMaterial _outlineMaterial;

    public override void _Ready()
    {
        var sprite = GetNode<Sprite2D>("Sprite2D");
        _outlineMaterial = (ShaderMaterial)sprite.Material;

        InputPickable = true;
        MouseEntered += ShowOutline;
        MouseExited += HideOutline;
        HideOutline();
    }

    private void ShowOutline()
    {
        _outlineMaterial.SetShaderParameter(Width, 1f);
    }

    private void HideOutline()
    {
        _outlineMaterial.SetShaderParameter(Width, 0f);
    }
}