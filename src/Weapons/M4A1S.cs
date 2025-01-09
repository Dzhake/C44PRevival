namespace DuckGame.C44P;

[EditorGroup("ADGM|Guns")]
public class M4A1S : Gun
{
    public M4A1S(float xval, float yval) : base(xval, yval)
    {
        ammo = 20;
        _ammoType = new ATHighCalMachinegun()
        {
            range = 250f
        };
        _type = "gun";
        _graphic = new Sprite($"{C44P.WeaponsPath}M4A1S");
        _center = new Vec2(18f, 17f);
        _collisionOffset = new Vec2(-16f, -7f);
        _collisionSize = new Vec2(27f, 12f);
        _barrelOffsetTL = new Vec2(38f, 14f);
        _holdOffset = new Vec2(3f, 2f);
        _fireSound = "deepMachineGun2";
        _fullAuto = true;
        _fireWait = 0.8f;
        _kickForce = 1.5f;
        _fireRumble = RumbleIntensity.Kick;
        loseAccuracy = 0.2f;
        maxAccuracyLost = 0.4f;
        _fireSoundPitch = 2f;
        _editorName = "M4A1-S";
        editorTooltip = "Accurate and fast machine gun with low ammo count.";
    }

    public override void PlayFireSound()
    {
        SFX.Play(_fireSound, 0.5f, -0.1f + Rando.Float(0.2f) + _fireSoundPitch);
    }
}
