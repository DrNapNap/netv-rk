using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace ChatSystem
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        PlayerInput playerInput;

        private Vector2 screenSize;
        public Vector2 ScreenSize { get => screenSize; set => screenSize = value; }

        private KeyboardState previousState;
        private Direction test;

        List<GameObject> gamebjects = new List<GameObject>();

        List<Pad> pads = new List<Pad>();

        private SpriteFont text; //A single spritefront for the text (viewing score)
        private Texture2D map;    //It's for Texture2D background

        NetworkHandler _networkHandler;

        Ball ball;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            ScreenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _networkHandler = new NetworkHandler(new NetworkMessageBaseEventHandler());
            _networkHandler.AddListener<SetInitialPositionsMessage>(SetInitialPositionsMessage);
            _networkHandler.SendMessageToServer(new JoinMessage() { playerName = "kaj", ResolutionX = graphics.PreferredBackBufferWidth, ResolutionY = graphics.PreferredBackBufferHeight }, MessageType.join);
        }

        private void SetInitialPositionsMessage(SetInitialPositionsMessage initialPositionsMessage)
        {
            ball = new Ball("ball", Content, new Vector2(initialPositionsMessage.ballXpos, initialPositionsMessage.ballXpos));
            gamebjects.Add(ball);

            if (initialPositionsMessage.isLeftPlayer)
            {
                pads.Add(new Pad("pad", Content, new Vector2(initialPositionsMessage.leftPlayerXPos, initialPositionsMessage.leftPlayeryYPos), true, _networkHandler));
                pads.Add(new Pad("pad", Content, new Vector2(initialPositionsMessage.rightPlayeryXPos, initialPositionsMessage.rightPlayeryYPos)));

            }
            else
            {
                pads.Add(new Pad("pad", Content, new Vector2(initialPositionsMessage.leftPlayerXPos, initialPositionsMessage.leftPlayeryYPos)));
                pads.Add(new Pad("pad", Content, new Vector2(initialPositionsMessage.rightPlayeryXPos, initialPositionsMessage.rightPlayeryYPos), true, _networkHandler));

            }
            gamebjects.AddRange(pads);
            gamebjects.ForEach(x => x.Init());

            _networkHandler.AddListener<SnapShot>(HandleSnapShotMessage);
        }

        private void HandleSnapShotMessage(SnapShot e)
        {
            ball.SetPosition(new Vector2(e.ballXpos, e.ballYPos));
            for (int i = 0; i < e.playerYPos.Count; i++)
            {
                pads[i].SetPosition(new Vector2(pads[i].Position.X, e.playerYPos[i]));
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            playerInput = new PlayerInput(Content);

            playerInput.ReturnGetApi();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerInput.LoadContent();
            gamebjects.ForEach(x => x.LoadContent());


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            playerInput.handleOverallInput();



            gamebjects.ForEach(x => x.Update(gameTime));


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            playerInput.Draw(spriteBatch);
            gamebjects.ForEach(x => x.Draw(gameTime, spriteBatch));
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}