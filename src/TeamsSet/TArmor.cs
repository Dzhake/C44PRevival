namespace DuckGame.C44P
{
    [EditorGroup("ADGM|Teams")]
    public class TArmor : ChestPlate
    {
        public TArmor(float xval, float yval) : base(xval, yval)
        {
            _sprite = new SpriteMap($"{C44P.SpritesPath}Items/TArmor", 32, 32);
            _pickupSprite = new SpriteMap($"{C44P.SpritesPath}Items/TArmorPickup", 16, 16);
            _pickupSprite.CenterOrigin();
            _graphic = _pickupSprite;
            _spriteOver = new SpriteMap(new Tex2D(1, 1), 1, 1);
            _editorName = "T";
            editorTooltip = "Give this armor to terrorists via Equipper";
        }

        public override void Update()
        {
            base.Update();
            canPickUp = true;
            if (!FuseTeams.DuckTeams.ContainsKey(_equippedDuck))
                FuseTeams.DuckTeams.Add(_equippedDuck, FuseTeams.FuseTeam.CT);
        }
    }
}