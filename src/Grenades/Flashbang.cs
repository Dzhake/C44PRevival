namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns|Grenades")]
[BaggedProperty("isFatal", false)]
public class FlashbangGrenade : BaseGrenade
{
    public FlashbangGrenade(float xval, float yval) : base(xval, yval)
    {
        sprite = new SpriteMap(GetPath("Sprites/Items/Weapons/Flashbang.png"), 16, 16);
        _graphic = sprite;
        _center = new Vec2(7f, 7f);
        _collisionOffset = new Vec2(-4f, -5f);
        _collisionSize = new Vec2(8f, 12f);
        Timer = 2f;
        bouncy = 0.4f;
        friction = 0.05f;
    }

    public override void Explode()
    {
        QuickFlash();
        Flash();
        SFX.Play(GetPath("flashGrenadeExplode.wav"));
        Level.Remove(this);
    }

    public virtual void QuickFlash()
    {
        Graphics.flashAdd = 1.3f;
    }

    public virtual void Flash()
    {
        if (!isServerForObject) return;
        Level.Add(new Flashlight(position.x, position.y));
    }

    public override void OnPressAction()
    {
        Level.Add(new Shell(x, y, GetPath<C44P>("Sprites/Items/Weapons/FlashbangPin.png"))
        {
            hSpeed = -offDir * (1.5f + Rando.Float(0.5f)),
            vSpeed = -2f
        });
        base.OnPressAction();
    }
}