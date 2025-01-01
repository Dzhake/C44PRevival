using System;

namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns")]
public class AWP : Sniper
{
    protected string fileName = "awp";
    protected int frameWidth = 40;
    protected int frameHeight = 10;

    public Vec2 normalBarrelOffsetTL = new(37f, 3f);
    public Vec2 shortBarrelOffsetTL = new(20f, 5f);

    public AWP(float xval, float yval) : base(xval, yval)
    {
        ammo = 5;
        _ammoType = new ATHighCalSniper();
        _graphic = new Sprite(GetPath($"{C44P.WeaponsPath}{fileName}"));
        _center = new Vec2(16f, 4f);
        _collisionOffset = new Vec2(-8f, -4f);
        _collisionSize = new Vec2(16f, 8f);
        _barrelOffsetTL = new Vec2(37f, 3f);
        _fireSoundPitch = -0.9f;
        _kickForce = 2f;
        _fireRumble = RumbleIntensity.Light;
        laserSight = true;
        _laserOffsetTL = new Vec2(37f, 4f);
        _manualLoad = true;

        _holdOffset = new Vec2(2f, -2f);

        _editorName = nameof(AWP);
        editorTooltip = "Sniper which automatically locks on target";
    }

    public override void Update()
    {
        base.Update();
        Util.TryReskin(this, fileName, frameWidth, frameHeight);
        if (!loaded || owner == null || _loadState != -1) return;

        Duck nearestTarget = null;
        foreach (Duck d in Level.current.things[typeof(Duck)])
        {
            if (((d.position.x <= barrelPosition.x || offDir <= 0) &&
                 (d.position.x >= barrelPosition.x || offDir >= 0))
                || Level.CheckLine<Block>(d.position, barrelPosition) != null ||
                Level.CheckLine<Safezone>(d.position, barrelPosition) != null) continue;

            if (nearestTarget == null || (barrelPosition - d.position).length < (barrelPosition - nearestTarget.position).length)
                nearestTarget = d;
        }

        if (nearestTarget == null) return;

        float aimAngle = (float)Math.Atan2(nearestTarget.position.y - barrelPosition.y, (nearestTarget.position.x - barrelPosition.x) * offDir);
        aimAngle *= offDir;
        if (aimAngle < Math.PI * 0.05f && aimAngle > Math.PI * -0.05f)
            angle = aimAngle * 0.8f;
    }

    public override void Fire()
    {
        CheckBarrelOffset();
        base.Fire();
    }

    public override void OnPressAction()
    {
        if (loaded)
        {
            CheckBarrelOffset();
            base.OnPressAction();
            return;
        }

        if (ammo <= 0 || _loadState != -1) return;

        _loadState = 0;
        _loadAnimation = 0;
    }

    public override void Draw()
    {
        float ang = angle;
        angle -= _angleOffset * offDir;
        base.Draw();
        angle = ang;
    }

    protected void CheckBarrelOffset()
    {
        if (Level.CheckLine<Safezone>(position, barrelPosition) != null || Level.CheckLine<Block>(position, barrelPosition) != null)
            _barrelOffsetTL = shortBarrelOffsetTL;
        else
            _barrelOffsetTL = normalBarrelOffsetTL;
    }
}

