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

        public static IEnumerable<Screen> CreateCalibration(ScreenManager screenManager)
        {
            var calibration = new List<Screen>();
            calibration.Add(CreateTransitionScreen(screenManager, "Calibration"));
            calibration.Add(CreateCalibrationScreen(screenManager));
            return calibration;
        }

        public static Screen CreateCalibrationScreen(ScreenManager screenManager)
        {
            return new SensorCalibrationScreen(screenManager);
        }

        public static Screen CreateTestScreen(ScreenManager screenManager, Cue cue, Mapping mapping, Vector2 sweetSpot)
        {
            Screen screen;
            Effect effect;

            switch (cue)
            {
                case Cue.BaselineArrow:
                    screen = new BaselineArrowScreen(screenManager, StringEnum.GetStringValue(cue), mapping, sweetSpot);
                    break;
                case Cue.BaselineText:
                    screen = new BaselineTextScreen(screenManager, StringEnum.GetStringValue(cue), mapping, sweetSpot);
                    break;
                case Cue.Brightness:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\BrightnessShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), mapping, sweetSpot, effect);
                    break;
                case Cue.Contrast:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\ContrastShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), mapping, sweetSpot, effect);
                    break;
                case Cue.Saturation:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\SaturationShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), mapping, sweetSpot, effect);
                    break;
                case Cue.Pixelate:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\PixelateShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), mapping, sweetSpot, effect);
                    break;
                case Cue.Distort:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\DistortShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), mapping, sweetSpot, effect);
                    break;
                case Cue.Jitter:
                    effect = screenManager.Game.Content.Load<Effect>(@"shader\JitterShader");
                    screen = new EffectScreen(screenManager, StringEnum.GetStringValue(cue), mapping, sweetSpot, effect);
                    break;
                default:
                    throw new ArgumentException("Invalid cue type.");
            }

            return screen;
        }
    }
}
