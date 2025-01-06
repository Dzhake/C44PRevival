using System;

namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns")]
public class Negev : Gun
{
    protected SpriteMap _sprite;
    protected Sprite _tip;
    
    public Negev(float x, float y) : base(x, y)
    {
        ammo = 100;
        _ammoType = new ATNegev();
        wideBarrel = true;
        barrelInsertOffset = new Vec2(0f, 0f);
        _type = "gun";
        _sprite = new SpriteMap($"{C44P.WeaponsPath}Negev", 34, 14)
        {
            _frame = 0
        };
        _tip = new Sprite($"{C44P.WeaponsPath}NegevTip");
        _graphic = _sprite;
        _center = new Vec2(15f, 8f);
        _collisionOffset = new Vec2(-15f, -8f);
        _collisionSize = new Vec2(34f, 14f);
        _barrelOffsetTL = new Vec2(34f, 4f);
        _fireSound = "pistolFire";
        _fullAuto = true;
        _fireWait = 0.5f;
        _kickForce = 1f;
        _fireRumble = RumbleIntensity.Kick;
        _weight = 7f;
        _holdOffset = new Vec2(0f, 2f);
        editorTooltip = "Heavy, but accurate and fast machine gun.";
    }

    public override void Update()
    {
        base.Update();
        if (_wait <= 0f) _ammoType.accuracy = Maths.LerpTowards(_ammoType.accuracy, 0.1f, 0.01f);
    }

    public override void Fire()
    {
        if (_wait > 0f) return;
        base.Fire();
        if (ammo <= 0) return;

        _sprite._frame = ammo switch
        {
            <= 4 => 7 - ammo,
            _ => 1 - _sprite._frame
        };
        _ammoType.accuracy = Math.Min(_ammoType.accuracy + 0.05f, 0.8f);
        _weight = Math.Max(weight - 0.02f, 4.95f);
    }

    public override void Draw()
    {
        Material mat = Graphics.material;
        base.Draw();
        Graphics.material = material;
        _tip.flipH = graphic.flipH;
        _tip.center = graphic.center;
        _tip.depth = depth + 1;
        DevConsole.Log(((_ammoType.accuracy - 0.1f) * 1.42f).ToString());
        _tip.alpha = (_ammoType.accuracy - 0.1f) * 1.42f; // 10/7
        _tip.angle = angle;
        Graphics.Draw(_tip, x, y);
        Graphics.material = mat;
    }

    protected class ATNegev : AmmoType
    {
        public ATNegev()
        {
            range = 250f;
            accuracy = 0.1f;
            penetration = 1f;
            combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            Level.Add(new PistolShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f)),
            });
        }
    }
}
