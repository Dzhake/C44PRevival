namespace DuckGame.C44P;

public abstract class SimpleTilesetBase : AutoBlock
{
    protected SimpleTilesetBase(float x, float y, string editorName, string path, PhysicsMaterial material, float verticalWidth,
        float verticalWidthThick, float horizontalHeight) : base(x, y, $"{C44P.TilesetsPath}{path}")
    {
        _editorName = editorName;
        physicsMaterial = material;
        this.verticalWidth = verticalWidth;
        this.verticalWidthThick = verticalWidthThick;
        this.horizontalHeight = horizontalHeight;
    }

    protected SimpleTilesetBase(float x, float y, string editorName, string path) : base(x, y,
        $"{C44P.TilesetsPath}{path}")
    {
        _editorName = editorName;
        physicsMaterial = PhysicsMaterial.Metal;
        verticalWidth = 10f;
        verticalWidthThick = 15f;
        horizontalHeight = 14f;
    }
}