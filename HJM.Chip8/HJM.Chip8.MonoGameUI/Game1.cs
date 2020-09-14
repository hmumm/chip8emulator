using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HJM.Chip8.MonoGameUI
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D whiteRectangle;
        private CPU.Chip8 chip8;

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
            chip8.LoadGame(@"");

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
        }

        protected override void Update(GameTime gameTime)
        {
            chip8.EmulateCycle();

            // chip8.SetKeys();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (chip8.DrawFlag)
            {
                GraphicsDevice.Clear(Color.Black);

                int screenWidth = this.GraphicsDevice.Viewport.Width;
                int screenHeight = this.GraphicsDevice.Viewport.Height;

                int pixelWidth = screenWidth / 64;
                int pixelHeight = screenHeight / 32;

                _spriteBatch.Begin();

                for(int y = 0; y < 32; y++)
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
    }
}
