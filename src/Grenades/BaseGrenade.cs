namespace DuckGame.C44P;

public abstract class BaseGrenade : Gun
{
    public StateBinding TimerBinding = new("Timer");
    public StateBinding HasPinBinding = new("HasPin");
    public bool HasPin = true;
    public float Timer;
    protected SpriteMap sprite;
    public bool HasExploded
    {
        get;
        protected set;
    }

    protected BaseGrenade(float xval, float yval) : base(xval, yval)
    {
        ammo = 1;
    }

    public override void Fire() { }

    public virtual void Explode()
    {
        HasExploded = true;
    }

    public override void OnPressAction()
    {
        HasPin = false;
    }

    public override void Update()
    {
        base.Update();
        UpdateTimer();
        UpdateFrame();
    }

    protected virtual void UpdateTimer()
    {
        if (HasPin) return;

        if (Timer > 0)
            Timer -= Maths.IncFrameTimer();
        else if (!HasExploded)
            Explode();
    }

    protected virtual void UpdateFrame()
    {
        sprite.frame = HasPin ? 0 : 1;
    }

    //why the hell EjectedShell is abstract..
    protected class Shell(float xpos, float ypos, string path) : EjectedShell(xpos, ypos, path);
}
