namespace DuckGame.C44P
{
	[EditorGroup("ADGM|Guns")]
	public class FlintlockPistol : Gun
	{
        protected string fileName = "FlintlockPistol";
        protected int frameWidth = 20;
        protected int frameHeight = 9;

		public int _loadState = -1;
        public StateBinding _loadStateBinding = new ("_loadState");
		public float _angleOffset;
		private SpriteMap _sprite;

		public FlintlockPistol(float xval, float yval) : base(xval, yval)
		{
			ammo = 2;
            _ammoType = new ATPellet();
			_type = "gun";
			_sprite = new SpriteMap($"{C44P.WeaponsPath}{fileName}", 20, 9);
			_graphic = _sprite;
			_center = new Vec2(10, 5);
			_collisionOffset = new Vec2(-8, -4);
			_collisionSize = new Vec2(16, 8);
			_barrelOffsetTL = new Vec2(20, 3);
			_fireSound = "shotgun";
			_fireSoundPitch = 0.4f;

			_kickForce = 2f;
			_fireRumble = RumbleIntensity.Kick;
			_manualLoad = true;
			_fullAuto = true;
			_holdOffset = new Vec2(2f, 0f);
			editorTooltip = "Old but gold pistol.";
		}
		public override void Update()
		{
			base.Update();
			_sprite.frame = ammo > 1 ? 0 : 1;
			if (!loaded && _loadState == -1)
			{
				_loadState = 0;
			}
			if (infinite.value) UpdateLoadState();
			UpdateLoadState(); 
		}
		private void UpdateLoadState()
        {
            if (_loadState <= -1) return;

            if (owner == null)
            {
                if (_loadState == 3)
                {
                    loaded = true;
                }
                _loadState = -1;
                _angleOffset = 0f;
                handOffset = Vec2.Zero;
            }
            switch (_loadState)
            {
                case 0:
                {
                    if (Network.isActive && isServerForObject)
                        NetSoundEffect.Play("oldPistolSwipe");
                    else
                        SFX.Play("swipe", 0.6f, -0.3f);
                    _loadState++;
                    return;
                }
                case 1 when _angleOffset < 0.16f:
                    _angleOffset = MathHelper.Lerp(_angleOffset, 0.2f, 0.08f);
                    return;
                case 1:
                    _loadState++;
                    return;
                case 2:
                {
                    handOffset.y -= 0.28f;
                    if (!(handOffset.y < -4f)) return;
                    _loadState++;
                    ammo = 2;
                    loaded = false;
                    if (!Network.isActive)
                    {
                        SFX.Play("shotgunLoad", 1f, 0f, 0f, false);
                        return;
                    }

                    if (!isServerForObject) return;
                    NetSoundEffect.Play("oldPistolLoad");
                    break;
                }
                case 3:
                {
                    handOffset.y += 0.15f;
                    if (!(handOffset.y >= 0f)) return;

                    _loadState++;
                    handOffset.y = 0f;
                    if (!Network.isActive)
                    {
                        SFX.Play("swipe", 0.7f);
                        return;
                    }

                    if (!isServerForObject) return;
                    NetSoundEffect.Play("oldPistolSwipe2");
                    break;
                }
                case 4 when _angleOffset > 0.04f:
                    _angleOffset = MathHelper.Lerp(_angleOffset, 0f, 0.08f);
                    return;
                case 4:
                {
                    _loadState = -1;
                    loaded = true;
                    _angleOffset = 0f;
                    if (isServerForObject && duck != null && duck.profile != null)
                    {
                        RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                    }
                    if (Network.isActive)
                    {
                        if (!isServerForObject) return;
                        SFX.PlaySynchronized("click", 1f, 0.5f, 0f, false, true);
                    }
                    else
                    {
                        SFX.Play("click", 1f, 0.5f, 0f, false);
                    }

                    break;
                }
            }
        }

		public override void OnPressAction()
        {
            if (!loaded || ammo <= 1) return;

            base.OnPressAction();
            for (int i = 0; i < 4; i++)
                Level.Add(Spark.New((offDir > 0) ? (x - 9f) : (x + 9f), y - 6f, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.05f));
            for (int j = 0; j < 4; j++)
                Level.Add(SmallSmoke.New(barrelPosition.x + offDir * 4f, barrelPosition.y));
            ammo = 1;
        }

		public override void Draw()
		{
			float ang = angle;
			if (offDir > 0)
			{
				angle -= _angleOffset;
			}
			else
			{
				angle += _angleOffset;
			}
			base.Draw();
			angle = ang;
		}
	}
}
