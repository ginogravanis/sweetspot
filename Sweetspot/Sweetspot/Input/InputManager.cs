using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SweetspotApp.Input
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
            keyboardState = Keyboard.GetState();
            previousKeyboardState = new KeyboardState();

            mouseState = Mouse.GetState();
            previousMouseState = new MouseState();

            gamePadState = GamePad.GetState(PlayerIndex.One);
            previousGamePadState = new GamePadState();
        }

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            previousMouseState = mouseState;
            mouseState = Mouse.GetState();

            previousGamePadState = gamePadState;
            gamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public Keys[] GetPreviousDownKeys()
        {
            return previousKeyboardState.GetPressedKeys();
        }

        public Keys[] GetDownKeys()
        {
            return keyboardState.GetPressedKeys();
        }

        public bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        protected bool IsKeyUp(Keys key)
        {
            return keyboardState.IsKeyUp(key);
        }

        protected bool WasKeyDown(Keys key)
        {
            return WasKeyDown(key);
        }

        protected bool WasKeyUp(Keys key)
        {
            return previousKeyboardState.IsKeyUp(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return IsKeyDown(key) && WasKeyUp(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return IsKeyUp(key) && WasKeyDown(key);
        }

        public bool IsLeftMouseButtonDown()
        {
            return mouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsMiddleMouseButtonDown()
        {
            return mouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsRightMouseButtonDown()
        {
            return mouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsRightMouseButtonPressed()
        {
            return IsRightMouseButtonDown() && (previousMouseState.RightButton == ButtonState.Released);
        }

        public bool IsLeftMouseButtonPressed()
        {
            return IsLeftMouseButtonDown() && (previousMouseState.LeftButton == ButtonState.Released);
        }

        public bool IsMiddleMouseButtonPressed()
        {
            return IsMiddleMouseButtonDown() && (previousMouseState.MiddleButton == ButtonState.Released);
        }

        public bool IsGamePadButtonDown(Buttons button)
        {
            return gamePadState.IsButtonDown(button);
        }

        public bool IsGamePadButtonPressed(Buttons button)
        {
            return IsGamePadButtonDown(button) && (previousGamePadState.IsButtonUp(button));
        }
    }
}
