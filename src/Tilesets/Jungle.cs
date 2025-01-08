﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuckGame.C44P
{
    [EditorGroup("ADGM|Tiles|Jungle")]
    public class JunglePlatform : AutoPlatform
    {
        public JunglePlatform(float xval, float yval) : base(xval, yval, GetPath<C44P>("Sprites/Tilesets/Jungle/junglePlatform.png"))
        {
            _editorName = "Jungle Platform";
            physicsMaterial = PhysicsMaterial.Wood;
            verticalWidth = 10f;
            verticalWidthThick = 12f;
            horizontalHeight = 14f;
        }
    }

    [EditorGroup("ADGM|Tiles|Jungle")]
    public class JungleTree : AutoPlatform
    {
        public JungleTree(float xval, float yval) : base(xval, yval, GetPath<C44P>("Sprites/Tilesets/Jungle/jungleTree.png"))
        {
            _editorName = "Jungle Tree";
            physicsMaterial = PhysicsMaterial.Wood;
            verticalWidth = 10f;
            verticalWidthThick = 12f;
            horizontalHeight = 14f;
        }
    }

    [EditorGroup("ADGM|Tiles|Jungle")]
    public class JungleTileset(float x, float y) : SimpleTilesetBase(x, y, "Jungle", "Jungle/jungleTileset");

    [EditorGroup("ADGM|Tiles|Jungle")]
    public class BackgroundJungle(float x, float y) : SimpleBackgroundBase(x, y, "BG Jungle", "Jungle/jungleBackground");

    [EditorGroup("ADGM|Tiles|Jungle")]
    public class JungleParallax : BackgroundUpdater
    {
        int animation;
        int animationFrame;

        public JungleParallax(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new SpriteMap(Mod.GetPath<C44P>("Sprites/Tilesets/Jungle/jungleicon.png"), 16, 16, false);
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = 0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _editorName = "Parallax Jungle";
        }
        public override void Initialize()
        {
            if (Level.current is Editor)
            {
                return;
            }
            backgroundColor = new Color(88, 185, 255);
            Level.current.backgroundColor = backgroundColor;

            AddParallax("Sprites/Tilesets/Jungle/jungleParallaxV2_1.png", 0.4f);
        }

        void AddParallax(string path, float speed)
        {
            float xmove = 0;
            float[] scroll = new float[30];
            Vec2 prevPosition = new Vec2();

            ParallaxBackground.Definition definition = null;
            
            float prevX = 0;

            if (_parallax != null && Level.current.things.Contains(_parallax))
            {
                definition = _parallax.definition;
                prevX = _parallax.x;
                prevPosition = _parallax.position;
                //_parallax.graphic = new Sprite(Mod.GetPath<C44P>(path));
                xmove = _parallax.xmove;
                FieldInfo field = _parallax.GetType().GetField("_zones", BindingFlags.NonPublic | BindingFlags.Instance);
                var zones = field.GetValue(_parallax);
                IDictionary<int, ParallaxZone> dict = zones as IDictionary<int, ParallaxZone>;

                for (int i = 0; i < _parallax.graphic.height / 8; i++)
                {
                    if (dict.ContainsKey(i))
                    {
                        scroll[i] = dict[i].scroll;
                    }
                }

                Level.Remove(_parallax);
            }

            _parallax = new ParallaxBackground(Mod.GetPath<C44P>(path), prevX, 0f, 3);

            /*_parallax.AddZone(0, 0.3f, speed, true, true);
            _parallax.AddZone(1, 0.3f, speed, true, true);
            _parallax.AddZone(2, 0.3f, speed, true, true);
            _parallax.AddZone(3, 0.3f, speed, true, true);*/

            _parallax.AddZone(0, 0f, speed, false, true);
            _parallax.AddZone(1, 0f, speed, false, true);
            _parallax.AddZone(2, 0f, speed, false, true);
            _parallax.AddZone(3, 0f, speed, false, true);

            _parallax.AddZone(4, 0f, speed, false, true);
            _parallax.AddZone(5, 0f, speed, false, true);
            _parallax.AddZone(6, 0f, speed, false, true);
            _parallax.AddZone(7, 0f, speed, false, true);
            _parallax.AddZone(8, 0f, speed, false, true);
            _parallax.AddZone(9, 0f, speed, false, true);
            _parallax.AddZone(10, 0f, speed, false, true);
            _parallax.AddZone(11, 0f, speed, false, true);
            _parallax.AddZone(12, 0f, speed, false, true);
            _parallax.AddZone(13, 0f, speed, false, true);
            _parallax.AddZone(14, 0f, speed, false, true);
            _parallax.AddZone(15, 0f, speed, false, true);
            _parallax.AddZone(16, 0f, speed, false, true);
            _parallax.AddZone(17, 0f, speed, false, true);

            _parallax.AddZone(18, 0f, speed, false, true);
            _parallax.AddZone(19, 0f, speed, false, true);
            _parallax.AddZone(20, 0f, speed, false, true);
            _parallax.AddZone(21, 0f, speed, false, true);
            _parallax.AddZone(22, 0f, speed, false, true);
            _parallax.AddZone(23, 0f, speed, false, true);
            _parallax.AddZone(24, 0f, speed, false, true);
            _parallax.AddZone(25, 0f, speed, false, true);
            _parallax.AddZone(26, 0f, speed, false, true);
            _parallax.AddZone(27, 0f, speed, false, true);
            _parallax.AddZone(28, 0f, speed, false, true);
            _parallax.AddZone(29, 0f, speed, false, true);

            /*_parallax.AddZone(18, 0.3f, speed, false, true);
            _parallax.AddZone(19, 0.3f, speed, false, true);
            _parallax.AddZone(20, 0.3f, speed, false, true);
            _parallax.AddZone(21, 0.3f, speed, false, true);

            _parallax.AddZone(22, 0.05f, speed, true, true);
            _parallax.AddZone(23, 0.05f, speed, true, true);

            _parallax.AddZone(24, 0.2f, speed, true, true);
            _parallax.AddZone(25, 0.2f, speed, true, true);

            _parallax.AddZone(26, 0.05f, speed, true, true);
            _parallax.AddZone(27, 0.0125f, speed, true, true);
            _parallax.AddZone(28, 0.125f, speed, true, true);
            _parallax.AddZone(29, 0.1f, speed, true, true);*/

            //_parallax.position = prevPosition;
            //_parallax.xmove = xmove;

            Level.Add(_parallax);

            if (_parallax != null && Level.current.things.Contains(_parallax))
            {
                if(definition != null)
                {
                    _parallax.definition = definition;
                }
                FieldInfo field = _parallax.GetType().GetField("_zones", BindingFlags.NonPublic | BindingFlags.Instance);
                var zones = field.GetValue(_parallax);
                IDictionary<int, ParallaxZone> dict = zones as IDictionary<int, ParallaxZone>;

                for (int i = 0; i < _parallax.graphic.height / 8; i++)
                {
                    if (dict.ContainsKey(i))
                    {
                        dict[i].scroll = scroll[i];
                    }
                }
                field.SetValue(_parallax, dict);
            }
        }

        public override void Update()
        {
            Vec2 wallScissor = GetWallScissor();
            if (wallScissor != Vec2.Zero)
            {
                scissor = new Rectangle((float)((int)wallScissor.x), 0f, (float)((int)wallScissor.y), (float)Graphics.height);
            }
            animation++;
            if (animation % 5 == 0)
            {
                animationFrame++;
                animationFrame = animationFrame % 4;
                string pathStart = "Sprites/Tilesets/Jungle/jungleParallaxV2_";
                string pathMid = Convert.ToString(animationFrame + 1);
                string pathEnd = ".png";

                AddParallax(pathStart + pathMid + pathEnd, 0.4f);
            }
            base.Update();
        }
        public override void Terminate()
        {
            Level.Remove(_parallax);
        }
    }
}
