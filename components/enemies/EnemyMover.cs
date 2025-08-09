using System;
using Godot;

public partial class EnemyMover : Node2D
{
  [Export] Path2D Path;
  [Export] public float MoveSpeed = 30f;
  [Export] public NoiseTexture2D Noise;
  [Export] public float WanderAngle = 0.3f * Mathf.Pi;
  private PathFollow2D _follower;
  private bool _done = false;
  public override void _EnterTree()
  {
    _follower = new PathFollow2D();
    var sprite = new Sprite2D();
    var texture = ResourceLoader.Load<Texture2D>("res://icon.svg");
    sprite.Texture = texture;
    sprite.Modulate = new Color(1, 1, 1, 0.25f);
    _follower.AddChild(sprite);
    _follower.Loop = false;
    AddChild(_follower);
    MoveToPath(Path);
  }

  private void StartNextPath()
  {
    if (Path is LinkedPath linkedPath && linkedPath.NextPath is not null)
    {
      MoveToPath(linkedPath.NextPath);
    }
  }

  public override void _Process(double delta)
  {
    if (_done) return;
    _follower.Progress += (float)delta * MoveSpeed;
    if (_follower.ProgressRatio >= 1f)
    {
      _done = true;
      StartNextPath();
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    var angle = GlobalPosition.AngleToPoint(_follower.GlobalPosition);
    var distance = GlobalPosition.DistanceTo(_follower.GlobalPosition);

    var v = (Noise.Noise as FastNoiseLite).GetNoise2D(GlobalPosition.X, GlobalPosition.Y);

    var newAngle = Mathf.LerpAngle(angle - WanderAngle, angle + WanderAngle, v);

    var newPos = Vector2.FromAngle(newAngle) * (distance * 0.05f);


    GlobalPosition += newPos;
    GlobalRotation = newAngle; // might need to use a smoothed difference here
    // GD.Print(GlobalPosition);
  }

  public void MoveToPath(Path2D newPath)
  {
    _follower.Reparent(newPath);
    _follower.Progress = 0;
    _done = false;
    Path = newPath;
  }

}
