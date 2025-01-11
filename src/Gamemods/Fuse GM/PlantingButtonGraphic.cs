namespace DuckGame.C44P;

public class PlantingButtonGraphic : Thing
{
    public PlantingButtonGraphic(float xval, float yval) : base(xval, yval)
    {
        _center = new Vec2(6f, 8f);
        _collisionOffset = new Vec2(-6f, -8f);
        _collisionSize = new Vec2(12f, 16f);
        SpriteMap sprite = new(GetPath("Sprites/Gamemodes/Fuse/Buttons.png"), 12, 16)
        {
            frame = Rando.Int(0, 9)
        };
        _graphic = sprite;
    }
    public override void Update()
    {
        base.Update();
        alpha -= 0.03f;
        if (alpha < 0.1f)
            Level.Remove(this);
    }
}