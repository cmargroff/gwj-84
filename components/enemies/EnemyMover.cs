using Godot;

public partial class EnemyMover : Node2D
{
  [Export] Path2D Path;
  [Export] public float MoveSpeed = 30f;
  private PathFollow2D _follower;
  private bool _done = false;
  public override void _EnterTree()
  {
    _follower = new PathFollow2D();
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
    GlobalPosition = _follower.GlobalPosition;
    GlobalRotation = _follower.GlobalRotation;
  }

  public void MoveToPath(Path2D newPath)
  {
    _follower.Reparent(newPath);
    _follower.Progress = 0;
    _done = false;
    Path = newPath;
  }

}
