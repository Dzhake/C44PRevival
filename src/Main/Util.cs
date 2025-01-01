using System.IO;

namespace DuckGame.C44P;

public static class Util
{
    public static void AddLevels(string path = "")
    {
        foreach (string file in Directory.GetFiles(Mod.GetPath<C44P>("/Levels/" + path)))
        {
            if (!DuckNetwork.core._activatedLevels.Contains(file))
            {
                DuckNetwork.core._activatedLevels.Add(file);
            }
        }
    }

    
    public static void TryReskin(Gun gun, string fileName, int frameWidth, int frameHeight)
    {
        if (gun is not { owner: Duck d } || d._equipment == null) return;

        string skin = "";
        if (d.HasEquipment(typeof(Rainbow)))
            skin = "Rainbow";
        if (d.HasEquipment(typeof(Carbon)))
            skin = "Carbon";
        if (d.HasEquipment(typeof(Jungle)))
            skin = "Jungle";
        if (d.HasEquipment(typeof(Aqua)))
            skin = "Aqua";

        if (skin != "")
            gun.graphic = new SpriteMap(Mod.GetPath<C44P>($"{C44P.WeaponsPath}{skin}/{fileName}"), frameWidth, frameHeight);
    }
}
