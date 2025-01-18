using System;

namespace DuckGame.C44P;

public enum ProgressBarType
{
    Progress,
    KeyPoint,
    ScoreCompetition
}

public class GMTimer : Thing
{
    protected BitmapFont _font;
    public string str = "";
    public string subtext = "";
    public Color subtextColor = Color.Orange;

    protected float _time;
    public float time
    {
        get => _time;
        set
        {
            if (value < 0)
                _time = 0;
            _time = value;
        }
    }


    protected bool isTimerActive;

    public StateBinding _TimerBind = new("_time");
    public StateBinding _StringBind = new("str");

    public bool progressBar;
    public float progress;
    public float progressTarget;
    public ProgressBarType progressBarType = ProgressBarType.Progress;

    protected Sprite? redDot;
    protected Sprite? greenDot;

    public GMTimer(float xpos, float ypos) : base(xpos, ypos)
    {
        _font = new BitmapFont("biosFont", 8);

        layer = Layer.Foreground;
        depth = 0.9f;
    }

    protected void InitializeDots()
    {
        for (int k = 0; k < 2; k++)
        {
            int TexHeight = 17;
            int TexWidth = 17;

            Tex2D tex = new Tex2D(TexWidth, TexHeight);

            Color[] texArray = new Color[TexWidth * TexHeight];
            for (int i = 0; i < TexHeight; i++)
            {
                for (int j = 0; j < TexWidth; j++)
                {
                    float pixAlpha = 0f;

                    double polarX = (i - TexHeight * 0.5f);
                    double polarY = (j - TexWidth * 0.5f);

                    double polarRange = Math.Sqrt(polarX * polarX + polarY * polarY);

                    Color color = Color.White;
                    if (polarRange <= 8.5f && polarRange > 7.5f)
                    {
                        pixAlpha = 1;
                        color = Color.Black;
                    }
                    if (polarRange <= 7.5f && polarRange > 6.5f)
                    {
                        pixAlpha = 1;
                        color = Color.White;
                    }
                    if (polarRange <= 6.5f)
                    {
                        pixAlpha = 1;
                        if (k == 0)
                        {
                            color = Color.Yellow;
                        }
                        else
                        {
                            color = Color.DarkRed;
                        }
                    }

                    texArray[i * TexWidth + j] = color * pixAlpha;
                }
            }
            tex.SetData(texArray);

            if (k == 0)
            {
                greenDot = new Sprite(tex);
            }
            else
            {
                redDot = new Sprite(tex);
            }
        }
    }

    public void Pause()
    {
        isTimerActive = false;
    }
    public void Resume()
    {
        isTimerActive = true;
    }

    public override void Update()
    {
        base.Update();
        if (!isTimerActive) return;
        time -= Maths.IncFrameTimer();
        if (time >= 0) return;
        time = 0;
        isTimerActive = false;
    }

    public override void Draw()
    {
        Vec2 camPos = new Vec2(Level.current.camera.position.x, Level.current.camera.position.y);
        Vec2 camSize = new Vec2(Level.current.camera.width, Level.current.camera.height);
        Vec2 textPos = camPos + camSize * new Vec2(0.5f, 0.015f);

        _font.scale = Level.current.camera.size / new Vec2(480, 270);

        int mins = (int)time / 60;
        int seconds = (int)time % 60;

        string text;
        if (!string.IsNullOrEmpty(str))
        {
            text = str;
            Color col = Color.Orange;
            float xpos = textPos.x - _font.GetWidth(text) / 2f;
            Graphics.DrawStringOutline(text, new Vec2(xpos, textPos.y), col, Color.Black, 0.9f, null, _font.scale.x);
            return;
        }
        text = Convert.ToString(mins) + ":";
        if (seconds < 10)
            text += "0";
        text += Convert.ToString(seconds);

        float xposit = textPos.x - _font.GetWidth(text) / 2f;

        Color c = Color.White;
        if ((int)time % 2 == 1 && (int)time < 10)
        {
            c = Color.Red;
        }

        Graphics.DrawStringOutline(text, new Vec2(xposit, textPos.y), c, Color.Black, 0.9f, null, _font.scale.x);

        Vec2 unit = camSize / new Vec2(320, 180);
        if (!string.IsNullOrEmpty(subtext))
        {
            textPos += new Vec2(0, 10f) * unit;
            Graphics.DrawStringOutline(subtext, textPos + new Vec2(-4 * subtext.Length, 0) * 0.5f * _font.scale.x, subtextColor, Color.Black, depth, null, 0.5f * _font.scale.x);
        }

        if (!progressBar) return;
        textPos += new Vec2(0, 10f) * unit;

        switch (progressBarType)
        {
        case ProgressBarType.Progress:
            Graphics.DrawRect(textPos + new Vec2(-camSize.x * 0.15f, -2.5f * unit.y), textPos + new Vec2(camSize.x * 0.15f, 2.5f * unit.y), Color.White, depth, false, unit.x * 0.5f);
            Graphics.DrawRect(textPos + new Vec2(-camSize.x * 0.15f + unit.x * -0.5f, -3 * unit.y), textPos + new Vec2(camSize.x * 0.15f + unit.x * 0.5f, 3f * unit.y), Color.Black, depth, false, unit.x * 0.5f);
            Graphics.DrawRect(textPos + new Vec2(-camSize.x * 0.15f, -2f * unit.y), textPos + new Vec2(-camSize.x * 0.15f + (progress) * camSize.x * 0.3f, 2f * unit.y), Color.Yellow, depth, true, unit.x);
            Graphics.DrawRect(textPos + new Vec2(-camSize.x * 0.15f, -2f * unit.y), textPos + new Vec2(-camSize.x * 0.15f + (1) * camSize.x * 0.3f, 2f * unit.y), Color.DarkRed, depth - 1, true, unit.x);
            break;
        case ProgressBarType.KeyPoint when redDot is null:
        {
            InitializeDots();
            break;
        }
        case ProgressBarType.KeyPoint:
            for (int k = 0; k < progressTarget; k++)
            {
                Sprite circle = redDot;
                if (greenDot != null && progress >= k + 1)
                {
                    circle = greenDot;
                }
                circle.CenterOrigin();
                circle.scale = unit * 0.5f;
                circle.depth = depth;

                Graphics.Draw(circle, textPos.x + (-(progressTarget - 1) * 0.5f + k) * (120 / progressTarget) * unit.x, textPos.y);
            }
            break;
        case ProgressBarType.ScoreCompetition:
            Graphics.DrawLine(textPos + new Vec2(-camSize.x * 0.15f, 0), textPos + new Vec2(camSize.x * 0.15f, 0), Color.White, unit.x * 0.5f, depth);
            Graphics.DrawRect(textPos + new Vec2(-camSize.x * 0.15f - 0.5f * unit.x, -0.5f * unit.y), textPos + new Vec2(camSize.x * 0.15f + 0.5f * unit.x, 0.5f * unit.y), Color.Black, depth.Add(-1));
            break;
        }
    }
}