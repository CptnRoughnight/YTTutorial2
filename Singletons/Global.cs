using Godot;
using System;

public class Global : Node
{
   public void setGlobalPosition(ref Spatial one,Vector3 newPosition )
    {
        Transform t = one.GlobalTransform;
        t.origin = newPosition;
        one.GlobalTransform = t;
    }
}
