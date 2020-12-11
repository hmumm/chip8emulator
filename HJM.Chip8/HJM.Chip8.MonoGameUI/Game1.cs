using System;
using System.Diagnostics;
using System.Threading;
using HJM.Chip8.MonoGameUI.Sound;
using HJM.Chip8.MonoGameUI.Terminal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;

namespace HJM.Chip8.MonoGameUI
{
    public class Game1 : Game
    {
        private const int ClockSpeed = 500;

        private readonly GraphicsDeviceManager graphics;
        private readonly Thread emulatorThread;
        private readonly CPU.Chip8 chip8;
        private readonly byte[] displayBuffer = new byte[4096];
        private readonly byte bufferedFrameMask = 0b_1111_0000;
        private readonly SoundInstance soundInstance;

        private SpriteBatch? spriteBatch;
        private Texture2D? whiteRectangle;
        private bool threadStopped = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            emulatorThread = new Thread(RunCycles);
            chip8 = new CPU.Chip8();
            soundInstance = new SoundInstance();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            chip8.Initalize();
            chip8.LoadGame(GameSelectWindow.GetSelectedFilename());

            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            emulatorThread.Start();
            Log.Information("Emulator thread started.");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            whiteRectangle = new Texture2D(GraphicsDevice, width: 1, height: 1);
            whiteRectangle.SetData(new[] { Color.White });
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            if (whiteRectangle != null)
            {
                whiteRectangle.Dispose();
            }

            if (spriteBatch != null)
            {
                spriteBatch.Dispose();
            }

            threadStopped = true;
            Log.Information("Emulator thread stopped.");
        }

        protected override void Update(GameTime gameTime)
        {
            // Poll for current keyboard state
            KeyboardState state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // set all the keys
            chip8.State.Key[0] = Convert.ToByte(state.IsKeyDown(Keys.X));
            chip8.State.Key[1] = Convert.ToByte(state.IsKeyDown(Keys.D1));
            chip8.State.Key[2] = Convert.ToByte(state.IsKeyDown(Keys.D2));
            chip8.State.Key[3] = Convert.ToByte(state.IsKeyDown(Keys.D3));
            chip8.State.Key[4] = Convert.ToByte(state.IsKeyDown(Keys.Q));
            chip8.State.Key[5] = Convert.ToByte(state.IsKeyDown(Keys.W));
            chip8.State.Key[6] = Convert.ToByte(state.IsKeyDown(Keys.E));
            chip8.State.Key[7] = Convert.ToByte(state.IsKeyDown(Keys.A));
            chip8.State.Key[8] = Convert.ToByte(state.IsKeyDown(Keys.S));
            chip8.State.Key[9] = Convert.ToByte(state.IsKeyDown(Keys.D));
            chip8.State.Key[0xA] = Convert.ToByte(state.IsKeyDown(Keys.Z));
            chip8.State.Key[0xB] = Convert.ToByte(state.IsKeyDown(Keys.C));
            chip8.State.Key[0xC] = Convert.ToByte(state.IsKeyDown(Keys.D4));
            chip8.State.Key[0xD] = Convert.ToByte(state.IsKeyDown(Keys.R));
            chip8.State.Key[0xE] = Convert.ToByte(state.IsKeyDown(Keys.F));
            chip8.State.Key[0xF] = Convert.ToByte(state.IsKeyDown(Keys.V));

            soundInstance.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            int pixelWidth = screenWidth / 64;
            int pixelHeight = screenHeight / 32;

            // LoadContent has been called at this point
            spriteBatch!.Begin();

            GraphicsDevice.Clear(Color.Black);

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    int index = (y * 64) + x;
                    if (chip8.State.Graphics[index] == 1 || displayBuffer[index] > 0)
                    {
                        spriteBatch.Draw(whiteRectangle, new Rectangle(x * pixelWidth, y * pixelHeight, pixelWidth, pixelHeight), Color.White);
                        displayBuffer[index] |= (byte)((chip8.State.Graphics[index] << 7) & 0x80);   // If _chip8.Graphics[index] == 1, set the first bit in displayBuffer[index] to 1
                    }

                    displayBuffer[index] = (byte)((displayBuffer[index] >> 1) & bufferedFrameMask);  // Shift buffered frames right, apply mask to cap amount of buffered frames
                }
            }

            spriteBatch.End();

            if (chip8.State.SoundFlag)
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

            while (!threadStopped)
            {
                chip8.EmulateCycle();

                if (chip8.State.SoundFlag == true)
                {
                    soundInstance.PlaySound();
                }
                else
                {
                    soundInstance.StopPlayingSound();
                }

                while (s.ElapsedMilliseconds < millesecondsPerCycle)
                {
                    Thread.Sleep(1);
                }

                s.Restart();
            }
        }
    }
}
