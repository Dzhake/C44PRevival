namespace DuckGame.C44P;

public abstract class BrickTilesetBase(float xval, float yval, string name)
    : SimpleTilesetBase(xval, yval, $"{name} brick", $"Bricks/{name}Brick");

[EditorGroup("ADGM|Tiles|Bricks")]
public class RedBrickTileset(float xval, float yval) : BrickTilesetBase(xval, yval, "Red");

[EditorGroup("ADGM|Tiles|Bricks")]
public class BlueBrickTileset(float xval, float yval) : BrickTilesetBase(xval, yval, "Blue");

[EditorGroup("ADGM|Tiles|Bricks")]
public class GreenBrickTileset(float xval, float yval) : BrickTilesetBase(xval, yval, "Green");

[EditorGroup("ADGM|Tiles|Bricks")]
public class LightGreenBrickTileset(float xval, float yval) : BrickTilesetBase(xval, yval, "LightGreen");

[EditorGroup("ADGM|Tiles|Bricks")]
public class OrangeBrickTileset(float xval, float yval) : BrickTilesetBase(xval, yval, "Orange");

[EditorGroup("ADGM|Tiles|Bricks")]
public class PinkBrickTileset(float xval, float yval) : BrickTilesetBase(xval, yval, "Pink");

[EditorGroup("ADGM|Tiles|Bricks")]
public class VioletBrickTileset(float xval, float yval) : BrickTilesetBase(xval, yval, "Violet");

[EditorGroup("ADGM|Tiles|Bricks")]
public class YellowBrickTileset(float xval, float yval) : BrickTilesetBase(xval, yval, "Yellow");