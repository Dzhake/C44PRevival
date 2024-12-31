using System;

namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns|Grenades")]
public class FireGrenade : BaseGrenade
{
    protected float volume = 0.4f;
    public FireGrenade(float xval, float yval) : base(xval, yval)
    {
        sprite = new SpriteMap(GetPath("Sprites/Items/Weapons/FireGrenade.png"), 16, 16, false);
        _graphic = sprite;
        _center = new Vec2(8f, 8f);
        _collisionOffset = new Vec2(-3f, -5f);
        _collisionSize = new Vec2(6f, 10f);
        Timer = 2f;
        bouncy = 0.4f;
        friction = 0.05f;
    }

    protected void PourOut()
    {
        FluidData _fluidData = Fluid.Gas;

        var _stream = new FluidStream(x, y, new Vec2(1f, 0.0f), 2f);
        Level.Add(_stream);

        int pie_pieces = 16;
        for (int i = 0; i < pie_pieces; i++)
        {
            _stream.Draw();
            _stream.sprayAngle = new Vec2((float)Math.Cos(i * 360 / pie_pieces), (float)Math.Sin(i * 360 / pie_pieces)) * 1.5f;
            _stream.DoUpdate();
            _stream.position = position;
            _fluidData.amount = volume / pie_pieces;
            _stream.Feed(_fluidData);
        }

        for (int i = 0; i < 4; i++)
        {
            Spark spark = Spark.New(position.x, position.y, new Vec2(Rando.Float(-2, 2), Rando.Float(-3, -4)), 0.002f);
            Level.Add(spark);
        }
    }

    public override void Explode()
    {
        PourOut();
        Level.Remove(this);
    }

    public override void OnPressAction()
    {
        if (HasPin)
        {
            Level.Add(new Shell(x, y, GetPath<C44P>("Sprites/Items/Weapons/FireGrenadePin.png"))
            {
                hSpeed = -offDir * (1.5f + Rando.Float(0.5f)),
                vSpeed = -2f
            });
        }
        base.OnPressAction();
    }
}
