﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
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

        NetworkHandler _networkHandler;

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
            _networkHandler.SendMessageToServer(new JoinMessage() { playerName = "Nap", ResolutionX = graphics.PreferredBackBufferWidth, ResolutionY = graphics.PreferredBackBufferHeight }, MessageType.join);

            _networkHandler.SendToServer(new PlayerMovemenUpdate() { direction = Direction.up });


        }

        private void SetInitialPositionsMessage(SetInitialPositionsMessage initialPositionsMessage)
        {

            _networkHandler.AddListener<SnapShot>(HandleSnapShotMessage);
        }

        private void HandleSnapShotMessage(SnapShot e)
        {
            Debug.WriteLine("ball is updating!");

            List<float> playerY = new List<float>();




            foreach (var item in e.playerYPos)
            {
                if (e.playerYPos != null)
                {
                    playerY.Add(item);
                }
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

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            playerInput.handleOverallInput();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            playerInput.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}