using System;
using Godot;

namespace FellSky.Scripts;

public interface IThrusterAPI
{
    ShipController Ship { get; set; }
}

public partial class ThrusterController : Node3D, IThrusterAPI
{
    [Export]
    AnimationPlayer animationPlayer;

    public ShipController Ship { get; set; }

    bool isThrusting;

    public override void _Process(double delta)
    {
        if (Ship == null)
            return;

        bool newThrustState = Ship.MoveInput != Vector2.Zero;

        if (isThrusting != newThrustState)
        {
            animationPlayer.Play("thruster", customSpeed: newThrustState ? 2 : -2, fromEnd: !newThrustState);
        }

        isThrusting = newThrustState;

        //Ship.TurnInput;
    }
}