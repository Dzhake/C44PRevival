using System;

namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns")]
public class AWP : Sniper
{
    protected float AimAngle;
    public StateBinding AimAngleBinding = new("AimAngle");

    public override float angle
    {
        get =>
            owner != null && AimAngle != 0
                ? Maths.DegToRad(AimAngle - (offDir < 0 ? 180 : 0))
                : base.angle;
        set => base.angle = value;
    }

    public AWP(float xval, float yval) : base(xval, yval)
    {
        ammo = 5;
        _ammoType = new ATHighCalSniper();
        _graphic = new Sprite($"{C44P.WeaponsPath}awp");
        _center = new Vec2(16f, 4f);
        _collisionOffset = new Vec2(-8f, -4f);
        _collisionSize = new Vec2(16f, 8f);
        _barrelOffsetTL = new Vec2(37f, 3f);
        _fireSoundPitch = -0.9f;
        _kickForce = 7f;
        _fireRumble = RumbleIntensity.Light;
        laserSight = true;
        _laserOffsetTL = new Vec2(40f, 4.5f);
        _manualLoad = true;

        _holdOffset = new Vec2(2f, -2f);

        _editorName = nameof(AWP);
        editorTooltip = "Sniper which automatically locks on target";
    }

    public override void Update()
    {
        base.Update();
        if (Network.isActive && !isServerForObject) return;
        AimAngle = 0;
        if (!loaded || _loadState != -1 || duck is null || Math.Abs(duck.hSpeed) + Math.Abs(duck.vSpeed) > 0.01f || !duck.grounded) return;
        Duck? target = GetTarget();
        if (target is null) return;
        float aimAngle = -Maths.PointDirection(position, target.position + new Vec2(0f, 3f));
        if (Math.Abs(Math.Abs(Math.Abs(aimAngle) - 90) - 90) <= 5) AimAngle = aimAngle; //enjoy this unreadable but a bit more optimized thingie :)
    }

    public override void Draw()
    {
        base.Draw();
        laserSight = loaded && duck != null && Math.Abs(duck.hSpeed) + Math.Abs(duck.vSpeed) <= 0.01f;
    }

    public override void OnPressAction()
    {
        if (loaded)
        {
            base.OnPressAction();
            return;
        }

        if (ammo <= 0 || _loadState != -1) return;

        _loadState = 0;
        _loadAnimation = 0;
    }

    public Duck? GetTarget()
    {
        if (duck is null) return null;
        Duck? d = null;
        float dist = float.MaxValue;
        foreach (Thing t in Level.current.things[typeof(IAmADuck)])
        {
            if ((t.x < x && offDir > 0) || (t.x > x && offDir < 0) || Level.CheckLine<Block>(position, t.position) is not null || t == owner) continue;

            float curDist = (position - t.position).lengthSq;
            if (curDist >= dist) continue;

            Duck target = Duck.GetAssociatedDuck(t);

            if (target is null || target.dead || (FuseTeams.Team(target) != FuseTeams.FuseTeam.None && FuseTeams.Team(target) == FuseTeams.Team(duck)) || duck.team == target.team) continue;
            d = target;
            dist = curDist;
        }
        return d;
    }
}

