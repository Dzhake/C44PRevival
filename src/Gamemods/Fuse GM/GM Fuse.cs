using System.Linq;

namespace DuckGame.C44P;

[EditorGroup("ADGM|GM Fuse")]
[BaggedProperty("canSpawn", false)]
public class GM_Fuse : Thing
{
    public GMTimer? _timer;
    public C4? c4;

    public float time = 90f;

    public EditorProperty<float> RoundTime;
    public EditorProperty<float> ExplosionTime;
    public EditorProperty<bool> PlantZones;

    public StateBinding timeBinding = new("time");
    public StateBinding c4Binding = new("c4");
    protected bool _inited;

    public NetSoundEffect TenSecSound;

    public GM_Fuse(float xval, float yval) : base(xval, yval)
    {
        FuseTeams.Reset();
        TenSecSound = new NetSoundEffect($"{C44P.Soundspath}10sec");
        _graphic = new($"{C44P.SpritesPath}Gamemodes/GameMode");
        _center = new Vec2(8f, 8f);
        _collisionOffset = new Vec2(-7f, -7f);
        _collisionSize = new Vec2(14f, 14f);
        _visibleInGame = false;
        layer = Layer.Foreground;
        RoundTime = new(90f, this, 20f, 180f, 1f);
        ExplosionTime = new(25f, this, 10f, 60f, 5f);
        PlantZones = new(true) { _tooltip = "Will C4 require being placed in 'plantzone' or not" };

        _editorName = "GM Fuse";
        editorTooltip = "Inactive bomb spawns instead of this block.";
        maxPlaceable = 1;
    }

    public override void Update()
    {
        base.Update();

        if (!_inited)
        {
            time = RoundTime;

            if (Level.current is Editor || !isServerForObject) return;

            _timer = new GMTimer(0, 0)
            {
                depth = 0.95f,
                time = 0
            };
            Level.Add(_timer);
            _inited = true;
        }

        if (isServerForObject && time >= 9.97 && time < 10)
            TenSecSound.Play();

        if (c4 == null || Level.current is Editor || _timer is null) return;

        switch (time)
        {
            case > 0f:
                time -= Maths.IncFrameTimer();
                _timer.time = time;
                if (c4.State == C4.BombState.Planted && time % 1 > 0.02f && time % 1 < 0.05f)
                    c4.boopBeepSound.Play();
                break;
            case <= 0f:
                if (c4.State == C4.BombState.Planted) c4.Explode();
                Win(c4.State == C4.BombState.Planted ? FuseTeams.FuseTeam.T : FuseTeams.FuseTeam.CT);
                Level.Remove(_timer);
                Fondle(_timer);
                break;
        }
    }

    public void OnPlant()
    {
        if (_timer != null) _timer.subtext = "Planted";
    }

    public void OnDefuse()
    {
        Win(FuseTeams.FuseTeam.CT);
    }

    public void Win(FuseTeams.FuseTeam team)
    {
        if (!isServerForObject) return;
        foreach (Duck d in FuseTeams.DucksNotInTeam(team))
            d.Kill(new DTImpact(this));
    }


    public override void Draw()
    {
        base.Draw();
        if (Level.current is not Editor) return;

        bool CTArmorExists = false;
        bool TArmorExists = false;
        bool C4Placed =  Level.First<C4>() != null;

        foreach (Equipper equipper in Level.current.things[typeof(Equipper)])
            switch (equipper.GetContainedInstance())
            {
                case CTArmor:
                    CTArmorExists = true;
                    break;
                case TArmor:
                    TArmorExists = true;
                    break;
            }

        int row = 0;
        const int yoffset = 16;
        const int spriteYOffset = 4;
        const int xoffset = 14;
        const int yMove = 10;


        float unit = Level.current.camera.size.x / 320 * 0.4f;

        GMIcons.Warn.scale = GMIcons.On.scale = GMIcons.Off.scale = new Vec2(unit, unit) * 0.5f;
        Sprite on = GMIcons.On;
        Sprite off = GMIcons.Off;
        //Sprite warn = GMIcons.Warn;

        string text = "CT Armor equipper";
        Graphics.DrawString(text, Level.current.camera.position + new Vec2(xoffset, yoffset) * unit, Color.White, depth, null, unit);
        Graphics.Draw(CTArmorExists ? on : off, Level.current.camera.position.x + xoffset * 0.5f * unit,
            Level.current.camera.position.y + (yoffset + spriteYOffset) * unit);

        row++;
        text = "T Armor equipper";
        Graphics.DrawString(text, Level.current.camera.position + new Vec2(xoffset, yMove + yoffset) * unit, Color.White, depth, null, unit);

        Graphics.Draw(TArmorExists ? on : off, Level.current.camera.position.x + xoffset * 0.5f * unit,
            Level.current.camera.position.y + (yMove + yoffset + spriteYOffset) * unit);

        row++;
        text = "C4 placed";
        Graphics.DrawString(text, Level.current.camera.position + new Vec2(xoffset, row * yMove + yoffset) * unit, Color.White, depth, null, unit);
        Graphics.Draw(C4Placed ? on : off, Level.current.camera.position.x + xoffset * 0.5f * unit, Level.current.camera.position.y + (row * yMove + yoffset + spriteYOffset) * unit);


        if (PlantZones)
        {
            bool isTherePlantZones = Level.First<PlantZone>() != null;

            row++;
            text = "Plant zone setted up";
            Graphics.DrawString(text, Level.current.camera.position + new Vec2(xoffset, row * yMove + yoffset) * unit, Color.White, depth, null, unit);

            Graphics.Draw(isTherePlantZones ? on : off, Level.current.camera.position.x + xoffset * 0.5f * unit,
                Level.current.camera.position.y + (row * yMove + yoffset + spriteYOffset) * unit);
        }

        /*if (!Level.current.things[typeof(Defuser)].Any())
        {
            row++;
            text = "Defuser not placed";
            Graphics.DrawString(text, Level.current.camera.position + new Vec2(xoffset, row * yMove + yoffset) * unit, Color.White, depth, null, unit);
            Graphics.Draw(warn, Level.current.camera.position.x + xoffset * 0.5f * unit, Level.current.camera.position.y + (row * yMove + yoffset + spriteYOffset) * unit);
        }*/
    }
}

