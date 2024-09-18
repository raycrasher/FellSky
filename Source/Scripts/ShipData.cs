using Godot;
using System;
using System.ComponentModel;

namespace FellSky.Scripts;

public partial class ShipData : Resource
{
	[Export]
	public string HullName { get; set; }

	/// <summary>
	/// Maximum m/s for forward thrust
	/// </summary>
	[Export]
	public float CruiseSpeed { get; set; }

	/// <summary>
	/// Maximum m/s for combat / maneuvering
	/// </summary>
	[Export]
	public float ManeuverSpeed { get; set; }

	/// <summary>
	/// Maximum degrees/second when turning
	/// </summary>
	[Export]
	public float TurnSpeed { get; set; }

	/// <summary>
	/// How much uncapped speed is reachable in 1 second for forward thrust
	/// </summary>
	[Export]
	public float CruiseAccel { get; set; }

	/// <summary>
	/// How much uncapped speed is reachable in 1 second for side thrust
	/// </summary>
	[Export]
	public float ManeuverAccel { get; set; }

	/// <summary>
	/// How much uncapped angular speed is reachable in 1 second
	/// </summary>
	[Export]
	public float TurnAccel { get; set; }

	/// <summary>
	/// Health points
	/// </summary>
	[Export]
	public float Health { get; set; }

	/// <summary>
	/// Armor points
	/// </summary>
	[Export]
	public float Armor { get; set; }
}
