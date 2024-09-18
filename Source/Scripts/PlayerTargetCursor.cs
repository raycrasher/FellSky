using Godot;
using System;
using System.ComponentModel;

namespace FellSky.Scripts;

public partial class PlayerTargetCursor : Node3D
{
    private Node3D _cursor;

    [Export]
    public Camera3D Camera { get; set; }
    public PlayerController PlayerController { get; set; }

    public override void _Ready()
    {
        _cursor = (Node3D)GetNode("target_cursor");
        _cursor.Visible = true;
        _cursor.Position = new Vector3(0, 0, 0);
        base._Ready();
    }
    public override void _Process(double delta)
    {
        if (Camera == null) return;
        var mousePos = GetViewport().GetMousePosition();
        var from = Camera.ProjectRayOrigin(mousePos);
        var to = Camera.ProjectRayNormal(mousePos) * 100;
        var cursorPos = new Plane(Vector3.Up, Transform.Origin.Y).IntersectsRay(from, to);
        if (cursorPos != null)
        {
            this.GlobalPosition = cursorPos.Value;
        }

        if (PlayerController?.Ship != null && _cursor != null)
        {
            _cursor.LookAt(PlayerController.Ship.GlobalPosition);
        }

    }
}