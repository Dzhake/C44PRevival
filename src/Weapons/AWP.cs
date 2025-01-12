namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns")]
public class AWP : Sniper
{
    protected float aimAngle;

    public override float angle
    {
        get =>
            owner != null && aimAngle != 0
                ? Maths.DegToRad(aimAngle - (offDir < 0 ? 180 : 0))
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
        aimAngle = 0;
        if (!loaded || _loadState != -1 || owner is not Duck || duck.hSpeed + duck.vSpeed > 0.1f || !duck.grounded) return;
        Duck? target = GetTarget();
        if (target is null) return;
        aimAngle = -Maths.PointDirection(barrelOffset, target.position + new Vec2(0f, 3f));
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
        Duck? d = null;
        float dist = float.MaxValue;
        foreach (Thing t in Level.current.things[typeof(IAmADuck)])
        {
            if ((t.x < barrelOffset.x && offDir > 0) || (t.x > barrelOffset.x && offDir < 0) || Level.CheckLine<Block>(position, t.position) != null
                || t == owner || !(Distance(t) <= _ammoType.range)) continue;

            float curDist = (position - t.position).lengthSq;
            if (curDist < dist)
            {
                dist = curDist;
                if (Duck.GetAssociatedDuck(t) != null && !Duck.GetAssociatedDuck(t).dead && (duck == null || Duck.GetAssociatedDuck(t).team != duck.team))
                    d = Duck.GetAssociatedDuck(t);
            }
        }
        return d;
    }
}

