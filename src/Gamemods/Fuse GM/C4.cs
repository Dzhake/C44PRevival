using System;

namespace DuckGame.C44P;

[EditorGroup("ADGM|GM Fuse")]
[BaggedProperty("canSpawn", false)]
public class C4 : Holdable, IDrawToDifferentLayers
{
    public enum BombState { Spawned, Planted, Defused, Exploded }
    protected enum ActionIcon { None, Down, Shoot }

    protected ActionIcon icon;

    public NetSoundEffect boopBeepSound;
    public NetSoundEffect bombdefusedSound;
    public NetSoundEffect bombplantedSound;
    public NetSoundEffect DefuseSound;

    public bool ZoneOnly;
    public GM_Fuse? GM;

    public BombState State = BombState.Spawned;
    public float ActionTimer;
    public StateBinding ActionTimerBinding = new("ActionTimer");

    protected Vec2 respawnPos;

    public C4(float xval, float yval) : base(xval, yval)
    {
        boopBeepSound = new NetSoundEffect($"{C44P.Soundspath}boopbeep");
        bombdefusedSound = new NetSoundEffect($"{C44P.Soundspath}bombdefused");
        bombplantedSound = new NetSoundEffect($"{C44P.Soundspath}bombplanted");
        DefuseSound = new NetSoundEffect($"{C44P.Soundspath}Defuse");

        _weight = 1f;
        tapeable = false;
        _collisionSize = new Vec2(8f, 10f);
        _collisionOffset = new Vec2(-4f, -6f);
        _center = new Vec2(8f, 6f);

        _graphic = new($"{C44P.SpritesPath}Gamemodes/Fuse/C4");

        respawnPos = new(xval, yval);
    }

    public override void Update()
    {
        if (GM is null)
        {
            GM = Level.First<GM_Fuse>();
            if (GM is null)
            {
                Level.Remove(this);
                return;
            }
            GM.c4 = this;
            ZoneOnly = GM.PlantZones;
        }

        if (position.y > Level.current.lowestPoint + 400f && State == BombState.Spawned)
            position = respawnPos;

        switch (State)
        {
        case BombState.Spawned:
        {
            SpawnedLogic();
            break;
        }
        case BombState.Planted:
        {
            PlantedLogic();
            break;
        }
        }

        if (removedFromFall && isServerForObject)
            State = BombState.Defused;

        base.Update();
    }

    protected void SpawnedLogic()
    {
        icon = ActionIcon.None;
        canPickUp = true;
        if (duck is null || GM is null) return;
        Duck d = duck;

        if (!d.grounded || (ZoneOnly && Level.CheckRect<PlantZone>(topLeft, bottomRight) == null))
        {
            ActionTimer = 0;
            return;
        }

        if (!d.crouch)
        {
            ActionTimer = 0;
            icon = ActionIcon.Down;
            return;
        }

        icon = ActionIcon.Shoot;

        if (!d.inputProfile.Down("SHOOT"))
        {
            ActionTimer = 0;
            return;
        }

        d._disarmDisable = 5;
        d.hSpeed = 0;
        ActionTimer += Maths.IncFrameTimer();
        if (isServerForObject && ActionTimer % 0.3 > 0.02 && ActionTimer % 0.3 < 0.05)
        {
            boopBeepSound.Play();
            Level.Add(new PlantingButtonGraphic(x, y - 16f));
            d._disarmDisable = 0;
        }

        if (ActionTimer >= 2f) OnPlant();
    }

    protected void PlantedLogic()
    {
        icon = ActionIcon.None;
        bool defusing = false;
        bool defuser = false;

        if (Level.current is DeathmatchLevel dml)
        {
            Deathmatch? dm = dml._deathmatch;
            if (dm != null)
            {
                dm._matchOver = false;
                dm._deadTimer = 1f;
            }
        }

        foreach (Duck d in Level.CheckRectAll<Duck>(new Vec2(position.x - 10f, position.y + 2f),
                     new Vec2(position.x + 10f, position.y - 6f)))
        {
            if (FuseTeams.Team(d) != FuseTeams.FuseTeam.CT || !d.grounded) continue;
            bool hasDefuser = d.holdObject is Defuser;
            if (!hasDefuser && d.holdObject is not null) continue;
            icon = ActionIcon.Shoot;
            if (!d.inputProfile.Down("SHOOT")) continue;

            defusing = true;
            defuser = hasDefuser;

            if (Network.isActive && isServerForObject)
                Fondle(this, d.connection);

            d.hSpeed = 0;
            break;
        }

        if (!defusing)
        {
            ActionTimer = 0;
            return;
        }

        ActionTimer += Maths.IncFrameTimer() * (defuser ? 2 : 1);

        if (ActionTimer % 0.7 > 0.02 && ActionTimer % 0.7 < 0.05 && ActionTimer < 5)
            if (isServerForObject) DefuseSound.Play();

        if (ActionTimer < 5) return;
        OnDefuse();
    }

    public void OnPlant()
    {
        GM?.OnPlant();
        State = BombState.Planted;
        ActionTimer = 0f;

        if (duck is not null) duck.doThrow = true;
        canPickUp = false;
        angleDegrees = 0f;
        if (isServerForObject) bombplantedSound.Play();

        if (GM == null) return;
        GM.time = GM.ExplosionTime;
    }

    public void OnDefuse()
    {
        icon = ActionIcon.None;
        State = BombState.Defused;
        if (isServerForObject) bombdefusedSound.Play();
        GM?.OnDefuse();
        ActionTimer = 0;
    }

    public virtual void Explode()
    {
        State = BombState.Exploded;
        foreach (Duck d in Level.CheckCircleAll<Duck>(position, 160f))
            d.Kill(new DTImpact(this));

        Graphics.FlashScreen();

        Level.Remove(this);

        if (!isServerForObject) return;

        Level.Add(new ExplosionPart(position.x, position.y));
        int num = 6;

        for (int i = 0; i < num; i++)
        {
            float dir = i * 60f + Rando.Float(-10f, 10f);
            float dist = Rando.Float(20f, 20f);
            ExplosionPart ins = new(position.x + (float)(Math.Cos(Maths.DegToRad(dir)) * dist), position.y -
                (float)(Math.Sin(Maths.DegToRad(dir)) * dist));
            Level.Add(ins);
        }
    }

    public void OnDrawLayer(Layer l)
    {
        if (l != Layer.Foreground) return;

        if (icon != ActionIcon.None)
            Graphics.DrawString(icon == ActionIcon.Shoot ? "@SHOOT@" : "@DOWN@", position + new Vec2(-6, -36), Color.White);

        if (ActionTimer <= 0 || State != BombState.Planted) return;
        Vec2 pos = new(position.x, position.y - 6f);
        Graphics.DrawCircle(pos, 25, Color.Black, 3f, depth.value - 0.1f, 50);
        Util.DrawCircle(pos, 25, Color.LightGreen, 2f, depth, 50, (int)(ActionTimer * 10));
    }
}

