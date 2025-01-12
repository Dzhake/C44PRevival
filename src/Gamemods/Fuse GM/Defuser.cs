namespace DuckGame.C44P;

[EditorGroup("ADGM|GameMode Fuse")]
[BaggedProperty("canSpawn", false)]
public class Defuser : Holdable
{
    public Defuser(float xval, float yval) : base(xval, yval)
    {
        _center = new Vec2(7f, 5f);
        _collisionOffset = new Vec2(-4f, -5f);
        _collisionSize = new Vec2(8f, 11f);
        _graphic = new($"{C44P.SpritesPath}Gamemodes/Fuse/Defuser");
        _weight = 1f;
        tapeable = false;
    }

    public override void Update()
    {
        base.Update();
        canPickUp = true;
    }
}