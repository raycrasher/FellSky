using System.Collections.Generic;
using Godot;

namespace FellSky.Sim;

public struct SimProjectile
{
    public bool IsActive;
    public Vector3 Position;
    public Quaternion Rotation;
}

public class ProjectileSimulator
{
    public List<SimProjectile> Projectiles = new();

    public void Update(float delta)
    {

    }
}