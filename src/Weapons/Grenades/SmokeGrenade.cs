namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns|Grenades")]
[BaggedProperty("isFatal", false)]
public class SmokeGrenade : BaseGrenade
{
    protected int charges = 30;

    public SmokeGrenade(float xval, float yval) : base(xval, yval)
    {
        sprite = new SpriteMap(GetPath("Sprites/Items/Weapons/SmokeGrenade"), 16, 16, false);
        _graphic = sprite;
        _center = new Vec2(7f, 7f);
        _collisionOffset = new Vec2(-4f, -5f);
        _collisionSize = new Vec2(8f, 12f);
        Timer = 1f;
        bouncy = 0.4f;
        friction = 0.05f;
    }

    public override void Explode()
    {
        if (charges > 0)
        {
            Timer = charges > 25 ? 0.1f : 0.5f;
            for (int i = 0; i < 8; i++)
                Level.Add(new SmokeGR(x, y));
            charges--;
            HasExploded = false;
        }
        else
            Level.Remove(this);
    }
}

