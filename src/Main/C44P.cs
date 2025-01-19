namespace DuckGame.C44P
{
    public class C44P : Mod
    {
        public static readonly string ContentPath = GetPath<C44P>("");
        public static readonly string Soundspath = $"{ContentPath}SFX/";
        public static readonly string SpritesPath = $"{ContentPath}Sprites/";
        public static readonly string WeaponsPath = $"{SpritesPath}Items/Weapons/";
        public static readonly string TilesetsPath = $"{SpritesPath}Tilesets/";

        public static bool awpdebug;

        public override void OnPostInitialize()
        {
            base.OnPostInitialize();
            DependencyResolver.ResolveDependencies();
            AutoPatchHandler.Patch();
            GMIcons.Initialize();
            DevConsole.AddCommand(new CMD("resavelevels", delegate ()
            {
                if (Level.current is Editor editor)
                {
                    editor.Resave($"{ContentPath}Levels/");
                    DevConsole.Log("Starting resave!", Color.LightGreen);
                }
                else
                    DevConsole.Log("Current level is not Editor!", Color.Red);
            }));

            DevConsole.AddCommand(new CMD("awpdebug", delegate ()
            {
                awpdebug = !awpdebug;
                DevConsole.Log($"AWP Debug is now {(awpdebug ? "on" : "off")}.", Color.LightGreen);
            }));
        }
	}
}


