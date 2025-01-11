namespace DuckGame.C44P
{
    [EditorGroup("ADGM|GameMode Fuse")]
    public class PlantZone : Thing
    {
        public PlantZone(float xval, float yval) : base(xval, yval)
        {
            _graphic = new Sprite($"{C44P.SpritesPath}Gamemodes/Fuse/PlantZone");
            _center = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-8f, -8f);
            _collisionSize = new Vec2(16f, 16f);
            _visibleInGame = false;
            layer = Layer.Foreground;
        }
    }
}
