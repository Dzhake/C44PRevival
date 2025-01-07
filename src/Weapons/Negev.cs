using System;

namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns")]
public class Negev : Gun
{
    protected Sprite _tipSprite;
    protected SpriteMap _ammoTopSprite;
    protected SpriteMap _ammoSprite;
    
    public Negev(float x, float y) : base(x, y)
    {
        ammo = 100;
        _ammoType = new ATNegev();
        wideBarrel = true;
        barrelInsertOffset = new Vec2(0f, 0f);
        _type = "gun";
        string rootSpritesPath = $"{C44P.WeaponsPath}Negev";
        _tipSprite = new Sprite($"{rootSpritesPath}Tip");
        _graphic = new Sprite(rootSpritesPath);
        _ammoSprite = new SpriteMap($"{rootSpritesPath}Ammo", 3, 5);
        _ammoTopSprite = new SpriteMap($"{rootSpritesPath}AmmoTop", 2, 1);
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
        if (!_triggerHeld) _ammoType.accuracy = Maths.LerpTowards(_ammoType.accuracy, 0.1f, 0.01f);
    }

    public override void Fire()
    {
        if (_wait > 0f) return;
        base.Fire();
        if (ammo <= 0) return;
        _ammoTopSprite.frame = 1 - _ammoTopSprite.frame;
        _ammoSprite.frame = ammo switch
        {
            <= 4 => 6 - ammo,
            _ => 1 - _ammoSprite.frame
        };
        _ammoType.accuracy = Math.Min(_ammoType.accuracy + 0.05f, 0.8f);
        _weight = Math.Max(weight - 0.02f, 4.95f);
    }

    public override void Draw()
    {
        Material mat = Graphics.material;
        base.Draw();
        Graphics.material = material;
        Depth tempDepth = depth + 1;

        _tipSprite.flipH = graphic.flipH;
        _tipSprite.center = graphic.center + new Vec2(-28, -3);
        _tipSprite.depth = tempDepth;
        _tipSprite.alpha = (_ammoType.accuracy - 0.1f) * 1.42f; // 10/7
        _tipSprite.angle = angle;
        Graphics.Draw(_tipSprite, x, y);

        _ammoTopSprite.flipH = graphic.flipH;
        _ammoTopSprite.center = graphic.center + new Vec2(-14, -4);
        _ammoTopSprite.depth = tempDepth;
        _ammoTopSprite.angle = angle;
        Graphics.Draw(_ammoTopSprite, x, y);

        _ammoSprite.flipH = graphic.flipH;
        _ammoSprite.center = graphic.center + new Vec2(-14, -5);
        _ammoSprite.depth = tempDepth;
        _ammoSprite.angle = angle;
        Graphics.Draw(_ammoSprite, x, y);

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
