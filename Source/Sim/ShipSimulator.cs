using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace FellSky.Sim;

public struct SimShip
{
    public Vector3 Position;
    public Sector Sector;
}

public class ShipSimulator
{
    public List<SimShip> Ships = new();

    public void Update(float delta)
    {


    }

    void UpdateShip(ref SimShip ship)
    {

    }
}