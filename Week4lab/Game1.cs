using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public static class StaticValue
    {
        public const float BULLET_SPEED = 1f;
        //9-11
        public const int ENEMY_TANK_AMOUNT = 5;
        public const int FLEE_TANK_AMOUNT = 3;
        public const int TOTAL_NPC_TANK = ENEMY_TANK_AMOUNT + FLEE_TANK_AMOUNT;

    }

    public class Game1 : Game
    {
        //public property


        public GraphicsDevice device { get; protected set; }

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Camera camera { get; protected set; }
        public ModelManager modelManager;
        public Text text;
        public SoundEffect soundFX;
        public SoundEffect voiceFX;
        public SoundEffectInstance music;
        public SoundEffectInstance sound;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(this, new Vector3(0, 50, -100), new Vector3(0, 0, 0), Vector3.Up);
            Components.Add(camera);
            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            //Updated Code
            text = new Text(this);
            Components.Add(text);

            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;
            soundFX = Content.Load<SoundEffect>(@"Audio/track");
            music = soundFX.CreateInstance();
            music.IsLooped = true;
            music.Play();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

    }
}
