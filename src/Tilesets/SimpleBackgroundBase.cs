namespace DuckGame.C44P;

public abstract class SimpleBackgroundBase : BackgroundTile
{
    protected SimpleBackgroundBase(float xpos, float ypos, string editorName, string path) : base(xpos, ypos)
    {
        _graphic = new SpriteMap($"{C44P.TilesetsPath}{path}", 16, 16, true);
        _opacityFromGraphic = true;
        _center = new Vec2(8f, 8f);
        _collisionSize = new Vec2(16f, 16f);
        _collisionOffset = new Vec2(-8f, -8f);
        _editorName = editorName;
    }
}
