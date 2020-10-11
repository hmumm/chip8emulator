using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading;
using Serilog;

namespace HJM.Chip8.MonoGameUI
{
    public class Game1 : Game
    {
        private const int ClockSpeed = 500;

        private readonly GraphicsDeviceManager _graphics;
        private readonly Thread _emulatorThread;
        private readonly CPU.Chip8 _chip8;

        private SpriteBatch? _spriteBatch;
        private Texture2D? _whiteRectangle;
        private bool _threadStopped = false;
        private byte[] _displayBuffer = new byte[4096];
        private byte _bufferedFrameMask = 0b_1111_0000;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _emulatorThread = new Thread(RunCycles);
            _chip8 = new CPU.Chip8();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _chip8.Initalize();
            _chip8.LoadGame(@"C:\Users\Hayden\Downloads\myChip8-bin-src\myChip8-bin-src\PONG2.c8");

            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            _emulatorThread.Start();
            Log.Information("Emulator thread started.");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _whiteRectangle = new Texture2D(GraphicsDevice, width: 1, height: 1);
            _whiteRectangle.SetData(new[] { Color.White });
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            if (_whiteRectangle != null)
                _whiteRectangle.Dispose();

            if (_spriteBatch != null)
                _spriteBatch.Dispose();

            _threadStopped = true;
            Log.Information("Emulator thread stopped.");
        }

        protected override void Update(GameTime gameTime)
        {
            // Poll for current keyboard state
            KeyboardState state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            // set all the keys
            _chip8.State.Key[0] = Convert.ToByte(state.IsKeyDown(Keys.X));
            _chip8.State.Key[1] = Convert.ToByte(state.IsKeyDown(Keys.D1));
            _chip8.State.Key[2] = Convert.ToByte(state.IsKeyDown(Keys.D2));
            _chip8.State.Key[3] = Convert.ToByte(state.IsKeyDown(Keys.D3));
            _chip8.State.Key[4] = Convert.ToByte(state.IsKeyDown(Keys.Q));
            _chip8.State.Key[5] = Convert.ToByte(state.IsKeyDown(Keys.W));
            _chip8.State.Key[6] = Convert.ToByte(state.IsKeyDown(Keys.E));
            _chip8.State.Key[7] = Convert.ToByte(state.IsKeyDown(Keys.A));
            _chip8.State.Key[8] = Convert.ToByte(state.IsKeyDown(Keys.S));
            _chip8.State.Key[9] = Convert.ToByte(state.IsKeyDown(Keys.D));
            _chip8.State.Key[0xA] = Convert.ToByte(state.IsKeyDown(Keys.Z));
            _chip8.State.Key[0xB] = Convert.ToByte(state.IsKeyDown(Keys.C));
            _chip8.State.Key[0xC] = Convert.ToByte(state.IsKeyDown(Keys.D4));
            _chip8.State.Key[0xD] = Convert.ToByte(state.IsKeyDown(Keys.R));
            _chip8.State.Key[0xE] = Convert.ToByte(state.IsKeyDown(Keys.F));
            _chip8.State.Key[0xF] = Convert.ToByte(state.IsKeyDown(Keys.V));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            int pixelWidth = screenWidth / 64;
            int pixelHeight = screenHeight / 32;

            // LoadContent has been called at this point
            _spriteBatch!.Begin();

            GraphicsDevice.Clear(Color.Black);

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    int index = (y * 64) + x;
                    if (_chip8.State.Graphics[index] == 1 || _displayBuffer[index] > 0)
                    {
                        _spriteBatch.Draw(_whiteRectangle, new Rectangle(x * pixelWidth, y * pixelHeight, pixelWidth, pixelHeight), Color.White);
                        _displayBuffer[index] |= (byte)((_chip8.State.Graphics[index] << 7) & 0x80);   //If _chip8.Graphics[index] == 1, set the first bit in displayBuffer[index] to 1
                    }
                    _displayBuffer[index] = (byte)((_displayBuffer[index] >> 1) & _bufferedFrameMask);  //Shift buffered frames right, apply mask to cap amount of buffered frames
                }
            }

            _spriteBatch.End();

            if (_chip8.State.SoundFlag)
            {
                // play sound
            }

            base.Draw(gameTime);
        }

        protected void RunCycles()
        {
            int millesecondsPerCycle = 1000 / ClockSpeed;

            Stopwatch s = new Stopwatch();
            s.Start();

            while (!_threadStopped)
            {
                _chip8.EmulateCycle();

                while (s.ElapsedMilliseconds < millesecondsPerCycle)
                {
                    Thread.Sleep(1);
                }

                s.Restart();
            }
        }
    }
}
