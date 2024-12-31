using System.Collections.Generic;
using System.Reflection;
using System;

namespace DuckGame.C44P
{
    class updater : IEngineUpdatable
    {
        private static readonly string weaponsPath = "Sprites/Items/Weapons/";
        public SinWave _wave = 0.3f;
        public static RenderTarget2D _renderTarget;
        public static int Defender;
        public static int Attacker;

        public updater()
        {
            MonoMain.RegisterEngineUpdatable(this);
        }

        public void PreUpdate() {}

        public void Reskin(Type weaponType, string fileName, int frameWidth, int frameHeight)
        {
            foreach (Gun gun in Level.current.things[weaponType])
            {
                if (gun is not { owner: Duck d }) continue;

                if (d._equipment == null) continue;
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
                    gun.graphic = new SpriteMap(Mod.GetPath<C44P>($"{weaponsPath}{skin}{fileName}"), frameWidth, frameHeight);
            }
        }

        public void Update()
        {
            Reskin(typeof(Thunderbuss), "/Newblunderbuss.png", 29, 10);
            Reskin(typeof(MagnumOpus), "/Newmagnum.png", 32, 32);
            Reskin(typeof(M16), "/m16.png", 32, 32);
            Reskin(typeof(XM1014), "/xm1014.png", 32, 32);
            Reskin(typeof(NewPistol), "Newpistol.png", 32, 16);
            Reskin(typeof(MP5), "/Newsmg.png", 20, 10);
            Reskin(typeof(OldVinchester), "/Newoldpistol.png", 32, 32);
            Reskin(typeof(SNAIPER), "/awp.png", 40, 10);
        }

        private static int prevMode;

        public static List<LSItem> removedLevelsDM = new List<LSItem>();
        public static List<LSItem> removedLevelsFuse = new List<LSItem>();
        public static List<LSItem> removedLevelsCTF = new List<LSItem>();
        public static List<LSItem> removedLevelsCP = new List<LSItem>();
        public static List<LSItem> removedLevelsC = new List<LSItem>();
        public static List<LSItem> removedLevelsTheft = new List<LSItem>();

        public static List<string> FuseLevels = new List<string>();
        public static List<string> CTFLevels = new List<string>();
        public static List<string> CPLevels = new List<string>();
        public static List<string> CLevels = new List<string>();
        public static List<string> TheftLevels = new List<string>();
        public void PostUpdate()
        {
            if (Level.current is TeamSelect2)
            {
                TeamSelect2 level = Level.current as TeamSelect2;
                LevelSelect select = null;
                int selectedGamemode = (int)TeamSelect2.GetMatchSetting("gamemode").value;
                if (Network.isActive && !Network.isServer)
                {
                    prevMode = selectedGamemode;
                }
                LevelSelectCompanionMenu menu;
                if (Network.isActive)
                {
                    menu = DuckNetwork.core._levelSelectMenu as LevelSelectCompanionMenu;
                }
                else
                {
                    menu = (LevelSelectCompanionMenu)typeof(TeamSelect2).GetField("_levelSelectMenu", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(level);
                }
                if (menu != null)
                {
                    select = (LevelSelect)typeof(LevelSelectCompanionMenu).GetField("_levelSelector", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);
                    if (select != null)
                    {
                        List<LSItem> items = (List<LSItem>)typeof(LevelSelect).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(select); 
                        if (items != null)
                        {
                            List<LSItem> remove = new List<LSItem>();
                            List<LSItem> removeFuse = new List<LSItem>();
                            List<LSItem> removeCTF = new List<LSItem>();
                            List<LSItem> removeCP = new List<LSItem>();
                            List<LSItem> removeC = new List<LSItem>();
                            List<LSItem> removeTheft = new List<LSItem>();

                            foreach (LSItem item in items)
                            {
                                if (item != null && item.data != null && !removedLevelsDM.Contains(item) && !removedLevelsFuse.Contains(item) && !removedLevelsCTF.Contains(item)
                                     && !removedLevelsCP.Contains(item) && !removedLevelsC.Contains(item) && !removedLevelsTheft.Contains(item))
                                {
                                    bool hasDM = false;
                                    bool hasFuseGM = false;
                                    bool hasCTFGM = false;
                                    bool hasCPGM = false;
                                    bool hasCGM = false;
                                    bool hasTheftGM = false;

                                    try
                                    {
                                        foreach (BinaryClassChunk elly in item.data.objects.objects)
                                        {
                                            string typeString = (string)elly.GetProperty("type");
                                            if (typeString != null && typeString.Contains("DuckGame.C44P.GM_Fuse"))
                                            {
                                                hasFuseGM = true;
                                            }
                                            if (typeString != null && typeString.Contains("DuckGame.C44P.GM_CTF"))
                                            {
                                                hasCTFGM = true;
                                            }
                                            if (typeString != null && typeString.Contains("DuckGame.C44P.GM_CP"))
                                            {
                                                hasCPGM = true;
                                            }
                                            if (typeString != null && typeString.Contains("DuckGame.C44P.GM_Collection"))
                                            {
                                                hasCGM = true;
                                            }
                                            if (typeString != null && typeString.Contains("DuckGame.C44P.GM_STOLEN"))
                                            {
                                                hasTheftGM = true;
                                            }
                                            if(!hasFuseGM && !hasCTFGM && !hasCPGM && !hasCGM && !hasTheftGM)
                                            {
                                                hasDM = true;
                                            }
                                        }
                                    }
                                    catch { }
                                    if (hasDM)
                                    {
                                        remove.Add(item);
                                    }
                                    if (hasFuseGM)
                                    {
                                        removeFuse.Add(item);
                                    }
                                    if (hasCTFGM)
                                    {
                                        removeCTF.Add(item);
                                    }
                                    if (hasCPGM)
                                    {
                                        removeCP.Add(item);
                                    }
                                    if (hasCGM)
                                    {
                                        removeC.Add(item);
                                    }
                                    if (hasTheftGM)
                                    {
                                        removeTheft.Add(item);
                                    }
                                }
                            }
                            foreach (LSItem item in remove)
                            {
                                removedLevelsDM.Add(item);
                                items.Remove(item);
                            }
                            foreach (LSItem item in removeFuse)
                            {
                                removedLevelsFuse.Add(item);
                                items.Remove(item);
                            }
                            foreach (LSItem item in removeCTF)
                            {
                                removedLevelsCTF.Add(item);
                                items.Remove(item);
                            }
                            foreach (LSItem item in removeCP)
                            {
                                removedLevelsCP.Add(item);
                                items.Remove(item);
                            }
                            foreach (LSItem item in removeC)
                            {
                                removedLevelsC.Add(item);
                                items.Remove(item);
                            }
                            foreach (LSItem item in removeTheft)
                            {
                                removedLevelsTheft.Add(item);
                                items.Remove(item);
                            }
                            typeof(LevelSelect).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(select, items);
                        }
                    }
                }
                if (DuckNetwork.core._activatedLevels.Count > 0)
                {
                    foreach (string t in DuckNetwork.core._activatedLevels)
                    {
                        if (!FuseLevels.Contains(t) && !CTFLevels.Contains(t) && !CPLevels.Contains(t) && !CLevels.Contains(t) && !TheftLevels.Contains(t))
                        {
                            LevelData lev = DuckFile.LoadLevel(t);
                            if (lev != null)
                            {
                                bool hasFuseGM = false;
                                bool hasCTFGM = false;
                                bool hasCPGM = false;
                                bool hasCGM = false;
                                bool hasTheftGM = false;

                                foreach (BinaryClassChunk elly in lev.objects.objects)
                                {
                                    string typeString = (string)elly.GetProperty("type");
                                    if (typeString != null)
                                    {
                                        if (typeString.Contains("DuckGame.C44P.GM_Fuse"))
                                        {
                                            hasFuseGM = true;
                                        }
                                        if (typeString.Contains("DuckGame.C44P.GM_CTF"))
                                        {
                                            hasCTFGM = true;
                                        }
                                        if (typeString.Contains("DuckGame.C44P.GM_CP"))
                                        {
                                            hasCPGM = true;
                                        }
                                        if (typeString.Contains("DuckGame.C44P.GM_Collection"))
                                        {
                                            hasCGM = true;
                                        }
                                        if (typeString.Contains("DuckGame.C44P.GM_STOLEN"))
                                        {
                                            hasTheftGM = true;
                                        }
                                    }
                                }
                                if (hasFuseGM && !FuseLevels.Contains(t))
                                {
                                    FuseLevels.Add(t);
                                }
                                if (hasCTFGM && !CTFLevels.Contains(t))
                                {
                                    CTFLevels.Add(t);
                                }
                                if (hasCPGM && !CPLevels.Contains(t))
                                {
                                    CPLevels.Add(t);
                                }
                                if (hasCGM && !CLevels.Contains(t))
                                {
                                    CLevels.Add(t);
                                }
                                if (hasTheftGM && !TheftLevels.Contains(t))
                                {
                                    TheftLevels.Add(t);
                                }
                            }
                        }
                    }
                }
                if (selectedGamemode == 0) //Vanilla
                {
                    if (select != null)
                    {
                        foreach (LSItem item in removedLevelsDM)
                        {
                            select.AddItem(item);
                        }
                        removedLevelsDM.Clear();
                    }

                    if(prevMode != 0)
                    {
                        //C44P.AddLevels("vanilla/");
                    }

                    foreach (string t in FuseLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CTFLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CPLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in TheftLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                }
                if (selectedGamemode == 1) //Fuse
                {
                    if (select != null)
                    {
                        foreach (LSItem item in removedLevelsFuse)
                        {
                            select.AddItem(item);
                        }
                        removedLevelsFuse.Clear();
                    }

                    C44P.AddLevels("Fuse/");

                    foreach (string t in CTFLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CPLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in TheftLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                }
                if (selectedGamemode == 2) //CTF
                {
                    if (select != null)
                    {
                        foreach (LSItem item in removedLevelsCTF)
                        {
                            select.AddItem(item);
                        }
                        removedLevelsCTF.Clear();
                    }

                    C44P.AddLevels("CTF/");

                    foreach (string t in FuseLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CPLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in TheftLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                }
                if (selectedGamemode == 3) //CP
                {
                    if (select != null)
                    {
                        foreach (LSItem item in removedLevelsCP)
                        {
                            select.AddItem(item);
                        }
                        removedLevelsCP.Clear();
                    }

                    C44P.AddLevels("CP");

                    foreach (string t in FuseLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CTFLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in TheftLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                }
                if (selectedGamemode == 4) //C
                {
                    if (select != null)
                    {
                        foreach (LSItem item in removedLevelsC)
                        {
                            select.AddItem(item);
                        }
                        removedLevelsC.Clear();
                    }

                    C44P.AddLevels("C/");

                    foreach (string t in FuseLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CTFLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CPLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in TheftLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                }
                if (selectedGamemode == 5) //Theft
                {
                    if (select != null)
                    {
                        foreach (LSItem item in removedLevelsTheft)
                        {
                            select.AddItem(item);
                        }
                        removedLevelsTheft.Clear();
                    }

                    C44P.AddLevels("Theft/");

                    foreach (string t in FuseLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CTFLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CPLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                    foreach (string t in CLevels)
                    {
                        DuckNetwork.core._activatedLevels.Remove(t);
                    }
                }
            }
            else
            {
                FuseLevels.Clear();
                CTFLevels.Clear();
                CPLevels.Clear();
                CLevels.Clear();
                TheftLevels.Clear();
            }
        }

        public void OnDrawLayer(Layer pLayer) {}
    }
}
