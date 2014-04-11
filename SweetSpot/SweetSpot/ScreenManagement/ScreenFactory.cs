using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.ScreenManagement.Screens;
using SweetSpot.Util;

namespace SweetSpot.ScreenManagement
{
    public class ScreenFactory
    {
        public static Screen CreateTransitionScreen(ScreenManager screenManager, string caption)
        {
            return new TransitionScreen(screenManager, caption);
        }

        public static Screen CreateAutoTransitionScreen(ScreenManager screenManager, string caption)
        {
            return new AutoTransitionScreen(screenManager, caption);
        }

        public static Screen CreateCalibrationScreen(ScreenManager screenManager)
        {
            return new SensorCalibrationScreen(screenManager);
        }

        public static IEnumerable<Screen> CreateSnellenTest(ScreenManager screenManager)
        {
            var snellenTest = new List<Screen>();
            snellenTest.Add(CreateTransitionScreen(screenManager, "Snellen Test"));
            snellenTest.Add(CreateSlideScreen(screenManager, @"texture\vision\snellen"));
            return snellenTest;
        }

        public static IEnumerable<Screen> CreateIshiharaTest(ScreenManager screenManager)
        {
            var ishiharaTest = new List<Screen>();
            ishiharaTest.Add(CreateTransitionScreen(screenManager, "Ishihara Test"));
            for (int i = 1; i <= 17; i++)
            {
                ishiharaTest.Add(CreateAutoTransitionScreen(screenManager, String.Format("Ishihara Plate {0}/17", i)));
                ishiharaTest.Add(CreateTimedSlideScreen(screenManager, String.Format(@"texture\vision\ishihara\Plate{0}", i), 3));
            }
            return ishiharaTest;
        }

        public static IEnumerable<Screen> CreatePelliRobsonTest(ScreenManager screenManager)
        {
            var pelliRobsonTest = new List<Screen>();
            pelliRobsonTest.Add(CreateTransitionScreen(screenManager, "Pelli-Robson Test"));
            pelliRobsonTest.Add(CreateSlideScreen(screenManager, @"texture\vision\pelli-robson"));
            return pelliRobsonTest;
        }

        public static Screen CreateDemoScreen(ScreenManager screenManager)
        {
            bool shuffleItems = false;
            Screen screen = new ImageScreen(screenManager, StringEnum.GetStringValue(Cue.Baseline), new Vector2(), shuffleItems);
            return screen;
        }

        public static Screen CreateSlideScreen(ScreenManager screenManager, string imagePath)
        {
            Texture2D image = screenManager.Game.Content.Load<Texture2D>(imagePath);
            return new SlideScreen(screenManager, image);
        }

        public static Screen CreateTimedSlideScreen(ScreenManager screenManager, string imagePath, float seconds)
        {
            Texture2D image = screenManager.Game.Content.Load<Texture2D>(imagePath);
            return new TimedSlideScreen(screenManager, image, seconds);
        }

        public static Screen CreateTestScreen(ScreenManager screenManager, Cue cue, Vector2 sweetSpot)
        {
            Screen screen;
            Effect effect;

            switch (cue)
            {
                case Cue.Baseline:
                    screen = new ImageScreen(screenManager, StringEnum.GetStringValue(cue), new Vector2());
                    break;
                case Cue.BaselineArrow:
                    screen = new BaselineArrowScreen(screenManager, StringEnum.GetStringValue(cue), sweetSpot);
                    break;
                case Cue.BaselineText:
                    screen = new BaselineTextScreen(screenManager, StringEnum.GetStringValue(cue), sweetSpot);
                    break;
                case Cue.Brightness:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\BrightnessShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), sweetSpot, effect);
                    break;
                case Cue.Contrast:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\ContrastShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), sweetSpot, effect);
                    break;
                case Cue.Saturation:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\SaturationShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), sweetSpot, effect);
                    break;
                case Cue.Pixelate:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\PixelateShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), sweetSpot, effect);
                    break;
                case Cue.Distort:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\DistortShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), sweetSpot, effect);
                    break;
                case Cue.Jitter:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\JitterShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), sweetSpot, effect);
                    break;
                default:
                    throw new ArgumentException("Invalid cue type.");
            }

            return screen;
        }
    }
}
