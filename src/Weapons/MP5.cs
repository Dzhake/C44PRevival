using System;

namespace DuckGame.C44P
{
    [EditorGroup("ADGM|Guns")]
    public class MP5 : Gun
    {
        protected string fileName = "MP5";
        protected int frameWidth = 20;
        protected int frameHeight = 10;

		int burst;
        public MP5(float xval, float yval) : base(xval, yval)
        {
			ammo = 30;
			_ammoType = new AT9mm();
			_ammoType.range = 170f;
			_ammoType.accuracy = 0.9f;
			_type = "gun";
			_graphic = new Sprite($"{C44P.WeaponsPath}{fileName}");
			_center = new Vec2(10f, 5f);
			_collisionOffset = new Vec2(-8f, -4f);
			_collisionSize = new Vec2(16f, 8f);
			_barrelOffsetTL = new Vec2(9f, 2f);
			_fireSound = "smg";
			_fullAuto = true;
			_fireWait = 0.4f;
			_fireSoundPitch = -0.4f;
			_kickForce = 1f;
			_fireRumble = RumbleIntensity.Kick;
			_holdOffset = new Vec2(-1f, 0f);
			loseAccuracy = 0.02f;
			maxAccuracyLost = 0.7f;
			editorTooltip = "Semi-auto pistol which is better with short bursting";
		}

        public override void Fire()
        {
            base.Fire();
			burst++;
			if(burst > 6)
            {
				burst = 6;
            }
			loseAccuracy = 0.02f + 0.03f * burst;
			_ammoType.accuracy = 0.9f - 0.05f * burst;
			_ammoType.range = 170f - 10 * burst;
			_fireWait = 0.4f + 0.1f * burst;
			_fireSoundPitch = -0.4f + 0.04f * burst;
		}
        public override void OnReleaseAction()
        {
            base.OnReleaseAction();
			loseAccuracy = 0.02f; 
			_ammoType.accuracy = 0.9f;
			_ammoType.range = 170f;
			burst = 0; 
			_fireWait = 0.4f;
			_fireSoundPitch = -0.4f;
		}
    }
}
