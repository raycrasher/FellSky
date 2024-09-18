using System;
using System.Collections.Generic;
using System.Threading;

namespace Godot;

public delegate void SimThreadAction(int threadIndex, float delta);

public partial class SimRunner : Node
{
    public override void _Ready()
    {

    }



}