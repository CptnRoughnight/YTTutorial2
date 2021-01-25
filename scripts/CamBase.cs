using Godot;
using System;

public class CamBase : Spatial
{
    private Global globals;

    [Export]
    private float move_speed = 80.0f;
    [Export]
    private NodePath gridMapPath = "";
    [Export]
    private NodePath turmContainerPath = "";

    private Camera camera;
    private GridMap gridMap;

    private SpatialMaterial ballMat;

    private MeshInstance ball;
    private Spatial turmContainer;

    private PackedScene turm1Scene;

    private Vector2 mousepos;

    private Vector3 illegal = new Vector3(-500,-500,-500);


    public override void _Ready()
    {
        globals = GetNode<Global>("/root/Global");

        Input.SetMouseMode(Input.MouseMode.Hidden);
        camera = GetNode<Camera>("Camera");
        ball = GetNode<MeshInstance>("ball");
        ballMat = ResourceLoader.Load<SpatialMaterial>("res://ball.tres");
        turm1Scene = ResourceLoader.Load<PackedScene>("res://Scenes/tuerme/Turm1.tscn");
     
        if (!String.IsNullOrEmpty(turmContainerPath))
            turmContainer = GetNode<Spatial>(turmContainerPath);

        if (!String.IsNullOrEmpty(gridMapPath))
            gridMap = GetNode<GridMap>(gridMapPath);
    }

    public override void _Process(float delta)
    {
        mousepos = GetViewport().GetMousePosition();

        if (Input.IsActionPressed("left"))
        {
            Translate(new Vector3(-1, 0, 0) * move_speed * delta);
        }

        if (Input.IsActionPressed("right"))
        {
            Translate(new Vector3(1, 0, 0) * move_speed * delta);
        }

        if (Input.IsActionPressed("forward"))
        {
            Translate(new Vector3(0, 0, -1) * move_speed * delta);
        }

        if (Input.IsActionPressed("backward"))
        {
            Translate(new Vector3(0, 0, 1) * move_speed * delta);
        }

        if (Input.IsActionJustPressed("linksklick"))
        {
            Vector3 ret = updateMouse(mousepos);
            if (ret!=illegal)
            {
                gridMap.SetCellItem((int)ret.x,(int)ret.y,(int)ret.z,0);
                Spatial turm = (Spatial)turm1Scene.Instance();
                
                turmContainer.AddChild(turm);
                globals.setGlobalPosition(ref turm,gridMap.MapToWorld((int)ret.x,(int)ret.y,(int)ret.z));
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        updateMouse(mousepos);      // Ball - Visuelle Rückmeldung
    }

    private Vector3 updateMouse(Vector2 mouse)
    {
        Vector3 ret = illegal;       
        
        Vector3 from = camera.ProjectRayOrigin(mouse);
        Vector3 to = (camera.ProjectRayNormal(mouse) * 500);
        var col = GetWorld().DirectSpaceState.IntersectRay(from, to);
        
        if (col != null)
        {
            foreach (var h in col.Keys)
            {
                if (h.Equals("collider"))
                {
                    Vector3 p = (Vector3)col["position"];

                    Transform t = ball.GlobalTransform;
                    t.origin = p;
                    ball.GlobalTransform = t;

                    Vector3 m = gridMap.WorldToMap(p);
                    int cell = gridMap.GetCellItem((int)m.x, (int)m.y, (int)m.z);
                    
                    if (cell != -1)
                    {
                        if (cell == 1)  // placement
                        {
                            ball.MaterialOverride = ballMat;          
                            ret = m;
                        }
                        else
                            ball.MaterialOverride = null;
                    }
                }
            }
        }
        return ret;
    }
}
