using Godot;
using System;
using System.ComponentModel;
using System.Linq;

namespace FellSky.Scripts;

public enum WeaponState
{
	Ready,
	WarmingUp,
	Firing,
	CoolingDown,
	Reloading,
	Disabled
}

public enum WeaponType
{
	Ballistic,
	Beam,
	Energy,
	Missile
}

public enum MuzzleFlashType
{
	Mesh, Particle
}

public partial class WeaponController : Node3D
{
	private static Logger logger = new Logger(typeof(WeaponController));
	private static Random rng = new Random();
	private Node3D[] _muzzles;

	public WeaponMount WeaponMount { get; set; }
	public ShipController Ship { get; set; }
	public Node3D TrackedTarget { get; set; }
	public bool FireWhenReady { get; set; }
	public double Cycle { get; set; } // between 0 and 1

	[Export]
	public float RotateSpeed { get; set; } = 20f;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	public float Cooldown { get; set; }
	[Export(PropertyHint.Range, "0.0,10,or_greater")]
	public float WarmUp { get; set; }

	[Export(PropertyHint.Range, "0.0,100000,or_greater")]
	public float MuzzleSpeed { get; set; }

	[Export(PropertyHint.Range, "350,2000,or_greater,or_less")]
	public float MaxEffectiveRange { get; set; }

	// How big the error circle is at max effective range, in meters
	[Export(PropertyHint.Range, "0,100,or_greater")]
	public float Spread { get; set; }

	[Export(PropertyHint.Range, "0,5,or_greater")]
	public float Duration { get; set; }

	[Export]
	public PackedScene Bullet { get; set; }
	[Export]
	public Vector3 ProjectileScale { get; set; }

	[Export]
	public PackedScene MuzzleFlash { get; set; }

	[Export]
	public Vector3 MuzzleFlashScale { get; set; }

	[Export]
	public MuzzleFlashType MuzzleFlashType { get; set; }
	[Export]
	public float MuzzleFlashDuration { get; set; } = 0.2f;

	public WeaponState State { get; set; }

	private Node3D _projectileSceneRoot;
	private WeaponState _lastState;
	private WeaponState _state;

	private Node3D[] _muzzleFlashes;
	private float[] _muzzleFlashTime;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_muzzles = GetChildren().OfType<Node3D>().Where(c => c.Name.ToString().StartsWith("muzzle")).OrderBy(c => c.Name).ToArray();
		if (_muzzles.Length == 0)
		{
			_muzzles = new Node3D[] { this };
		}
		if (!GetTree().Root.HasNode("projectiles"))
		{
			_projectileSceneRoot = new Node3D();
			_projectileSceneRoot.Name = "projectiles";
			Callable.From(() => GetTree().Root.AddChild(_projectileSceneRoot)).CallDeferred();
		}
		else
		{
			_projectileSceneRoot = GetTree().Root.GetNode("projectiles") as Node3D;
		}

		// create muzzle flash
		if (MuzzleFlash != null && MuzzleFlashType == MuzzleFlashType.Mesh)
		{
			_muzzleFlashes = new Node3D[_muzzles.Length];
			_muzzleFlashTime = new float[_muzzles.Length];
			for (int i = 0; i < _muzzles.Length; i++)
			{
				var flash = (Node3D)MuzzleFlash.Instantiate();
				_muzzleFlashes[i] = flash;
				_muzzles[i].AddChild(flash);
				flash.Scale = MuzzleFlashScale;
				flash.Visible = false;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Fire projectile. This is in PhysicsProcess since we need the velocities / positions to be accurate.
		if (State == WeaponState.Firing)
		{
			// spawn
			if (Bullet != null)
			{
				FireWeapon();
			}
			State = WeaponState.CoolingDown;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TrackTargetIfAvailable(delta);
		DoCycle(delta);
	}

	void DoCycle(double delta)
	{
		switch (State)
		{
			case WeaponState.Ready:
				if (FireWhenReady)
				{
					State = WeaponState.WarmingUp;
					goto case WeaponState.WarmingUp;
				}
				if (Cycle > 0.0f)
				{
					State = WeaponState.CoolingDown;
					goto case WeaponState.CoolingDown;
				}
				break;
			case WeaponState.WarmingUp:
				if (!FireWhenReady)
				{
					State = WeaponState.CoolingDown;
					goto case WeaponState.CoolingDown;
				}
				if (WarmUp <= 0.0f || Cycle >= 1.0f)
				{
					Cycle = 1.0f;
					State = WeaponState.Firing;
					// Note: Firing is done in the PhysicsProcess
				}
				else
				{
					Cycle = Mathf.Clamp(Cycle + delta * WarmUp, 0.0f, 1.0f);
				}
				break;
			case WeaponState.CoolingDown:
				if (Cooldown <= 0.0f || Cycle <= 0.0f)
				{
					Cycle = 0.0f;
					if (FireWhenReady)
					{
						State = WeaponState.WarmingUp;
						goto case WeaponState.WarmingUp;
					}
					else
					{
						State = WeaponState.Ready;
						goto case WeaponState.Ready;
					}
				}
				else
				{
					Cycle = Mathf.Clamp(Cycle - (1f / Cooldown * delta), 0.0f, 1.0f);
				}
				break;
			case WeaponState.Disabled:
				// TODO
				State = WeaponState.Ready;
				goto case WeaponState.Ready;
		}

		// muzzle flash
		if (MuzzleFlashDuration > 0)
		{
			for (int i = 0; i < _muzzleFlashTime.Length; i++)
			{
				if (_muzzleFlashTime[i] > 0)
				{
					_muzzleFlashes[i].Visible = true;
					_muzzleFlashTime[i] = Mathf.Clamp(_muzzleFlashTime[i] - (1 / MuzzleFlashDuration * (float)delta), 0, 1);
				}
				else
				{
					_muzzleFlashes[i].Visible = false;
				}
			}
		}
	}

	public void FireWeapon()
	{
		var projectile = Bullet.Instantiate();

		Vector3 fireDir;
		if (Spread <= 0)
		{
			fireDir = new Vector3(0, 0, 1);
		}
		else
		{
			var spreadOffsetUnit = rng.GetRandomVectorInCircle(Spread);
			fireDir = new Vector3(spreadOffsetUnit.X, spreadOffsetUnit.Y, MaxEffectiveRange).Normalized();
		}

		this._projectileSceneRoot.AddChild(projectile);
		int muzzleIndex = 0; // TODO

		if (projectile is Node3D pnode)
		{
			pnode.Scale = ProjectileScale;
			pnode.GlobalPosition = _muzzles[muzzleIndex].GlobalPosition;
			var facing = (_muzzles[muzzleIndex].GlobalTransform.Basis * fireDir).Normalized();
			pnode.LookAt(_muzzles[muzzleIndex].GlobalPosition + facing);

			if (projectile is RigidBody3D body)
			{
				body.LinearVelocity = Ship.LinearVelocity + MuzzleSpeed * facing;
			}
		}

		if (projectile is IProjectile p2)
		{
			p2.ProjectileScale = ProjectileScale;
			if (projectile is RigidBody3D && MuzzleSpeed > 0)
			{
				p2.TimeToLive = MaxEffectiveRange / MuzzleSpeed;
			}
			else
			{
				p2.TimeToLive = Duration;
			}
			p2.SourceShip = Ship;
			p2.SourceWeapon = this;
		}

		if (MuzzleFlashDuration > 0)
		{
			// show muzzle flash
			if (MuzzleFlashType == MuzzleFlashType.Mesh)
			{
				_muzzleFlashes[muzzleIndex].Visible = true;
				_muzzleFlashes[muzzleIndex].RotateZ(rng.Next());
				_muzzleFlashTime[muzzleIndex] = 1.0f;
			}
		}

	}

	void TrackTargetIfAvailable(double delta)
	{
		if (TrackedTarget == null || WeaponMount == null)
			return;
		var targetLocalPos2D = ToLocal(TrackedTarget.GlobalPosition).ToVector2();
		var dot = Vector2.Right.Dot(targetLocalPos2D.Normalized());
		var angle = targetLocalPos2D.Angle();
		var lastAngle = angle;

		if (dot != 0f)
		{
			angle += Mathf.DegToRad(RotateSpeed) * (float)delta * Mathf.Sign(dot);
		}

		RotateY(angle - lastAngle);

	}
}
