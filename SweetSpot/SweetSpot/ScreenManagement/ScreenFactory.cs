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
        public static Screen CreateTitleScreen(ScreenManager sm)
        {
            return new TitleScreen(sm);
        }

        public static Screen CreateCalibrationScreen(ScreenManager sm)
        {
            return new SensorCalibrationScreen(sm);
        }

        public static Screen CreateQuestionScreen(ScreenManager sm, Cue cue, Mapping mapping, Vector2 sweetspot)
        {
            Screen screen;
            Effect effect;

            switch (cue)
            {
                case Cue.BaselineArrow:
                    screen = new BaselineArrowScreen(sm, StringEnum.GetStringValue(cue), mapping, sweetspot);
                    break;
                case Cue.BaselineText:
                    screen = new BaselineTextScreen(sm, StringEnum.GetStringValue(cue), mapping, sweetspot);
                    break;
                case Cue.Brightness:
                    effect = sm.Game.Content.Load<Effect>(@"shader\BrightnessShader");
                    screen = new EffectScreen(sm, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Contrast:
                    effect = sm.Game.Content.Load<Effect>(@"shader\ContrastShader");
                    screen = new EffectScreen(sm, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Saturation:
                    effect = sm.Game.Content.Load<Effect>(@"shader\SaturationShader");
                    screen = new EffectScreen(sm, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Pixelate:
                    effect = sm.Game.Content.Load<Effect>(@"shader\PixelateShader");
                    screen = new EffectScreen(sm, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Distort:
                    effect = sm.Game.Content.Load<Effect>(@"shader\DistortShader");
                    screen = new EffectScreen(sm, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Jitter:
                    effect = sm.Game.Content.Load<Effect>(@"shader\JitterShader");
                    screen = new EffectScreen(sm, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                default:
                    throw new ArgumentException("Invalid cue type.");
            }

            return screen;
        }
    }
}
