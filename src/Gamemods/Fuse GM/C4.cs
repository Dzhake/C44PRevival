﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DuckGame.C44P;
[EditorGroup("ADGM|GameMode Fuse")]
public class C4 : Holdable
{
    public bool ZoneOnly;
    public GM_Fuse? GM;

    public enum BombState { Spawned, Planted, Defused, Exploded }

    public BombState State = BombState.Spawned;
    public float ActionTimer;

    protected bool DrawAction;
    protected float actionVisibility;

    public StateBinding ActionTimerBinding = new("ActionTimer");


    public C4(float xval, float yval) : base(xval, yval)
    {
        _weight = 0f;
        _editorName = "Non-GM C4";

        _collisionSize = new Vec2(8f, 10f);
        _collisionOffset = new Vec2(-4f, -6f);
        _center = new Vec2(8f, 6f);

        _graphic = new($"{C44P.SpritesPath}Gamemodes/Fuse/C4");
    }

    public override void Update()
    {
        if (position.y > Level.current.lowestPoint + 400f)
        {
            if (GM is null) return;
            position = GM.position;
        }

        switch (State)
        {
            case BombState.Spawned:
            {
                SpawnedLogic();
                break;
            }
            case BombState.Planted:
            {
                PlantedLogic();
                break;
            }
            case BombState.Defused:
                DrawAction = false;
                break;
        }

        if (removedFromFall && isServerForObject)
            State = BombState.Defused;

        DevConsole.Log(DrawAction.ToString());
        base.Update();
    }

    protected void SpawnedLogic()
    {
        canPickUp = true;
        if (duck is null || GM is null) return;
        Duck d = duck;
        if (d.inputProfile.Released("SHOOT"))
            ActionTimer = 0f;

        if (!d.crouch || !d.grounded || (ZoneOnly && Level.CheckRect<PlantZone>(topLeft, bottomRight) == null)) return;

        DrawAction = Level.current is DuckGameTestArea || !Network.isActive || d == DuckNetwork.localProfile?.duck;
        if (!d.inputProfile.Down("SHOOT")) return;

        d._disarmDisable = 5;
        d.hSpeed = 0;
        ActionTimer += Maths.IncFrameTimer();
        if (isServerForObject && ActionTimer % 0.3 > 0.02 && ActionTimer % 0.3 < 0.05)
        {
            SFX.Play($"{C44P.Soundspath}boopbeep.wav");
            Level.Add(new PlantingButtonGraphic(x, y - 24f));
            d._disarmDisable = 0;
        }

        if (ActionTimer >= 2f) OnPlant();
    }

    protected void PlantedLogic()
    {
        DrawAction = false;
        foreach (Duck d in Level.CheckRectAll<Duck>(new Vec2(position.x - 10f, position.y + 2f),
                     new Vec2(position.x + 10f, position.y - 6f)))
        {
            if (FuseTeams.Team(d) != FuseTeams.FuseTeam.CT) continue;
            bool defuser = d.holdObject is Defuser;
            if (!defuser && d.holdObject is not null) continue;
            Duck? localDuck = DuckNetwork.localProfile?.duck;
            if (Level.current is DuckGameTestArea || !Network.isActive) DrawAction = true;
            if (d == localDuck)
            {
                Fondle(this);
                DrawAction = true;
            }

            if (!d.inputProfile.Down("SHOOT"))
            {
                ActionTimer = 0;
                continue;
            }
            d.hSpeed = 0;
            ActionTimer += Maths.IncFrameTimer() * (defuser ? 2 : 1);

            if (ActionTimer % 0.7 > 0.02 && ActionTimer % 0.7 < 0.05 && ActionTimer < 6)
            {
                SFX.Play(GetPath("SFX/Defuse.wav"));
                //Level.Add(new DefuseFore(position.x, position.y - 6f, 0.5f, (int)ActionTimer));
            }


            if (!(ActionTimer >= 5f)) continue;
            OnDefuse();
        }
    }

    public void OnPlant()
    {
        State = BombState.Planted;
        ActionTimer = 0f;

        if (duck is not null) duck.doThrow = true;
        canPickUp = false;
        angleDegrees = 0f;
        SFX.Play(GetPath("SFX/bombplanted.wav"));

        if (GM == null) return;
        GM.time = GM.ExplosionTime;
    }

    public void OnDefuse()
    {
        State = BombState.Defused;
        SFX.Play(GetPath("SFX/bombdefused.wav"));
        GM?.OnDefuse();
    }

    public virtual void Explode()
    {
        State = BombState.Exploded;
        foreach (Duck d in Level.CheckCircleAll<Duck>(position, 160f))
            d.Kill(new DTImpact(this));

        Level.Add(new ExplosionPart(position.x, position.y));
        int num = 6;

        for (int i = 0; i < num; i++)
        {
            float dir = i * 60f + Rando.Float(-10f, 10f);
            float dist = Rando.Float(20f, 20f);
            ExplosionPart ins = new(position.x + (float)(Math.Cos(Maths.DegToRad(dir)) * dist), position.y -
                (float)(Math.Sin(Maths.DegToRad(dir)) * dist));
            Level.Add(ins);
        }

        Graphics.FlashScreen();
        SFX.Play("explode");

        if (!isServerForObject) return;

        List<Bullet> firedBullets = new();

        for (int i = 0; i < 20; i++)
        {
            float dir = i * 18f - 5f + Rando.Float(10f);
            ATShrapnel shrap = new()
            {
                range = 160f,
                rangeVariation = 8f
            };
            Bullet bullet = new(position.x + (float)(Math.Cos(Maths.DegToRad(dir)) * 6.0), position.y -
                (float)(Math.Sin(Maths.DegToRad(dir)) * 6.0), shrap, dir);
            Level.Add(bullet);
            firedBullets.Add(bullet);
            if (Network.isActive)
            {
                NMFireGun gunEvent = new(null, firedBullets, 20, false);
                Send.Message(gunEvent, NetMessagePriority.ReliableOrdered);
                firedBullets.Clear();
            }
            Level.Remove(this);
        }
    }
    public override void Draw()
    {
        base.Draw();
        actionVisibility = Maths.LerpTowards(actionVisibility, DrawAction ? 1 : 0, DrawAction ? 0.04f : 0.05f);

        if (ActionTimer > 0 && State == BombState.Planted && (FuseTeams.Team(DuckNetwork.localProfile?.duck) == FuseTeams.FuseTeam.CT ||
             !Network.isActive || Level.current is DuckGameTestArea))
                Util.DrawCircle(new (position.x, position.y - 6f), 25, Color.LightGreen, 2f, depth, 50,
                    (int)(ActionTimer * 10));
        

        if (actionVisibility <= 0) return;

        Graphics.DrawString("@SHOOT@", position + new Vec2(-6, -36), Color.White * actionVisibility);
    }
}

