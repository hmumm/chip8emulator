using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading;

namespace HJM.Chip8.MonoGameUI
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D whiteRectangle;
        private CPU.Chip8 chip8;
        private int ClockSpeed = 500;
        private Thread emulatorThread;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            chip8 = new CPU.Chip8();
            chip8.Initalize();
            chip8.LoadGame(@"C:\Users\Hayden\Downloads\myChip8-bin-src\myChip8-bin-src\pong2.c8");

            _graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;

            emulatorThread = new Thread(RunCycles);
            emulatorThread.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            whiteRectangle.Dispose();
            _spriteBatch.Dispose();

            emulatorThread.Abort();
        }

        protected override void Update(GameTime gameTime)
        {
            // Poll for current keyboard state
            KeyboardState state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            // set all the keys
            chip8.Key[0] = Convert.ToByte(state.IsKeyDown(Keys.X));
            chip8.Key[1] = Convert.ToByte(state.IsKeyDown(Keys.D1));
            chip8.Key[2] = Convert.ToByte(state.IsKeyDown(Keys.D2));
            chip8.Key[3] = Convert.ToByte(state.IsKeyDown(Keys.D3));
            chip8.Key[4] = Convert.ToByte(state.IsKeyDown(Keys.Q));
            chip8.Key[5] = Convert.ToByte(state.IsKeyDown(Keys.W));
            chip8.Key[6] = Convert.ToByte(state.IsKeyDown(Keys.E));
            chip8.Key[7] = Convert.ToByte(state.IsKeyDown(Keys.A));
            chip8.Key[8] = Convert.ToByte(state.IsKeyDown(Keys.S));
            chip8.Key[9] = Convert.ToByte(state.IsKeyDown(Keys.D));
            chip8.Key[0xA] = Convert.ToByte(state.IsKeyDown(Keys.Z));
            chip8.Key[0xB] = Convert.ToByte(state.IsKeyDown(Keys.C));
            chip8.Key[0xC] = Convert.ToByte(state.IsKeyDown(Keys.D4));
            chip8.Key[0xD] = Convert.ToByte(state.IsKeyDown(Keys.R));
            chip8.Key[0xE] = Convert.ToByte(state.IsKeyDown(Keys.F));
            chip8.Key[0xF] = Convert.ToByte(state.IsKeyDown(Keys.V));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (chip8.DrawFlag)
            {
                int screenWidth = this.GraphicsDevice.Viewport.Width;
                int screenHeight = this.GraphicsDevice.Viewport.Height;

                int pixelWidth = screenWidth / 64;
                int pixelHeight = screenHeight / 32;

                _spriteBatch.Begin();

                GraphicsDevice.Clear(Color.Black);

                for (int y = 0; y < 32; y++)
                {
                    for(int x = 0; x < 64; x++)
                    {
                        if(chip8.Graphics[(y*64) + x] == 1)
                        {
                            _spriteBatch.Draw(whiteRectangle, new Rectangle(x * pixelWidth, y * pixelHeight, pixelWidth, pixelHeight), Color.White);
                        }
                    }
                }

                _spriteBatch.End();
                chip8.DrawFlag = false;
            }

            if(chip8.SoundFlag)
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

            while (true)
            {
                chip8.EmulateCycle();

                while(s.ElapsedMilliseconds < millesecondsPerCycle)
                {
                    Thread.Sleep(1);
                }

                s.Restart();
            }
        }
    }
}
