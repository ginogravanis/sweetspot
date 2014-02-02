using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SweetSpot_2._0
{
    public class InputManager
    {
        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;

        MouseState mouseState;
        MouseState previousMouseState;

        GamePadState gamePadState;
        GamePadState previousGamePadState;

        public InputManager()
        {
            this.keyboardState = Keyboard.GetState();
            this.previousKeyboardState = new KeyboardState();

            this.mouseState = Mouse.GetState();
            this.previousMouseState = new MouseState();

            this.gamePadState = GamePad.GetState(PlayerIndex.One);
            this.previousGamePadState = new GamePadState();
        }

        public void Update(GameTime gameTime)
        {
            this.previousKeyboardState = this.keyboardState;
            this.keyboardState = Keyboard.GetState();

            this.previousMouseState = this.mouseState;
            this.mouseState = Mouse.GetState();

            this.previousGamePadState = this.gamePadState;
            this.gamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public Keys[] GetPreviousDownKeys()
        {
            return this.previousKeyboardState.GetPressedKeys();
        }

        public Keys[] GetDownKeys()
        {
            return this.keyboardState.GetPressedKeys();
        }

        public bool IsKeyDown(Keys key)
        {
            return this.keyboardState.IsKeyDown(key);
        }

        private bool IsKeyUp(Keys key)
        {
            return this.keyboardState.IsKeyUp(key);
        }

        private bool WasKeyDown(Keys key)
        {
            return this.WasKeyDown(key);
        }

        private bool WasKeyUp(Keys key)
        {
            return this.previousKeyboardState.IsKeyUp(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return this.IsKeyDown(key) && this.WasKeyUp(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return this.IsKeyUp(key) && this.WasKeyDown(key);
        }

        public bool IsLeftMouseButtonDown()
        {
            return this.mouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsMiddleMouseButtonDown()
        {
            return this.mouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsRightMouseButtonDown()
        {
            return this.mouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsRightMouseButtonPressed()
        {
            return this.IsRightMouseButtonDown() && (this.previousMouseState.RightButton == ButtonState.Released);
        }

        public bool IsLeftMouseButtonPressed()
        {
            return this.IsLeftMouseButtonDown() && (this.previousMouseState.LeftButton == ButtonState.Released);
        }

        public bool IsMiddleMouseButtonPressed()
        {
            return IsMiddleMouseButtonDown() && (this.previousMouseState.MiddleButton == ButtonState.Released);
        }

        public bool IsGamePadButtonDown(Buttons button)
        {
            return this.gamePadState.IsButtonDown(button);
        }

        public bool IsGamePadButtonPressed(Buttons button)
        {
            return this.IsGamePadButtonDown(button) && (this.previousGamePadState.IsButtonUp(button));
        }
    }
}
