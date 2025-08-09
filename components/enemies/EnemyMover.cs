using Godot;

public partial class EnemyMover : PathFollow2D
{
  [Export] public float MoveSpeed = 30f;
  private bool _done = false;
  public override void _EnterTree()
  {
    Loop = false;
  }

  private void StartNextPath()
  {
    var path = GetParent();
    if (path is LinkedPath linkedPath && linkedPath.NextPath is not null)
    {
      MoveToPath(linkedPath.NextPath);
    }
  }

  public override void _Process(double delta)
  {
    if (_done) return;
    Progress += (float)delta * MoveSpeed;
    if (ProgressRatio >= 1f)
    {
      _done = true;
      StartNextPath();
    }
  }

  public void MoveToPath(Path2D newPath)
  {
    Reparent(newPath);
    Progress = 0;
    _done = false;
  }

}
