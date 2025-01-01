namespace DuckGame.C44P
{
    public class C44P : Mod
    {
        public static readonly string WeaponsPath = "Sprites/Items/Weapons/";

        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();
            AutoPatchHandler.Patch();
        }
	}
}


