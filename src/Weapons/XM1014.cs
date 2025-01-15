namespace DuckGame.C44P
{
	[EditorGroup("ADGM|Guns")]
	public class XM1014 : Gun
	{
        protected string fileName = "XM1014";
        protected int frameWidth = 29;
        protected int frameHeight = 10;

		public XM1014(float xval, float yval) : base(xval, yval)
		{
			ammo = 7;
			_fullAuto = true;
			_ammoType = new ATShotgun();
			wideBarrel = true;
			_type = "gun";
			_graphic = new Sprite($"{C44P.WeaponsPath}{fileName}");
			_center = new Vec2(16f, 16f);
			_collisionOffset = new Vec2(-8f, -3f);
			_collisionSize = new Vec2(16f, 6f);
			_barrelOffsetTL = new Vec2(30f, 14f);
			_fireSound = "shotgunFire2";
			_kickForce = 3f;
			_fireRumble = RumbleIntensity.Light;
			_numBulletsPerFire = 6;
            _fireWait = 2;
			_holdOffset = new Vec2(3f, 1f);
            editorTooltip = "Very fast automatic shotgun. You can spam attack to shoot faster.";
        }
	}
}
