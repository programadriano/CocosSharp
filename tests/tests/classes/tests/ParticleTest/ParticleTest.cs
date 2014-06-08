using System;
using System.Collections.Generic;
using CocosSharp;
using Random = CocosSharp.CCRandom;

namespace tests
{
    public class ParticleTestScene : TestScene
    {
        #region eIDClick enum

        public enum eIDClick
        {
            IDC_NEXT = 100,
            IDC_BACK,
            IDC_RESTART,
            IDC_TOGGLE
        };

        #endregion

        internal static int TagLabelAtlas = 1;
        internal static int SceneIdx = -1;
        internal static int MAX_LAYER = 43;

        public static CCLayer CreateParticleLayer(int nIndex)
        {
            switch (nIndex)
            {
            case 0:
                return new ParticleReorder();
            case 1:
                return new ParticleBatchHybrid();
            case 2:
                return new ParticleBatchMultipleEmitters();
            case 3:
                return new DemoFlower();
            case 4:
                return new DemoGalaxy();
            case 5:
                return new DemoFirework();
            case 6:
                return new DemoSpiral();
            case 7:
                return new DemoSun();
            case 8:
                return new DemoMeteor();
            case 9:
                return new DemoFire();
            case 10:
                return new DemoSmoke();
            case 11:
                return new DemoExplosion();
            case 12:
                return new DemoSnow();
            case 13:
                return new DemoRain();
            case 14:
                return new DemoBigFlower();
            case 15:
                return new DemoRotFlower();
            case 16:
                return new DemoModernArt();
            case 17:
                return new DemoRing();
            case 18:
                return new ParallaxParticle();
            case 19:
                return new DemoParticleFromFile("BoilingFoam");
            case 20:
                return new DemoParticleFromFile("BurstPipe");
            case 21:
                return new DemoParticleFromFile("Comet");
            case 22:
                return new DemoParticleFromFile("debian");
            case 23:
                return new DemoParticleFromFile("ExplodingRing");
            case 24:
                return new DemoParticleFromFile("LavaFlow");
            case 25:
                return new DemoParticleFromFile("SpinningPeas");
            case 26:
                return new DemoParticleFromFile("SpookyPeas");
            case 27:
                return new DemoParticleFromFile("Upsidedown");
            case 28:
                return new DemoParticleFromFile("Flower");
            case 29:
                return new DemoParticleFromFile("Spiral");
            case 30:
                return new DemoParticleFromFile("Galaxy");
            case 31:
                return new DemoParticleFromFile("Phoenix");
            case 32:
                return new RadiusMode1();
            case 33:
                return new RadiusMode2();
            case 34:
                return new Issue704();
            case 35:
                return new Issue870();
            case 36:
                return new Issue1201();
                // v1.1 tests
            case 37:
                return new MultipleParticleSystems();
            case 38:
                return new MultipleParticleSystemsBatched();
            case 39:
                return new AddAndDeleteParticleSystems();
            case 40:
                return new ReorderParticleSystems();
            case 41:
                return new PremultipliedAlphaTest();
            case 42:
                return new PremultipliedAlphaTest2();
            }

            return null;
        }

        public static CCLayer NextParticleAction()
        {
            SceneIdx++;
            SceneIdx = SceneIdx % MAX_LAYER;

            CCLayer layer = CreateParticleLayer(SceneIdx);

            return layer;
        }

        public static CCLayer BackParticleAction()
        {
            SceneIdx--;
            int total = MAX_LAYER;
            if (SceneIdx < 0)
                SceneIdx += total;

            CCLayer layer = CreateParticleLayer(SceneIdx);

            return layer;
        }

        public static CCLayer RestartParticleAction()
        {
            CCLayer layer = CreateParticleLayer(SceneIdx);

            return layer;
        }

        protected override void NextTestCase()
        {
            NextParticleAction();
        }
        protected override void PreviousTestCase()
        {
            BackParticleAction();
        }
        protected override void RestTestCase()
        {
            RestartParticleAction();
        }

        public override void runThisTest()
        {
            AddChild(NextParticleAction());

            CCApplication.SharedApplication.MainWindowDirector.ReplaceScene(this);
        }
    };

    public class ParticleDemo : CCLayerColor
    {
        const int labelTag = 9000;

        protected CCPoint MidWindowPoint;
        protected CCSize WindowSize;
        protected CCParticleSystem Emitter;
        protected CCSprite Background;

        CCLabelTtf titleLabel;
        CCLabelTtf subtitleLabel;
        CCLabelAtlas particleCounter;

        CCMenu particleMenu;
        CCMenuItem backMenuItem;
        CCMenuItem restartMenuItem;
        CCMenuItem nextMenuItem;
        CCMenuItemToggle toggleParticleMovMenuItem;


        #region Constructors

        public ParticleDemo() : base(new CCColor4B(127, 127, 127, 255))
        {
            titleLabel = new CCLabelTtf(Title(), "arial", 28);
            AddChild(titleLabel, 100, labelTag);

            subtitleLabel = new CCLabelTtf(Subtitle(), "arial", 20);
            AddChild(subtitleLabel, 100);

            backMenuItem = new CCMenuItemImage(TestResource.s_pPathB1, TestResource.s_pPathB2, BackCallback);
            restartMenuItem = new CCMenuItemImage(TestResource.s_pPathR1, TestResource.s_pPathR2, RestartCallback);
            nextMenuItem = new CCMenuItemImage(TestResource.s_pPathF1, TestResource.s_pPathF2, NextCallback);

            toggleParticleMovMenuItem = new CCMenuItemToggle(ToggleCallback,
                new CCMenuItemFont("Free Movement"),
                new CCMenuItemFont("Relative Movement"),
                new CCMenuItemFont("Grouped Movement"));

            particleMenu = new CCMenu(backMenuItem, restartMenuItem, nextMenuItem, toggleParticleMovMenuItem);
            AddChild(particleMenu, 100);

            particleCounter = new CCLabelAtlas("0000", "Images/fps_Images", 12, 32, '.');
            AddChild(particleCounter, 100, ParticleTestScene.TagLabelAtlas);

            Background = new CCSprite(TestResource.s_back3);
            AddChild(Background, 5);
        }

        #endregion Constructors


        #region Setup content

        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            WindowSize = windowSize;
            MidWindowPoint = new CCPoint(windowSize.Width / 2.0f, windowSize.Height / 2.0f);

            // Laying out content based on window size

            titleLabel.Position = new CCPoint(windowSize.Width / 2, windowSize.Height - 50);
            subtitleLabel.Position = new CCPoint(windowSize.Width / 2, windowSize.Height - 80);

            particleMenu.Position = new CCPoint(0, 0);
            backMenuItem.Position = new CCPoint(windowSize.Width / 2 - 100, 30);
            restartMenuItem.Position = new CCPoint(windowSize.Width / 2, 30);
            nextMenuItem.Position = new CCPoint(windowSize.Width / 2 + 100, 30);
            toggleParticleMovMenuItem.Position = new CCPoint(0, 100);
            toggleParticleMovMenuItem.AnchorPoint = new CCPoint(0, 0);

            particleCounter.Position = new CCPoint(windowSize.Width - 66, 50);

            Background.Position = new CCPoint(windowSize.Width / 2, windowSize.Height - 180);

            // Set title label

            var label = (CCLabelTtf) (GetChildByTag(labelTag));
            label.Text = Title();

            // Run background animation

            CCActionInterval move = new CCMoveBy (4, new CCPoint(300, 0));
            CCFiniteTimeAction move_back = move.Reverse();
            CCFiniteTimeAction seq = new CCSequence(move, move_back);
            Background.RunAction(new CCRepeatForever ((CCActionInterval) seq));

            Schedule(Step);

            // Add event listeners

            var listener = new CCEventListenerTouchAllAtOnce();
            listener.OnTouchesBegan = OnTouchesBegan;
            listener.OnTouchesMoved = OnTouchesMoved;
            listener.OnTouchesEnded = OnTouchesEnded;

            EventDispatcher.AddEventListener(listener, this);
        }

        #endregion Setup content


        #region Titles

        protected virtual string Title()
        {
            return "No title";
        }

        protected virtual string Subtitle()
        {
            return String.Empty;
        }

        #endregion Titles


        #region Callbacks

        void RestartCallback(object sender)
        {
            if (Emitter != null)
            {
                Emitter.ResetSystem();
            }
        }

        void NextCallback(object sender)
        {
            var s = new ParticleTestScene();
            s.AddChild(ParticleTestScene.NextParticleAction());
            CCApplication.SharedApplication.MainWindowDirector.ReplaceScene(s);
        }

        void BackCallback(object sender)
        {
            var s = new ParticleTestScene();
            s.AddChild(ParticleTestScene.BackParticleAction());
            CCApplication.SharedApplication.MainWindowDirector.ReplaceScene(s);
        }

        void ToggleCallback(object sender)
        {
            if (Emitter != null)
            {
                if (Emitter.PositionType == CCPositionType.Grouped)
                    Emitter.PositionType = CCPositionType.Free;
                else if (Emitter.PositionType == CCPositionType.Free)
                    Emitter.PositionType = CCPositionType.Relative;
                else if (Emitter.PositionType == CCPositionType.Relative)
                    Emitter.PositionType = CCPositionType.Grouped;
            }
        }

        #endregion Callbacks


        #region Event handling

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            OnTouchesEnded(touches, touchEvent);
        }

        void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            OnTouchesEnded(touches, touchEvent);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            var touch = touches [0];
            var convertedLocation = touch.Location;

            var pos = new CCPoint(0, 0);
            if (Background != null)
            {
                pos = Background.ConvertToWorldSpace(CCPoint.Zero);
            }

            if (Emitter != null)
            {
                Emitter.Position = convertedLocation - pos;
            }
        }

        #endregion Event handling


        void Step(float dt)
        {
            var atlas = (CCLabelAtlas) GetChildByTag(ParticleTestScene.TagLabelAtlas);

            if (Emitter != null)
            {
                string str = string.Format("{0:0000}", Emitter.ParticleCount);
                atlas.Text = (str);
            }
            else
            {
                int count = 0;
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i] is CCParticleSystem)
                    {
                        count += ((CCParticleSystem) Children[i]).ParticleCount;
                    }
                    else if (Children[i] is CCParticleBatchNode)
                    {
                        var bn = (CCParticleBatchNode) Children[i];
                        for (int j = 0; j < bn.ChildrenCount; j++)
                        {
                            if (bn.Children[j] is CCParticleSystem)
                            {
                                count += ((CCParticleSystem) bn.Children[j]).ParticleCount;
                            }
                        }
                    }
                }
                string str = string.Format("{0:0000}", count);
                atlas.Text = (str);
            }
        }

        protected void SetEmitterPosition()
        {
            if (Emitter != null)
            {
                Emitter.Position = new CCPoint(WindowSize.Width / 2, WindowSize.Height / 2 - 30);
            }
        }
    }

    //------------------------------------------------------------------
    //
    // DemoFirework
    //
    //------------------------------------------------------------------
    public class DemoFirework : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleFireworks(MidWindowPoint);
            Background.AddChild(Emitter, 10);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_stars1);
        }

        protected override string Title()
        {
            return "ParticleFireworks";
        }
    }

    //------------------------------------------------------------------
    //
    // DemoFire
    //
    //------------------------------------------------------------------
    public class DemoFire : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            CCPoint emitterPos = new CCPoint(windowSize.Width / 2, 100);
            Emitter = new CCParticleFire(emitterPos);
            Background.AddChild(Emitter, 10);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire); //.pvr"];
        }

        protected override string Title()
        {
            return "ParticleFire";
        }
    };

    //------------------------------------------------------------------
    //
    // DemoSun
    //
    //------------------------------------------------------------------
    public class DemoSun : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleSun(MidWindowPoint);
            Background.AddChild(Emitter, 10);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);
        }

        protected override string Title()
        {
            return "ParticleSun";
        }
    }

    //------------------------------------------------------------------
    //
    // DemoGalaxy
    //
    //------------------------------------------------------------------
    public class DemoGalaxy : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            CCPoint position = new CCPoint(MidWindowPoint);
            Emitter = new CCParticleGalaxy(position);
            Background.AddChild(Emitter, 10);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);
        }

        protected override string Title()
        {
            return "ParticleGalaxy";
        }
    };

    //------------------------------------------------------------------
    //
    // DemoFlower
    //
    //------------------------------------------------------------------
    public class DemoFlower : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleFlower(MidWindowPoint);
            Background.AddChild(Emitter, 10);
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_stars1);
        }

        protected override string Title()
        {
            return "ParticleFlower";
        }
    };

    //------------------------------------------------------------------
    //
    // DemoBigFlower
    //
    //------------------------------------------------------------------
    public class DemoBigFlower : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleSystemQuad(50);
            //Emitter.autorelease();

            Background.AddChild(Emitter, 10);
            ////Emitter.release();    // win32 :  use this line or remove this line and use autorelease()
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_stars1);

            Emitter.Duration = -1;

            // gravity
            Emitter.Gravity = (new CCPoint(0, 0));

            // angle
            Emitter.Angle = 90;
            Emitter.AngleVar = 360;

            // speed of particles
            Emitter.Speed = (160);
            Emitter.SpeedVar = (20);

            // radial
            Emitter.RadialAccel = (-120);
            Emitter.RadialAccelVar = (0);

            // tagential
            Emitter.TangentialAccel = (30);
            Emitter.TangentialAccelVar = (0);

            // emitter position
            Emitter.Position = new CCPoint(160, 240);
            Emitter.PositionVar = new CCPoint(0, 0);

            // life of particles
            Emitter.Life = 4;
            Emitter.LifeVar = 1;

            // spin of particles
            Emitter.StartSpin = 0;
            Emitter.StartSizeVar = 0;
            Emitter.EndSpin = 0;
            Emitter.EndSpinVar = 0;

            // color of particles
            var startColor = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColor = startColor;

            var startColorVar = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColorVar = startColorVar;

            var endColor = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColor = endColor;

            var endColorVar = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColorVar = endColorVar;

            // size, in pixels
            Emitter.StartSize = 80.0f;
            Emitter.StartSizeVar = 40.0f;
            Emitter.EndSize = CCParticleSystem.ParticleStartSizeEqualToEndSize;

            // emits per second
            Emitter.EmissionRate = Emitter.TotalParticles / Emitter.Life;

            // additive
            Emitter.BlendAdditive = true;
        }

        protected override string Title()
        {
            return "ParticleBigFlower";
        }
    };

    //------------------------------------------------------------------
    //
    // DemoRotFlower
    //
    //------------------------------------------------------------------
    public class DemoRotFlower : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleSystemQuad(300);
            //Emitter.autorelease();

            Background.AddChild(Emitter, 10);
            ////Emitter.release();    // win32 : Remove this line
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_stars2);

            // duration
            Emitter.Duration = -1;

            // gravity
            Emitter.Gravity = (new CCPoint(0, 0));

            // angle
            Emitter.Angle = 90;
            Emitter.AngleVar = 360;

            // speed of particles
            Emitter.Speed = (160);
            Emitter.SpeedVar = (20);

            // radial
            Emitter.RadialAccel = (-120);
            Emitter.RadialAccelVar = (0);

            // tagential
            Emitter.TangentialAccel = (30);
            Emitter.TangentialAccelVar = (0);

            // emitter position
            Emitter.Position = new CCPoint(160, 240);
            Emitter.PositionVar = new CCPoint(0, 0);

            // life of particles
            Emitter.Life = 3;
            Emitter.LifeVar = 1;

            // spin of particles
            Emitter.StartSpin = 0;
            Emitter.StartSpinVar = 0;
            Emitter.EndSpin = 0;
            Emitter.EndSpinVar = 2000;

            // color of particles
            var startColor = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColor = startColor;

            var startColorVar = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColorVar = startColorVar;

            var endColor = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColor = endColor;

            var endColorVar = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColorVar = endColorVar;

            // size, in pixels
            Emitter.StartSize = 30.0f;
            Emitter.StartSizeVar = 00.0f;
            Emitter.EndSize = CCParticleSystem.ParticleStartSizeEqualToEndSize;

            // emits per second
            Emitter.EmissionRate = Emitter.TotalParticles / Emitter.Life;

            // additive
            Emitter.BlendAdditive = false;
        }

        protected override string Title()
        {
            return "ParticleRotFlower";
        }
    };

    public class DemoMeteor : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleMeteor(MidWindowPoint);

            Background.AddChild(Emitter, 10);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);
        }

        protected override string Title()
        {
            return "ParticleMeteor";
        }
    };

    public class DemoSpiral : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleSpiral(MidWindowPoint);

            Background.AddChild(Emitter, 10);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);
        }

        protected override string Title()
        {
            return "ParticleSpiral";
        }
    };

    public class DemoExplosion : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleExplosion(MidWindowPoint);

            Background.AddChild(Emitter, 10);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_stars1);

            Emitter.AutoRemoveOnFinish = true;
        }

        protected override string Title()
        {
            return "ParticleExplosion";
        }
    };

    public class DemoSmoke : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleSmoke(new CCPoint(windowSize.Width / 2.0f, 0));

            Background.AddChild(Emitter, 10);
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);

            CCPoint p = Emitter.Position;
            Emitter.Position = new CCPoint(p.X, 100);
        }

        protected override string Title()
        {
            return "ParticleSmoke";
        }
    };

    public class DemoSnow : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleSnow(new CCPoint(windowSize.Width / 2, windowSize.Height + 10));

            Background.AddChild(Emitter, 10);

            CCPoint p = Emitter.Position;
            Emitter.Position = new CCPoint(p.X, p.Y - 110);
            Emitter.Life = 3;
            Emitter.LifeVar = 1;

            // gravity
            Emitter.Gravity = (new CCPoint(0, -10));

            // speed of particles
            Emitter.Speed = (130);
            Emitter.SpeedVar = (30);

            var startColor = Emitter.StartColor;
            startColor.R = 0.9f;
            startColor.G = 0.9f;
            startColor.B = 0.9f;
            Emitter.StartColor = startColor;

            var startColorVar = Emitter.StartColorVar;
            startColorVar.B = 0.1f;
            Emitter.StartColorVar = startColorVar;

            Emitter.EmissionRate = Emitter.TotalParticles / Emitter.Life;

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_snow);
        }

        protected override string Title()
        {
            return "ParticleSnow";
        }
    };

    public class DemoRain : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleRain(new CCPoint (windowSize.Width / 2.0f, windowSize.Height));
            Background.AddChild(Emitter, 10);

            CCPoint p = Emitter.Position;
            Emitter.Position = new CCPoint(p.X, p.Y - 100);
            Emitter.Life = 4;

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);
        }

        protected override string Title()
        {
            return "ParticleRain";
        }
    };

    // todo: CCParticleSystemPoint::draw() hasn't been implemented.
    public class DemoModernArt : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleSystemQuad(1000);
            //Emitter.autorelease();

            Background.AddChild(Emitter, 10);
            ////Emitter.release();

            CCSize s = CCApplication.SharedApplication.MainWindowDirector.WindowSizeInPoints;

            // duration
            Emitter.Duration = -1;

            // gravity
            Emitter.Gravity = (new CCPoint(0, 0));

            // angle
            Emitter.Angle = 0;
            Emitter.AngleVar = 360;

            // radial
            Emitter.RadialAccel = (70);
            Emitter.RadialAccelVar = (10);

            // tagential
            Emitter.TangentialAccel = (80);
            Emitter.TangentialAccelVar = (0);

            // speed of particles
            Emitter.Speed = (50);
            Emitter.SpeedVar = (10);

            // emitter position
            Emitter.Position = new CCPoint(s.Width / 2, s.Height / 2);
            Emitter.PositionVar = new CCPoint(0, 0);

            // life of particles
            Emitter.Life = 2.0f;
            Emitter.LifeVar = 0.3f;

            // emits per frame
            Emitter.EmissionRate = Emitter.TotalParticles / Emitter.Life;

            // color of particles
            var startColor = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColor = startColor;

            var startColorVar = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColorVar = startColorVar;

            var endColor = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColor = endColor;

            var endColorVar = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColorVar = endColorVar;

            // size, in pixels
            Emitter.StartSize = 1.0f;
            Emitter.StartSizeVar = 1.0f;
            Emitter.EndSize = 32.0f;
            Emitter.EndSizeVar = 8.0f;

            // texture
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);

            // additive
            Emitter.BlendAdditive = false;
        }

        protected override string Title()
        {
            return "Varying size";
        }
    };

    public class DemoRing : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Emitter = new CCParticleFlower(MidWindowPoint);

            Background.AddChild(Emitter, 10);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_stars1);
            Emitter.LifeVar = 0;
            Emitter.Life = 10;
            Emitter.Speed = (100);
            Emitter.SpeedVar = (0);
            Emitter.EmissionRate = 10000;
        }

        protected override string Title()
        {
            return "Ring Demo";
        }
    };

    public class ParallaxParticle : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Background.Parent.RemoveChild(Background, true);
            Background = null;

            CCParallaxNode p = new CCParallaxNode();
            AddChild(p, 5);

            CCSprite p1 = new CCSprite(TestResource.s_back3);
            CCSprite p2 = new CCSprite(TestResource.s_back3);

            p.AddChild(p1, 1, new CCPoint(0.5f, 1), new CCPoint(0, 250));
            p.AddChild(p2, 2, new CCPoint(1.5f, 1), new CCPoint(0, 50));

            Emitter = new CCParticleFlower(MidWindowPoint);

            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);

            p1.AddChild(Emitter, 10);
            Emitter.Position = new CCPoint(250, 200);

            CCParticleSun par = new CCParticleSun(MidWindowPoint);
            p2.AddChild(par, 10);
            par.Texture = CCApplication.SharedApplication.TextureCache.AddImage(TestResource.s_fire);

            CCActionInterval move = new CCMoveBy (4, new CCPoint(300, 0));
            CCFiniteTimeAction move_back = move.Reverse();
            CCFiniteTimeAction seq = new CCSequence(move, move_back);
            p.RunAction(new CCRepeatForever ((CCActionInterval) seq));
        }

        protected override string Title()
        {
            return "Parallax + Particles";
        }
    };

    public class DemoParticleFromFile : ParticleDemo
    {
        readonly string title;

        static Dictionary<string, CCParticleSystemConfig> particleConfigManager;

        public DemoParticleFromFile()
        {
            if (particleConfigManager == null)
                particleConfigManager = new Dictionary<string, CCParticleSystemConfig> ();
        }

        public DemoParticleFromFile(string file) : base()
        {
            if (particleConfigManager == null)
                particleConfigManager = new Dictionary<string, CCParticleSystemConfig> ();

            title = file;

        }

        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = new CCColor3B(0, 0, 0);
            RemoveChild(Background, true);
            Background = null;

            string filename = "Particles/" + title;

            CCParticleSystemConfig config;

            if (particleConfigManager.ContainsKey (filename))
                config = particleConfigManager [filename];
            else
            {
                config = new CCParticleSystemConfig (filename);
                particleConfigManager.Add (filename, config);
            }

            Emitter = new CCParticleSystemQuad(config);

            AddChild(Emitter, 10);

            Emitter.BlendAdditive = true;
        }

        protected override string Title()
        {
            if (null != title)
            {
                return title;
            }
            else
            {
                return "ParticleFromFile";
            }
        }
    };

    public class RadiusMode1 : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = new CCColor3B(0, 0, 0);
            RemoveChild(Background, true);
            Background = null;

            Emitter = new CCParticleSystemQuad(200, CCEmitterMode.Radius);
            AddChild(Emitter, 10);
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage("Images/stars-grayscale");

            // duration
            Emitter.Duration = CCParticleSystem.ParticleDurationInfinity;

            // radius mode: start and end radius in pixels
            Emitter.StartRadius = (0);
            Emitter.StartRadiusVar = (0);
            Emitter.EndRadius = (160);
            Emitter.EndRadiusVar = (0);

            // radius mode: degrees per second
            Emitter.RotatePerSecond = (180);
            Emitter.RotatePerSecondVar = (0);

            // angle
            Emitter.Angle = 90;
            Emitter.AngleVar = 0;

            // emitter position
            CCSize size = CCApplication.SharedApplication.MainWindowDirector.WindowSizeInPoints;
            Emitter.Position = new CCPoint(size.Width / 2, size.Height / 2);
            Emitter.PositionVar = new CCPoint(0, 0);

            // life of particles
            Emitter.Life = 5;
            Emitter.LifeVar = 0;

            // spin of particles
            Emitter.StartSpin = 0;
            Emitter.StartSpinVar = 0;
            Emitter.EndSpin = 0;
            Emitter.EndSpinVar = 0;

            // color of particles
            var startColor = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColor = startColor;

            var startColorVar = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColorVar = startColorVar;

            var endColor = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColor = endColor;

            var endColorVar = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColorVar = endColorVar;

            // size, in pixels
            Emitter.StartSize = 32;
            Emitter.StartSizeVar = 0;
            Emitter.EndSize = CCParticleSystem.ParticleStartSizeEqualToEndSize;

            // emits per second
            Emitter.EmissionRate = Emitter.TotalParticles / Emitter.Life;

            // additive
            Emitter.BlendAdditive = false;
        }

        protected override string Title()
        {
            return "Radius Mode: Spiral";
        }
    };

    public class RadiusMode2 : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = new CCColor3B(0, 0, 0);
            RemoveChild(Background, true);
            Background = null;

            Emitter = new CCParticleSystemQuad(200, CCEmitterMode.Radius);
            AddChild(Emitter, 10);
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage("Images/stars-grayscale");

            // duration
            Emitter.Duration = CCParticleSystem.ParticleDurationInfinity;

            // radius mode: start and end radius in pixels
            Emitter.StartRadius = (100);
            Emitter.StartRadiusVar = (0);
            Emitter.EndRadius = (CCParticleSystem.ParticleStartRadiusEqualToEndRadius);
            Emitter.EndRadiusVar = (0);

            // radius mode: degrees per second
            Emitter.RotatePerSecond = (45);
            Emitter.RotatePerSecondVar = (0);


            // angle
            Emitter.Angle = 90;
            Emitter.AngleVar = 0;

            // emitter position
            CCSize size = CCApplication.SharedApplication.MainWindowDirector.WindowSizeInPoints;
            Emitter.Position = new CCPoint(size.Width / 2, size.Height / 2);
            Emitter.PositionVar = new CCPoint(0, 0);

            // life of particles
            Emitter.Life = 4;
            Emitter.LifeVar = 0;

            // spin of particles
            Emitter.StartSpin = 0;
            Emitter.StartSpinVar = 0;
            Emitter.EndSpin = 0;
            Emitter.EndSpinVar = 0;

            // color of particles
            var startColor = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColor = startColor;

            var startColorVar = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColorVar = startColorVar;

            var endColor = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColor = endColor;

            var endColorVar = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColorVar = endColorVar;

            // size, in pixels
            Emitter.StartSize = 32;
            Emitter.StartSizeVar = 0;
            Emitter.EndSize = CCParticleSystem.ParticleStartSizeEqualToEndSize;

            // emits per second
            Emitter.EmissionRate = Emitter.TotalParticles / Emitter.Life;

            // additive
            Emitter.BlendAdditive = false;
        }

        protected override string Title()
        {
            return "Radius Mode: Semi Circle";
        }
    }

    public class Issue704 : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = new CCColor3B(0, 0, 0);
            RemoveChild(Background, true);
            Background = null;

            Emitter = new CCParticleSystemQuad(100, CCEmitterMode.Radius);
            AddChild(Emitter, 10);
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage("Images/fire");

            // duration
            Emitter.Duration = CCParticleSystem.ParticleDurationInfinity; 

            // radius mode: start and end radius in pixels
            Emitter.StartRadius = (50);
            Emitter.StartRadiusVar = (0);
            Emitter.EndRadius = (CCParticleSystem.ParticleStartRadiusEqualToEndRadius);
            Emitter.EndRadiusVar = (0);

            // radius mode: degrees per second
            Emitter.RotatePerSecond = (0);
            Emitter.RotatePerSecondVar = (0);


            // angle
            Emitter.Angle = 90;
            Emitter.AngleVar = 0;

            // emitter position
            CCSize size = CCApplication.SharedApplication.MainWindowDirector.WindowSizeInPoints;
            Emitter.Position = new CCPoint(size.Width / 2, size.Height / 2);
            Emitter.PositionVar = new CCPoint(0, 0);

            // life of particles
            Emitter.Life = 5;
            Emitter.LifeVar = 0;

            // spin of particles
            Emitter.StartSpin = 0;
            Emitter.StartSpinVar = 0;
            Emitter.EndSpin = 0;
            Emitter.EndSpinVar = 0;

            // color of particles
            var startColor = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColor = startColor;

            var startColorVar = new CCColor4F(0.5f, 0.5f, 0.5f, 1.0f);
            Emitter.StartColorVar = startColorVar;

            var endColor = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColor = endColor;

            var endColorVar = new CCColor4F(0.1f, 0.1f, 0.1f, 0.2f);
            Emitter.EndColorVar = endColorVar;

            // size, in pixels
            Emitter.StartSize = 16;
            Emitter.StartSizeVar = 0;
            Emitter.EndSize = CCParticleSystem.ParticleStartSizeEqualToEndSize;

            // emits per second
            Emitter.EmissionRate = Emitter.TotalParticles / Emitter.Life;

            // additive
            Emitter.BlendAdditive = false;

            CCRotateBy rot = new CCRotateBy (16, 360);
            Emitter.RunAction(new CCRepeatForever (rot));
        }

        protected override string Title()
        {
            return "Issue 704. Free + Rot";
        }

        protected override string Subtitle()
        {
            return "Emitted particles should not rotate";
        }
    }

    public class Issue870 : ParticleDemo
    {
        int index;

        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = new CCColor3B(0, 0, 0);
            RemoveChild(Background, true);
            Background = null;

            var system = new CCParticleSystemQuad("Particles/SpinningPeas");
            system.Texture = (CCApplication.SharedApplication.TextureCache.AddImage ("Images/particles"));
            system.TextureRect = new CCRect(0, 0, 32, 32);

            AddChild(system, 10);
            Emitter = system;

            index = 0;
            Schedule(UpdateQuads, 2.0f);
        }

        void UpdateQuads(float dt)
        {
            index = (index + 1) % 4;
            var rect = new CCRect(index * 32, 0, 32, 32);
            var system = (CCParticleSystemQuad) Emitter;
            system.Texture = Emitter.Texture;
            system.TextureRect = rect;
        }

        protected override string Title()
        {
            return "Issue 870. SubRect";
        }

        protected override string Subtitle()
        {
            return "Every 2 seconds the particle should change";
        }
    }

    public class ParticleReorder : ParticleDemo
    {
        int order;
        CCLabelTtf label;

        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            order = 0;
            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            label = new CCLabelTtf("Loading...", "Marker Felt", 32);
            label.Position = windowSize.Center;
            label.Visible = false;
            AddChild(label, 10);

            var scale = new CCScaleBy(0.3f, 2);
            label.RunActions (new CCDelayTime (2.0f), new CCShow ());
            label.RepeatForever (scale, scale.Reverse ());

            ScheduleOnce(LoadParticleSystem, 0.0f);
        }

        void LoadParticleSystem(float dt)
        {
            CCApplication.SharedApplication.ParticleSystemCache.AddParticleSystemAsync("Particles/SmallSun", ParticleSystemLoaded);
        }

        void ParticleSystemLoaded(CCParticleSystemConfig psConfig)
        {
            label.RemoveFromParent(true);

            CCParticleSystemQuad ignore = new CCParticleSystemQuad (psConfig);

            //ignore.TotalParticles = 200;
            CCNode parent1 = new CCNode ();
            CCParticleBatchNode parent2 = new CCParticleBatchNode (ignore.Texture);

            for (int i = 0; i < 2; i++) {
                CCNode parent = (i == 0 ? parent1 : parent2);

                CCParticleSystemQuad emitter1 = new CCParticleSystemQuad (psConfig);
                //emitter1.TotalParticles = 200;
                emitter1.StartColor = (new CCColor4F (1, 0, 0, 1));
                emitter1.BlendAdditive = (false);
                CCParticleSystemQuad emitter2 = new CCParticleSystemQuad (psConfig);
                //emitter2.TotalParticles = 200;
                emitter2.StartColor = (new CCColor4F (0, 1, 0, 1));
                emitter2.BlendAdditive = (false);
                CCParticleSystemQuad emitter3 = new CCParticleSystemQuad (psConfig);
                //emitter3.TotalParticles = 200;
                emitter3.StartColor = (new CCColor4F (0, 0, 1, 1));
                emitter3.BlendAdditive = (false);

                CCSize s = CCApplication.SharedApplication.MainWindowDirector.WindowSizeInPoints;

                int neg = (i == 0 ? 1 : -1);

                emitter1.Position = (new CCPoint (s.Width / 2 - 30, s.Height / 2 + 60 * neg));
                emitter2.Position = (new CCPoint (s.Width / 2, s.Height / 2 + 60 * neg));
                emitter3.Position = (new CCPoint (s.Width / 2 + 30, s.Height / 2 + 60 * neg));

                parent.AddChild (emitter1, 0, 1);
                parent.AddChild (emitter2, 0, 2);
                parent.AddChild (emitter3, 0, 3);

                AddChild (parent, 10, 1000 + i);
            }

            Schedule(ReorderParticles, 1.0f);
        }

        protected override string Title()
        {
            return "Reordering particles";
        }

        protected override string Subtitle()
        {
            return "Reordering particles with and without batches batches";
        }

        void ReorderParticles(float dt)
        {
            for (int i = 0; i < 2; i++)
            {
                CCNode parent = GetChildByTag(1000 + i);

                CCNode child1 = parent.GetChildByTag(1);
                CCNode child2 = parent.GetChildByTag(2);
                CCNode child3 = parent.GetChildByTag(3);

                if (order % 3 == 0)
                {
                    parent.ReorderChild(child1, 1);
                    parent.ReorderChild(child2, 2);
                    parent.ReorderChild(child3, 3);
                }
                else if (order % 3 == 1)
                {
                    parent.ReorderChild(child1, 3);
                    parent.ReorderChild(child2, 1);
                    parent.ReorderChild(child3, 2);
                }
                else if (order % 3 == 2)
                {
                    parent.ReorderChild(child1, 2);
                    parent.ReorderChild(child2, 3);
                    parent.ReorderChild(child3, 1);
                }
            }

            order++;
        }
    }


    public class ParticleBatchHybrid : ParticleDemo
    {
        CCNode parent1;
        CCNode parent2;


        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            Emitter = new CCParticleSystemQuad("Particles/LavaFlow");
            Emitter.Texture = CCApplication.SharedApplication.TextureCache.AddImage("Images/fire");
            CCParticleBatchNode batch = new CCParticleBatchNode(Emitter.Texture);

            batch.AddChild(Emitter);

            AddChild(batch, 10);

            Schedule(SwitchRender, 2.0f);

            CCLayer node = new CCLayer();
            AddChild(node);

            parent1 = batch;
            parent2 = node;
        }

        void SwitchRender(float dt)
        {
            bool usingBatch = (Emitter.BatchNode != null);
            Emitter.RemoveFromParent(false);

            CCNode newParent = (usingBatch ? parent2 : parent1);
            newParent.AddChild(Emitter);

            CCLog.Log("Particle: Using new parent: {0}", usingBatch ? "CCNode" : "CCParticleBatchNode");
        }

        protected override string Title()
        {
            return "Paticle Batch";
        }

        protected override string Subtitle()
        {
            return "Hybrid: batched and non batched every 2 seconds";
        }
    }

    public class ParticleBatchMultipleEmitters : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            CCParticleSystemQuad emitter1 = new CCParticleSystemQuad("Particles/LavaFlow");
            emitter1.StartColor = (new CCColor4F(1, 0, 0, 1));
            CCParticleSystemQuad emitter2 = new CCParticleSystemQuad("Particles/LavaFlow");
            emitter2.StartColor = (new CCColor4F(0, 1, 0, 1));
            CCParticleSystemQuad emitter3 = new CCParticleSystemQuad("Particles/LavaFlow");
            emitter3.StartColor = (new CCColor4F(0, 0, 1, 1));

            CCSize s = CCApplication.SharedApplication.MainWindowDirector.WindowSizeInPoints;

            emitter1.Position = (new CCPoint(s.Width / 1.25f, s.Height / 1.25f));
            emitter2.Position = (new CCPoint(s.Width / 2, s.Height / 2));
            emitter3.Position = (new CCPoint(s.Width / 4, s.Height / 4));

            emitter1.Texture = CCApplication.SharedApplication.TextureCache.AddImage("Images/fire");
            emitter2.Texture = emitter1.Texture;
            emitter3.Texture = emitter1.Texture;

            CCParticleBatchNode batch = new CCParticleBatchNode(emitter1.Texture);

            batch.AddChild(emitter1, 0);
            batch.AddChild(emitter2, 0);
            batch.AddChild(emitter3, 0);

            AddChild(batch, 10);
        }

        protected override string Title()
        {
            return "Paticle Batch";
        }

        protected override string Subtitle()
        {
            return "Multiple emitters. One Batch";
        }
    }

    public class RainbowEffect : CCParticleSystemQuad
    {
        public bool init()
        {
            return true;
        }

        public RainbowEffect(int numOfParticles) : base(numOfParticles)
        {
            // additive
            BlendAdditive = (false);

            // duration
            Duration = (ParticleDurationInfinity);;

            // Gravity Mode: gravity
            Gravity = (new CCPoint(0, 0));

            // Gravity mode: radial acceleration
            RadialAccel = (0);
            RadialAccelVar = (0);

            // Gravity mode: speed of particles
            Speed = (120);
            SpeedVar = (0);


            // angle
            Angle = (180);
            AngleVar = (0);



            // life of particles
            Life = (0.5f);
            LifeVar = (0);

            // size, in pixels
            StartSize = (25.0f);
            StartSizeVar = (0);
            EndSize = (ParticleStartSizeEqualToEndSize);

            // emits per seconds
            EmissionRate = (TotalParticles / Life);

            // color of particles
            StartColor = (new CCColor4F(50, 50, 50, 50));
            EndColor = (new CCColor4F(0, 0, 0, 0));
            StartColorVar = new CCColor4F();
            EndColorVar = new CCColor4F();

            Texture = (CCApplication.SharedApplication.TextureCache.AddImage("Images/particles"));
        }

        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            // emitter position
            Position = (new CCPoint(windowSize.Width / 2, windowSize.Height / 2));
            PositionVar = (CCPoint.Zero);
        }

        public override void Update(float dt)
        {
            EmitCounter = 0;
            base.Update(dt);
        }
    }

    public class Issue1201 : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            var particle = new RainbowEffect(50);

            AddChild(particle);

            particle.Position = (MidWindowPoint);

            Emitter = particle;
        }

        protected override string Title()
        {
            return "Issue 1201. Unfinished";
        }

        protected override string Subtitle()
        {
            return "Unfinished test. Ignore it";
        }
    }

    public class MultipleParticleSystems : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            CCApplication.SharedApplication.TextureCache.AddImage("Images/particles");

            for (int i = 0; i < 5; i++)
            {
                CCParticleSystemQuad particleSystem = new CCParticleSystemQuad("Particles/SpinningPeas");

                particleSystem.Position = (new CCPoint(i * 50, i * 50));

                particleSystem.PositionType = CCPositionType.Grouped;
                AddChild(particleSystem);
            }

            Emitter = null;
        }

        protected override string Title()
        {
            return "Multiple particle systems";
        }

        protected override string Subtitle()
        {
            return "v1.1 test: FPS should be lower than next test";
        }
    }

    public class MultipleParticleSystemsBatched : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            CCParticleBatchNode batchNode = new CCParticleBatchNode("Images/fire", 3000);

            AddChild(batchNode, 1, 2);

            for (int i = 0; i < 5; i++)
            {
                CCParticleSystemQuad particleSystem = new CCParticleSystemQuad("Particles/SpinningPeas");

                particleSystem.PositionType = CCPositionType.Grouped;
                particleSystem.Position = (new CCPoint(i * 50, i * 50));

                particleSystem.Texture = batchNode.Texture;
                batchNode.AddChild(particleSystem);
            }


            Emitter = null;
        }

        protected override string Title()
        {
            return "Multiple particle systems";
        }

        protected override string Subtitle()
        {
            return "v1.1 test: FPS should be lower than next test";
        }
    }


    public class AddAndDeleteParticleSystems : ParticleDemo
    {
        CCParticleBatchNode batchNode;

        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            //adds the texture inside the plist to the texture cache
            batchNode = new CCParticleBatchNode("Images/fire", 16000);

            AddChild(batchNode, 1, 2);

            for (int i = 0; i < 6; i++)
            {
                CCParticleSystemQuad particleSystem = new CCParticleSystemQuad("Particles/Spiral");
                particleSystem.Texture = batchNode.Texture;

                particleSystem.PositionType = CCPositionType.Grouped;
                particleSystem.TotalParticles = (200);

                particleSystem.Position = (new CCPoint(i * 15 + 100, i * 15 + 100));

                int randZ = CCRandom.Next(100);
                batchNode.AddChild(particleSystem, randZ, -1);
            }

            Schedule(RemoveSystem, 0.5f);
            Emitter = null;
        }

        void RemoveSystem(float dt)
        {
            int nChildrenCount = batchNode.ChildrenCount;
            if (nChildrenCount > 0)
            {
                CCLog.Log("remove random system");
                int uRand = CCRandom.Next(nChildrenCount - 1);
                batchNode.RemoveChild(batchNode.Children[uRand], true);

                CCParticleSystemQuad particleSystem = new CCParticleSystemQuad("Particles/Spiral");
                //add new

                particleSystem.PositionType = CCPositionType.Grouped;
                particleSystem.TotalParticles = (200);

                particleSystem.Position = (new CCPoint(CCRandom.Next(300), CCRandom.Next(400)));

                CCLog.Log("add a new system");
                int randZ = CCRandom.Next(100);
                particleSystem.Texture = batchNode.Texture;
                batchNode.AddChild(particleSystem, randZ, -1);
            }
        }

        protected override string Title()
        {
            return "Add and remove Particle System";
        }

        protected override string Subtitle()
        {
            return "v1.1 test: every 2 sec 1 system disappear, 1 appears";
        }
    }

    public class ReorderParticleSystems : ParticleDemo
    {
        CCParticleBatchNode batchNode;

        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            batchNode = new CCParticleBatchNode("Images/stars-grayscale", 3000);

            AddChild(batchNode, 1, 2);


            for (int i = 0; i < 3; i++)
            {
                var particleSystem = new CCParticleSystemQuad(200, CCEmitterMode.Radius);
                particleSystem.Texture = (batchNode.Texture);

                // duration
                particleSystem.Duration = CCParticleSystem.ParticleDurationInfinity;

                // radius mode: 100 pixels from center
                particleSystem.StartRadius = (100);
                particleSystem.StartRadiusVar = (0);
                particleSystem.EndRadius = (CCParticleSystem.ParticleStartRadiusEqualToEndRadius);
                particleSystem.EndRadiusVar = (0); // not used when start == end

                // radius mode: degrees per second
                // 45 * 4 seconds of life = 180 degrees
                particleSystem.RotatePerSecond = (45);
                particleSystem.RotatePerSecondVar = (0);


                // angle
                particleSystem.Angle = (90);
                particleSystem.AngleVar = (0);

                // emitter position
                particleSystem.PositionVar = (CCPoint.Zero);

                // life of particles
                particleSystem.Life = (4);
                particleSystem.LifeVar = (0);

                // spin of particles
                particleSystem.StartSpin = (0);
                particleSystem.StartSpinVar = (0);
                particleSystem.EndSpin = (0);
                particleSystem.EndSpinVar = (0);

                // color of particles
                var color = new float[3] {0, 0, 0};
                color[i] = 1;
                var startColor = new CCColor4F(color[0], color[1], color[2], 1.0f);
                particleSystem.StartColor = (startColor);

                var startColorVar = new CCColor4F(0, 0, 0, 0);
                particleSystem.StartColorVar = (startColorVar);

                CCColor4F endColor = startColor;
                particleSystem.EndColor = (endColor);

                CCColor4F endColorVar = startColorVar;
                particleSystem.EndColorVar = (endColorVar);

                // size, in pixels
                particleSystem.StartSize = (32);
                particleSystem.StartSizeVar = (0);
                particleSystem.EndSize = CCParticleSystem.ParticleStartSizeEqualToEndSize;

                // emits per second
                particleSystem.EmissionRate = (particleSystem.TotalParticles / particleSystem.Life);

                // additive

                particleSystem.Position = (new CCPoint(i * 10 + 120, 200));


                batchNode.AddChild(particleSystem);
                particleSystem.PositionType = CCPositionType.Free;
            }

            Schedule(ReorderSystem, 2.0f);
            Emitter = null;
        }

        void ReorderSystem(float dt)
        {
            var system = (CCParticleSystem) batchNode.Children[1];
            batchNode.ReorderChild(system, system.ZOrder - 1);
        }

        protected override string Title()
        {
            return "reorder systems";
        }

        protected override string Subtitle()
        {
            return "changes every 2 seconds";
        }
    }

    public class PremultipliedAlphaTest : ParticleDemo
    {
        public override void OnEnter()
        {
            base.OnEnter();

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            Emitter = new CCParticleSystemQuad("Particles/BoilingFoam");

            // Particle Designer "normal" blend func causes black halo on premul textures (ignores multiplication)
            //this->emitter.blendFunc = (ccBlendFunc){ GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA };

            // Cocos2d "normal" blend func for premul causes alpha to be ignored (oversaturates colors)
            var tBlendFunc = new CCBlendFunc(CCOGLES.GL_ONE, CCOGLES.GL_ONE_MINUS_SRC_ALPHA);
            Emitter.BlendFunc = tBlendFunc;

            //Debug.Assert(Emitter.OpacityModifyRGB, "Particle texture does not have premultiplied alpha, test is useless");

            // Toggle next line to see old behavior
            //  this->emitter.opacityModifyRGB = NO;

            Emitter.StartColor = new CCColor4F(1, 1, 1, 1);
            Emitter.EndColor = new CCColor4F(1, 1, 1, 0);
            Emitter.StartColorVar = new CCColor4F(0, 0, 0, 0);
            Emitter.EndColorVar = new CCColor4F(0, 0, 0, 0);

            AddChild(Emitter, 10);
        }


        protected override string Title()
        {
            return "premultiplied alpha";
        }

        protected override string Subtitle()
        {
            return "no black halo, particles should fade out";
        }
    }

    public class PremultipliedAlphaTest2 : ParticleDemo
    {
        protected override void RunningOnNewWindow(CCSize windowSize)
        {
            base.RunningOnNewWindow(windowSize);

            Color = CCColor3B.Black;
            RemoveChild(Background, true);
            Background = null;

            Emitter = new CCParticleSystemQuad("Particles/TestPremultipliedAlpha");
            AddChild(Emitter, 10);
        }


        protected override string Title()
        {
            return "premultiplied alpha 2";
        }

        protected override string Subtitle()
        {
            return "Arrows should be faded";
        }
    }
}