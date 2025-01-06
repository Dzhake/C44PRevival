using System.IO;

namespace DuckGame.C44P;

public static class Util
{
    public static bool TryReskin(Gun gun, string fileName, int frameWidth, int frameHeight)
    {
        if (gun is not { owner: Duck d } || d._equipment == null) return false;
        string skin = "";
        if (d.HasEquipment(typeof(Rainbow)))
            skin = "Rainbow";
        if (d.HasEquipment(typeof(Carbon)))
            skin = "Carbon";
        if (d.HasEquipment(typeof(Jungle)))
            skin = "Jungle";
        if (d.HasEquipment(typeof(Aqua)))
            skin = "Aqua";

        string path = $"{C44P.WeaponsPath}{skin}/{fileName}";

        if (skin == "") return false;

        gun.graphic = new SpriteMap(path, frameWidth, frameHeight);
        return true;

    }
}
