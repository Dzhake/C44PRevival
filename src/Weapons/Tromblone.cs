namespace DuckGame.C44P
{
    [EditorGroup("ADGM|Guns")]
    public class Tromblone : TampingWeapon
    {
        protected string fileName = "Tromblone";
        protected int frameWidth = 29;
        protected int frameHeight = 10;

        public Tromblone(float xval, float yval) : base(xval, yval)
        {
            wideBarrel = true;
            ammo = 99;
            _ammoType = new ATPellet
            {
                accuracy = 0.01f,
                penetration = 0.4f
            };
            _numBulletsPerFire = 4;
            _type = "gun";
            _graphic = new Sprite($"{C44P.WeaponsPath}{fileName}");
            _center = new Vec2(19f, 5f);
            _collisionOffset = new Vec2(-8f, -3f);
            _collisionSize = new Vec2(16f, 7f);
            _barrelOffsetTL = new Vec2(34f, 4f);
            _fireSound = "shotgun";
            _kickForce = 2f;
            _fireSoundPitch = 0.6f;

            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(10f, -1f);
            editorTooltip = "Old-timey shotgun, takes approximately 150 years to reload.";
        }

        public override void Update()
        {
            base.Update();
            _canRaise = _tamped;
        }

        public override void OnPressAction()
        {
            if (_tamped)
            {
                base.OnPressAction();
                for (int i = 0; i < 14; i++)
                {
                    MusketSmoke smoke = new MusketSmoke(barrelPosition.x - 16f + Rando.Float(32f),
                        barrelPosition.y - 16f + Rando.Float(32f))
                    {
                        depth = 0.9f + i * 0.001f
                    };
                    switch (i)
                    {
                        case < 6:
                            smoke.move -= barrelVector * Rando.Float(0.1f);
                            break;
                        case < 10:
                            smoke.fly += barrelVector * (2f + Rando.Float(7.8f));
                            break;
                    }

                    _tampBoost += 0.05f;
                    Level.Add(smoke);
                }

                _tampInc = 0f;
                _tampTime = infinite.value ? 0.1f : 0.5f;
                _tamped = false;
                return;
            }

            if (_raised || owner is not Duck duckOwner) return;
            duckOwner.sliding = false;
            _rotating = true;
        }
    }
}
