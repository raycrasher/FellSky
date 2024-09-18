using FellSky.World;
using Godot;

namespace FellSky.Scripts;

public interface IProjectile
{
    WeaponController SourceWeapon { get; set; }
    ShipController SourceShip { get; set; }

    float TimeToLive { get; set; }
    Vector3 ProjectileScale { get; set; }
}

public partial class Bullet : RigidBody3D, IProjectile
{
    private MeshInstance3D _model;

    public float TimeToLive { get; set; }
    public Vector3 MuzzleVelocity { get; set; }
    public WeaponController SourceWeapon { get; set; }
    public ShipController SourceShip { get; set; }
    public Vector3 ProjectileScale { get; set; }

    public float Age { get; set; }

    private bool _firstFrame = false;

    public override void _Ready()
    {
        //var collider = GetNode<CollisionShape3D>("collider");
        _model = GetNode<MeshInstance3D>("model");
        //((SphereShape3D)collider.Shape).Radius *= ProjectileScale.X;
        base._Ready();
    }
    public override void _Process(double delta)
    {
        if (!_firstFrame)
        {
            _model.Scale = ProjectileScale;
            _firstFrame = true;
        }
        //_model.LookAt(_model.GlobalPosition + LinearVelocity, null, true);
        Age += (float)delta;
        if (Age > TimeToLive)
        {
            QueueFree();
        }
        base._Process(delta);
    }
}