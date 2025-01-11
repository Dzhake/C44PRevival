namespace DuckGame.C44P
{
    public class C44P : Mod
    {
        public static readonly string ContentPath = GetPath<C44P>("");
        public static readonly string Soundspath = $"{ContentPath}SFX/";
        public static readonly string SpritesPath = $"{ContentPath}Sprites/";
        public static readonly string WeaponsPath = $"{SpritesPath}Items/Weapons/";
        public static readonly string TilesetsPath = $"{SpritesPath}Tilesets/";

        public override void OnPostInitialize()
        {
            base.OnPostInitialize();
            AutoPatchHandler.Patch();
            GMIcons.Initialize();
        }
	}
}


