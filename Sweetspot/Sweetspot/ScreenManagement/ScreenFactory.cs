﻿using System;
using Microsoft.Xna.Framework.Graphics;
using SweetspotApp.ScreenManagement.Screens;
using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement
{
    public class ScreenFactory
    {
        public static Screen CreateTitleScreen(GameController gc)
        {
            return new TitleScreen(gc);
        }

        public static Screen CreateCalibrationScreen(GameController gc)
        {
            return new CalibrationScreen(gc);
        }

        public static Screen CreateQuestionScreen(GameController gc, Cue cue, Mapping mapping, Sweetspot sweetspot)
        {
            Screen screen;
            Effect effect;

            switch (cue)
            {
                case Cue.BaselineArrow:
                    screen = new BaselineArrowScreen(gc, StringEnum.GetStringValue(cue), mapping, sweetspot);
                    break;
                case Cue.BaselineText:
                    screen = new BaselineTextScreen(gc, StringEnum.GetStringValue(cue), mapping, sweetspot);
                    break;
                case Cue.Brightness:
                    effect = gc.Game.Content.Load<Effect>(@"shader\BrightnessShader");
                    screen = new EffectScreen(gc, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Contrast:
                    effect = gc.Game.Content.Load<Effect>(@"shader\ContrastShader");
                    screen = new EffectScreen(gc, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Saturation:
                    effect = gc.Game.Content.Load<Effect>(@"shader\SaturationShader");
                    screen = new EffectScreen(gc, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Pixelate:
                    effect = gc.Game.Content.Load<Effect>(@"shader\PixelateShader");
                    screen = new EffectScreen(gc, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Distort:
                    effect = gc.Game.Content.Load<Effect>(@"shader\DistortShader");
                    screen = new EffectScreen(gc, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                case Cue.Jitter:
                    effect = gc.Game.Content.Load<Effect>(@"shader\JitterShader");
                    screen = new EffectScreen(gc, StringEnum.GetStringValue(cue), mapping, sweetspot, effect);
                    break;
                default:
                    throw new ArgumentException("Invalid cue type.");
            }

            return screen;
        }
    }
}
