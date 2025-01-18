namespace DuckGame.C44P;

public class SmokeGR : Thing
{
    public float Timer;

    public Vec2 move;
    protected float angleIncrement;
    protected float scaleDecrement;
    protected float fastGrowTimer;


    public SmokeGR(float xpos, float ypos, float stayTime = 1f) : base(xpos, ypos)
    {
        velocity = new Vec2(Rando.Float(-3, 3), Rando.Float(-3, 3));
        xscale = Rando.Float(0.30f, 0.35f);
        yscale = xscale;
        _angle = Maths.DegToRad(Rando.Float(360f));
        fastGrowTimer = Rando.Float(0.8f, 1f);
        Timer = stayTime;
        angleIncrement = Maths.DegToRad(Rando.Float(2f) - 1f);
        scaleDecrement = Rando.Float(0.001f, 0.002f);
        move = new Vec2(Rando.Float(-0.03f, 0.03f), Rando.Float(-0.03f, 0.03f));

        GraphicList graphicList = new();
        Sprite graphic1 = new("smoke")
        {
            alpha = 1f,
            depth = 1f
        };
        graphic1.CenterOrigin();
        graphicList.Add(graphic1);

        Sprite graphic2 = new("smokeBack")
        {
            depth = 0.1f,
            alpha = 1f
        };
        graphic2.CenterOrigin();
        graphicList.Add(graphic2);

        _graphic = graphicList;
        _center = new Vec2(0, 0);
        depth = 1f;
    }

    public override void Update()
    {
        angle += angleIncrement;

        foreach (MaterialThing t in Level.CheckRectAll<MaterialThing>(position, position + new Vec2(44, 42)))
            t.Extinquish();

        if (Timer > 0)
            Timer -= 0.01f;
        else
        {
            xscale -= scaleDecrement;
            scaleDecrement += 0.0001f;
        }

        if (fastGrowTimer > 0.0)
        {
            fastGrowTimer -= 0.05f;
            xscale += 0.05f;
        }

        yscale = xscale;

        position += move;
        position += velocity;
        velocity *= new Vec2(0.9f, 0.9f);

        if (xscale < 0.100000001490116)
            Level.Remove(this);
    }
}

