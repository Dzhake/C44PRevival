using System;

namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns|Grenades")]
public class FireGrenade : BaseGrenade
{
    protected float volume = 0.4f;
    public FireGrenade(float xval, float yval) : base(xval, yval)
    {
        sprite = new SpriteMap($"{C44P.WeaponsPath}FireGrenade", 8, 13);
        _graphic = sprite;
        _center = new Vec2(3, 6);
        _collisionOffset = new Vec2(-3, -6);
        _collisionSize = new Vec2(8, 13);
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
        Fondle(_stream);
    }

    public override void Update()
    {
        if (!HasPin && grounded) Explode();
        base.Update();
    }

    public override void Explode()
    {
        if (isServerForObject) PourOut();
        Level.Remove(this);
    }

    public override void OnPressAction()
    {
        if (HasPin && isServerForObject)
            Level.Add(new GrenadePin(x, y)
            {
                hSpeed = -offDir * (1.5f + Rando.Float(0.5f)),
                vSpeed = -2f
            });
        base.OnPressAction();
    }
}
