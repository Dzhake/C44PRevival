using System.Collections.Generic;
using System.Reflection;
using System.Timers;

namespace DuckGame.C44P
{
    class AutoUpdate : IEngineUpdatable
    {
        public enum Gamemode { Vanilla, Fuse, CTF, CP, C, Theft }

        public SinWave _wave = 0.3f;
        public static RenderTarget2D _renderTarget;

        public AutoUpdate()
        {
            MonoMain.RegisterEngineUpdatable(this);
        }

        public void PreUpdate() { }


        public void Update() { }

        private float Timer;
        private bool didClear;

        public static List<LSItem> removedLevelsDM = new();
        public static List<LSItem> removedLevelsFuse = new();
        public static List<LSItem> removedLevelsCTF = new();
        public static List<LSItem> removedLevelsCP = new();
        public static List<LSItem> removedLevelsC = new();
        public static List<LSItem> removedLevelsTheft = new();

        public static List<string> FuseLevels = new();
        public static List<string> CTFLevels = new();
        public static List<string> CPLevels = new();
        public static List<string> CLevels = new();
        public static List<string> TheftLevels = new();

        private static readonly FieldInfo _levelSelectMenuField =
            typeof(TeamSelect2).GetField("_levelSelectMenu", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _levelSelectorField =
            typeof(LevelSelectCompanionMenu).GetField("_levelSelector", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _itemsField =
            typeof(LevelSelect).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);

        public void PostUpdate()
        {
            Timer -= Maths.IncFrameTimer();
            if (Timer > 0) return;
            Timer = 1f;

            if (Level.current is TeamSelect2)
            {
                didClear = false;
                TeamSelect2 level = Level.current as TeamSelect2;
                LevelSelect select = null;
                int selectedGamemode = (int)TeamSelect2.GetMatchSetting("gamemode").value;
                /*LevelSelectCompanionMenu menu;
                if (Network.isActive)
                    menu = DuckNetwork.core._levelSelectMenu as LevelSelectCompanionMenu;
                else
                    menu = (LevelSelectCompanionMenu)_levelSelectMenuField.GetValue(level);*/
                /*if (menu != null)
                {
                    select = (LevelSelect)_levelSelectorField.GetValue(menu);
                    if (select != null)
                    {
                        List<LSItem> allLevels = (List<LSItem>)_itemsField.GetValue(select);
                        if (allLevels != null)
                        {
                            List<LSItem> toRemove = new();
                            foreach (LSItem item in allLevels)
                            {
                                if (item?.data == null || removedLevelsDM.Contains(item) ||
                                    removedLevelsFuse.Contains(item) || removedLevelsCTF.Contains(item)
                                    || removedLevelsCP.Contains(item) || removedLevelsC.Contains(item) ||
                                    removedLevelsTheft.Contains(item)) continue;
                                
                                Gamemode gm = ContainsGamemodeSpecific(item.data.objects.objects);

                                switch (gm)
                                {
                                    case Gamemode.Vanilla:
                                        removedLevelsDM.Add(item);
                                        toRemove.Add(item);
                                        break;
                                    case Gamemode.Fuse:
                                        removedLevelsFuse.Add(item);
                                        toRemove.Add(item);
                                        break;
                                    case Gamemode.CTF:
                                        removedLevelsCTF.Add(item);
                                        toRemove.Add(item);
                                        break;
                                    case Gamemode.CP:
                                        removedLevelsCP.Add(item);
                                        toRemove.Add(item);
                                        break;
                                    case Gamemode.C:
                                        removedLevelsC.Add(item);
                                        toRemove.Add(item);
                                        break;
                                    case Gamemode.Theft:
                                        removedLevelsTheft.Add(item);
                                        toRemove.Add(item);
                                        break;
                                }
                            }

                            foreach (LSItem item in toRemove)
                                allLevels.Remove(item);
                            _itemsField.SetValue(select, allLevels);
                        }
                    }
                }


                if (DuckNetwork.core._activatedLevels.Count > 0)
                {
                    foreach (string t in DuckNetwork.core._activatedLevels)
                    {
                        if (FuseLevels.Contains(t) || CTFLevels.Contains(t) || CPLevels.Contains(t) ||
                            CLevels.Contains(t) || TheftLevels.Contains(t)) continue;

                        LevelData lev = DuckFile.LoadLevel(t);
                        if (lev == null) continue;

                        Gamemode gm = ContainsGamemodeSpecific(lev.objects.objects);

                        switch (gm)
                        {
                            case Gamemode.Fuse when !FuseLevels.Contains(t):
                                FuseLevels.Add(t);
                                break;
                            case Gamemode.CTF when !CTFLevels.Contains(t):
                                CTFLevels.Add(t);
                                break;
                            case Gamemode.CP when !CPLevels.Contains(t):
                                CPLevels.Add(t);
                                break;
                            case Gamemode.C when !CLevels.Contains(t):
                                CLevels.Add(t);
                                break;
                            case Gamemode.Theft when !TheftLevels.Contains(t):
                                TheftLevels.Add(t);
                                break;
                        }
                    }
                }

                if (select != null)
                {
                    switch (selectedGamemode)
                    {
                        //Vanilla
                        case 0:
                        {
                            AddAndClearLevels(ref removedLevelsDM, ref select);
                            break;
                        }
                        //Fuse
                        case 1:
                        {
                            AddAndClearLevels(ref removedLevelsFuse, ref select);
                            Util.AddLevels("Fuse/");
                            break;
                        }
                        //CTF
                        case 2:
                        {
                            AddAndClearLevels(ref removedLevelsCTF, ref select);
                            Util.AddLevels("CTF/");
                            break;
                        }
                        //CP
                        case 3:
                        {
                            AddAndClearLevels(ref removedLevelsCP, ref select);
                            Util.AddLevels("CP");
                            break;
                        }
                        //C
                        case 4:
                        {
                            AddAndClearLevels(ref removedLevelsC, ref select);
                            Util.AddLevels("C/");
                            break;
                        }
                        //Theft
                        case 5:
                        {
                            AddAndClearLevels(ref removedLevelsTheft, ref select);
                            Util.AddLevels("Theft/");
                            break;
                        }
                    }
                }

                if (selectedGamemode != 1) RemoveLevels(FuseLevels);
                if (selectedGamemode != 2) RemoveLevels(CTFLevels);
                if (selectedGamemode != 3) RemoveLevels(CPLevels);
                if (selectedGamemode != 4) RemoveLevels(CLevels);
                if (selectedGamemode != 5) RemoveLevels(TheftLevels);*/
            }
            else
            {
                if (didClear) return;
                didClear = true;
                FuseLevels.Clear();
                CTFLevels.Clear();
                CPLevels.Clear();
                CLevels.Clear();
                TheftLevels.Clear();
            }
        }

        private static Gamemode ContainsGamemodeSpecific(List<BinaryClassChunk> chunks)
        {
            foreach (BinaryClassChunk chunk in chunks)
            {
                string typeString = (string)chunk.GetProperty("type");
                if (typeString != null && typeString.Contains("DuckGame.C44P.GM_Fuse"))
                    return Gamemode.Fuse;
                if (typeString != null && typeString.Contains("DuckGame.C44P.GM_CTF"))
                    return Gamemode.CTF;
                if (typeString != null && typeString.Contains("DuckGame.C44P.GM_CP"))
                    return Gamemode.CP;
                if (typeString != null && typeString.Contains("DuckGame.C44P.GM_Collection"))
                    return Gamemode.C;
                if (typeString != null && typeString.Contains("DuckGame.C44P.GM_STOLEN"))
                    return Gamemode.Theft;
            }
            return Gamemode.Vanilla;
        }

        private static void RemoveLevels(List<string> levels)
        {
            foreach (string t in levels)
                DuckNetwork.core._activatedLevels.Remove(t);
        }

        private static void AddAndClearLevels(ref List<LSItem> levels, ref LevelSelect select)
        {
            foreach (LSItem item in levels)
                select.AddItem(item);
            levels.Clear();
        }

        public void OnDrawLayer(Layer pLayer) { }
    }
}
