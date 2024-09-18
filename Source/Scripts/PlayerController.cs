using Godot;
using System;

namespace FellSky.Scripts;

public partial class PlayerController : Node3D
{
	private bool _targetingUpdated;
	private Logger _logger;
	private Camera3D _camera;
	private bool _firstRun;

	[Export]
	public PlayerTargetCursor TargetingCursor { get; set; }

	[Export]
	public WeaponGroup SelectedWeaponGroup { get; set; }

	[Export]
	public ShipController Ship { get; set; }
	// Called when the node enters the scene tree for the first time.

	[Export(PropertyHint.Range, "1,100,or_greater,or_less")]
	public float CameraMinZoom { get; set; }
	[Export(PropertyHint.Range, "1,100,or_greater,or_less")]
	public float CameraMaxZoom { get; set; }

	[Export]
	public float CameraZoomSpeed { get; set; } = 5;
	public override void _Ready()
	{
		_logger = new Logger(this);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_firstRun)
		{
			_firstRun = true;
			TargetingCursor.PlayerController = this;
		}

		_camera = _camera ?? (Camera3D)GetNode("camera");
		if (Ship != null)
		{
			UpdateTargetingCursor();
			Vector2 thrust = new Vector2(
				Input.GetAxis("ship_strafeleft", "ship_straferight"),
				Input.GetAxis("ship_decelerate", "ship_accelerate")
			);

			float rotate = Input.GetAxis("ship_turnright", "ship_turnleft");
			Ship.MoveInput = thrust.Rotated(-Ship.GlobalRotation.Y);
			Ship.TurnInput = rotate;
			Ship.IsCruiseMode = Input.IsActionPressed("ship_boost");

			// camera follows player ship
			this.GlobalPosition = Ship.GlobalPosition;
		}

		UpdateCamera(delta);
	}

	private void UpdateCamera(double delta)
	{
		if (_camera == null) return;
		bool zoomIn = Input.IsActionJustReleased("camera_zoomin");
		bool zoomOut = Input.IsActionJustReleased("camera_zoomout");

		var cameraZoom = zoomIn ? -1 : zoomOut ? 1 : 0;

		var lasty = _camera.Position.Y;
		var y = Mathf.Clamp(_camera.Position.Y + (cameraZoom * delta * CameraZoomSpeed), CameraMinZoom, CameraMaxZoom);
		_camera.Position = new Vector3(_camera.Position.X, (float)y, _camera.Position.Z);
		if (cameraZoom != 0)
		{
			_logger.debug($"camera zoom to {cameraZoom}, current {lasty}");
		}
		if (Ship != null)
			_camera.LookAt(Ship.GlobalPosition);
	}

	private void UpdateTargetingCursor()
	{
		//if (_targetingUpdated) return;
		//_targetingUpdated = true;
		var isFiring = Input.IsActionPressed("ship_fire_group1");
		foreach (var weapon in Ship.Weapons[SelectedWeaponGroup])
		{
			if (weapon.FireWhenReady != isFiring)
			{
				_logger.info($"firing: {weapon.FireWhenReady} -> {isFiring}");
			}
			weapon.FireWhenReady = isFiring;
			weapon.TrackedTarget = TargetingCursor;
		}
	}

}
