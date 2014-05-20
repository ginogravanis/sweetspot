//#define CALIBRATION_TARGET

using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SweetSpot.ScreenManagement;

namespace SweetSpot
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (SweetSpot app = new SweetSpot())
            {
                try
                {
                    app.Run();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
    }

    public class SweetSpot : Microsoft.Xna.Framework.Game
    {
        public SweetSpot()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Content.RootDirectory = "Content";
            Window.Title = "SweetSpot 2.0";
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.SynchronizeWithVerticalRetrace = true;

#if CALIBRATION_TARGET
            IsMouseVisible = true;
            Components.Add(new ScreenManager(this, Scene.Calibration));
#else
            Components.Add(new ScreenManager(this, Scene.Tests));
#endif
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
