﻿namespace DuckGame.C44P
{
	[EditorGroup("ADGM|Guns")]
	[BaggedProperty("isInDemo", true)]
	public class MagnumOpus : Gun
	{
        protected string fileName = "MagnumOpus";
        protected int frameWidth = 32;
        protected int frameHeight = 32;

		public float rise;
		public float _angleOffset;

		float burst;

		public StateBinding _angleOffsetBinding = new("_angleOffset");
		public StateBinding _riseBinding = new("rise");
		public MagnumOpus(float xval, float yval) : base(xval, yval)
		{
			ammo = 6;
			_ammoType = new ATMagnum();
			_type = "gun";
			graphic = new Sprite(GetPath($"{C44P.WeaponsPath}{fileName}"), 0f, 0f);
			center = new Vec2(16f, 16f);

			collisionOffset = new Vec2(-8f, -6f);
			collisionSize = new Vec2(16f, 10f);
			_barrelOffsetTL = new Vec2(25f, 12f);

			_fireSound = "magnum";
			_kickForce = 3f;
			_fireRumble = RumbleIntensity.Light;

			_holdOffset = new Vec2(3f, 0f);
			//handOffset = new Vec2(0f, 1f);

			_fireWait = 0.0f;

			_editorName = "Magnum Opus";
		}

		public override void Update()
		{
			base.Update();
            Util.TryReskin(this, fileName, frameWidth, frameHeight);
			if (owner != null)
			{
				if (offDir < 0)
				{
					_angleOffset = -Maths.DegToRad(-rise * 65f);
				}
				else
				{
					_angleOffset = -Maths.DegToRad(rise * 65f);
				}
			}
			else
			{
				_angleOffset = 0f;
			}
			if (rise > 0f)
			{
				rise -= 0.013f;
			}
			else
			{
				rise = 0f;
			}
			if (_raised)
			{
				_angleOffset = 0f;
			}

			if(ammo == 0)
            {
				_fullAuto = false;
            }
		}
		public override float angle
		{
			get
			{
				return base.angle + _angleOffset;
			}
			set
			{
				_angle = value;
			}
		}
		public override void Fire()
        {
            base.Fire();
			if (ammo > 0 && rise < 1f)
			{
				rise += 0.4f;
			}
		}
        public override void OnPressAction()
        {
			Fire();
        }
        public override void OnHoldAction()
        {
            base.OnHoldAction();
			burst += 0.04f;
			if(burst >= 1 && ammo > 0)
            {
				Fire();
            }
        }
        public override void OnReleaseAction()
        {
            base.OnReleaseAction();
			burst = 0;
        }
    }
}
