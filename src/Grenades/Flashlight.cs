using System.Collections.Generic;

namespace DuckGame.C44P;

public class Flashlight : Thing
{
    public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
    public bool IsLocalDuckAffected;
    public float Timer;
    protected SpriteMap _sprite;
    protected float radius;
    protected int outFrame;
    protected SinWave _pulse = Rando.Float(0.5f, 4f);

    public Flashlight(float xval, float yval, float stayTime = 2f, float radius = 160f, float alpha = 1f) : base(xval, yval)
    {
        Timer = stayTime;

        depth = 1f;
        layer = Layer.Foreground;
        this.radius = radius;
        SetIsLocalDuckAffected();
        _sprite = new SpriteMap(Mod.GetPath<C44P>("Sprites/Items/Weapons/StunLight.png"), 32, 32)
        {
            alpha = alpha
        };
    }

    public void SetIsLocalDuckAffected()
    {
        List<Duck> ducks = new List<Duck>();
        foreach (Duck duck in Level.CheckCircleAll<Duck>(position, radius))
        {
            if (!ducks.Contains(duck))
            {
                ducks.Add(duck);
            }
        }
        foreach (Ragdoll ragdoll in Level.CheckCircleAll<Ragdoll>(position, radius))
        {
            if (!ducks.Contains(ragdoll._duck))
            {
                ducks.Add(ragdoll._duck);
            }
        }
        foreach (Duck duck in ducks)
        {
            if (!duck.profile.localPlayer || Level.CheckLine<Block>(position, duck.position, duck) != null) continue;
            IsLocalDuckAffected = true;
            return;
        }
    }

    public override void Update()
    {
        base.Update();
        _sprite.xscale = Level.current.camera.width / 32;
        _sprite.yscale = Level.current.camera.height / 32;
        _sprite.angleDegrees = 0f + _pulse * 0.1f;
        if (Timer > 0)
        {
            Timer -= 0.01f;
        }
        else
        {
            _sprite.alpha -= 0.011f;
            outFrame++;
        }
        if (outFrame > 90)
        {
            Level.Remove(this);
        }
    }

    public override void Draw()
    {
        if (IsLocalDuckAffected)
        {
            Graphics.Draw(_sprite, Level.current.camera.position.x, Level.current.camera.position.y, 1f);
        }
        base.Draw();
    }
}

