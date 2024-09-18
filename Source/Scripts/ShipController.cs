using FellSky.World;
using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FellSky.Scripts;

public partial class ShipController : RigidBody3D
{
	public static float MinThrusterLengthScale = 0.1f;
	public static float MaxThrusterLengthScale = 1.0f;

	public Dictionary<WeaponGroup, List<WeaponController>> Weapons { get; private set; } = new();

	[Export]
	public ShipData ShipData { get; set; }
	public float TurnInput { get; set; }

	/// <summary>
	/// World-space move input.
	/// </summary>
	public Vector2 MoveInput { get; set; }
	public IThrusterAPI[] Thrusters { get; private set; }
	public bool IsCruiseMode { get; set; }

	private float thrusterVisualScale = 1.0f;
	private float thrusterVisualSpeed = 1.0f; // how fast the thruster visual goes from 0 -> 1 and vice versa 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadWeapons();
		LoadThrusters();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (ShipData == null)
			return;


		Vector2 thrustForce = Vector2.Zero;
		if (MoveInput != Vector2.Zero)
		{
			thrustForce = GetThrustForce(ShipData, MoveInput, IsCruiseMode) * this.Mass;
			this.ApplyForce(thrustForce.ToVector3());
		}
		if (TurnInput != 0f)
		{
			this.ApplyTorque(new Vector3(0, Mathf.DegToRad(ShipData.TurnAccel) * this.Mass * Mathf.Clamp(TurnInput, -1, 1), 0));
		}

		var currentSpeed = this.LinearVelocity.Length();
		if (currentSpeed > ShipData.ManeuverSpeed)
			this.LinearVelocity = (this.LinearVelocity / currentSpeed) * ShipData.ManeuverSpeed;
		var currentTurnSpeed = this.AngularVelocity.Y;
		if (currentTurnSpeed > Mathf.Abs(Mathf.DegToRad(ShipData.TurnSpeed)))
		{
			this.AngularVelocity = new Vector3(0, Mathf.DegToRad(ShipData.TurnSpeed) * Mathf.Sign(currentTurnSpeed), 0);
		}
	}

	private void LoadWeapons()
	{
		foreach (var val in Enum.GetValues<WeaponGroup>())
		{
			Weapons[val] = new List<WeaponController>();
		}
		foreach (var mount in GetChildren().OfType<WeaponMount>())
		{
			var weapon = mount.GetChildren().OfType<WeaponController>().FirstOrDefault();
			if (weapon != null)
			{
				Weapons[mount.Group].Add(weapon);
				weapon.WeaponMount = mount;
				weapon.Ship = this;
			}
		}
	}

	private void LoadThrusters()
	{
		Thrusters = GetChildren().OfType<IThrusterAPI>().ToArray();
		foreach (var thruster in Thrusters)
		{
			thruster.Ship = this;
		}
	}

	static private Vector2 GetThrustForce(ShipData data, Vector2 inputs, bool isCruise)
	{
		float throttle = MathF.Min(inputs.Length(), data.ManeuverAccel);
		if (isCruise)
		{
			throttle = data.CruiseAccel;
		}
		return inputs * throttle;
	}
}
