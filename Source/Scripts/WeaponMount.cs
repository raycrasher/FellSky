using System;
using Godot;

namespace FellSky.Scripts;

[Flags]
public enum WeaponMountFlags
{
    // +50% to energy weapon damage
    Powered = 1 << 0,
    // +50% to reload rate for weapons that have reloads
    AmmoFeed = 1 << 1,
    // -50% recoil, weapon doesn't turn
    Hardpoint = 1 << 2,
    // -25% heat generation, +30% damage to weapon when hit
    Vented = 1 << 3,
    // -50% fire rate, -90% damage to weapon when hit
    Internal = 1 << 4,
    // -50% damage to weapon when hit, -50% turn rate
    Armored = 1 << 5,
}

public enum WeaponGroup
{
    Group1, Group2, Group3, Group4, Group5, Group6, Group7, Group8
}

public partial class WeaponMount : Node3D
{
    [Export(PropertyHint.Flags)]
    public WeaponMountFlags Flags { get; set; }

    [Export]
    public float FireArc { get; set; }

    [Export]
    public WeaponGroup Group { get; set; }
}