namespace DuckGame.C44P
{
	[EditorGroup("ADGM|Guns")]
	[BaggedProperty("isInDemo", true)]
	public class RevolverR8 : Gun
	{
        protected string fileName = "RevolverR8";
        protected int frameWidth = 19;
        protected int frameHeight = 10;

        protected SpriteMap _sprite;

		public float rise;
		public StateBinding riseBinding = new("rise");
        
        public int charge;
        public StateBinding chargeBinding = new("charge");

        public static readonly int chargeAt = 30;
        public bool shouldRise = true;

        public override float angle
        {
            get
            {
                return base.angle - Maths.DegToRad((offDir < 0 ? -rise : rise) * 65f);
            }
            set
            {
                _angle = value;
            }
        }

		public RevolverR8(float xval, float yval) : base(xval, yval)
		{
			ammo = 8;
			_ammoType = new ATRevolver();
			_type = "gun";
            _sprite = new SpriteMap($"{C44P.WeaponsPath}{fileName}", frameWidth, frameHeight);
			_graphic = _sprite;
			_center = new Vec2(9,6);
			_collisionOffset = new Vec2(-9, -6);
			_collisionSize = new Vec2(19, 10);
			_barrelOffsetTL = new Vec2(19, 2);
			_fireSound = "magnum";
			_kickForce = 5f;
			_fireRumble = RumbleIntensity.Light;
			_holdOffset = new Vec2(0, 1);
			_editorName = "Revolver R8";
            editorTooltip = "The R8 Revolver is highly accurate and powerful at the expense of a lengthy trigger-pull.";
            _fireWait = 2.25f; //15 frames..?
        }

		public override void Update()
		{
			base.Update();

            if (owner == null)
            {
                charge = 0;
                rise = 0f;
            }

            if (shouldRise) rise = _wait > 0 ? Maths.LerpTowards(rise, 1f, 0.08f)
                : Maths.LerpTowards(rise, 0f, 0.04f);
            if (!_triggerHeld && charge > 0) charge--;
            _sprite.frame = charge >= chargeAt / 2 ? 1 : 0;
        }

        public override void OnPressAction() {}

        public override void OnHoldAction()
        {
            if (_wait > 0) return;
            charge++;
            if (charge < chargeAt) return;
            shouldRise = ammo > 0;
            Fire();
            charge = 0;
        }

        protected class ATRevolver : AmmoType
        {
            public ATRevolver()
            {
                accuracy = 0.98f; //up to 0.3 degrees
                range = 450f;
                penetration = 6f;
                bulletSpeed = 48f;
                combustable = true;
                impactPower = 4f;
            }

            public override void PopShell(float x, float y, int dir)
            {
                Level.Add(new MagnumShell(x, y)
                {
                    hSpeed = dir * (1.5f + Rando.Float(1f))
                });
            }
        }
    }
}
