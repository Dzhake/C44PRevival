namespace DuckGame.C44P;

public static class GMIcons
{
    public static Sprite Off = new($"{C44P.SpritesPath}Gamemodes/StatusOFF");
    public static Sprite On = new($"{C44P.SpritesPath}Gamemodes/StatusON");
    public static Sprite Warn = new($"{C44P.SpritesPath}Gamemodes/StatusWARN");

    public static void Initialize()
    {
        Off.CenterOrigin();
        On.CenterOrigin();
        Warn.CenterOrigin();
    }
}
