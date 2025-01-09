namespace DuckGame.C44P
{
    public class C44P : Mod
    {
        public static readonly string SpritesPath = GetPath<C44P>("Sprites/");
        public static readonly string WeaponsPath = $"{SpritesPath}Items/Weapons/";
        public static readonly string TilesetsPath = $"{SpritesPath}Tilesets/";

        public override void OnPostInitialize()
        {
            base.OnPostInitialize();
            AutoPatchHandler.Patch();
        }
	}
}


