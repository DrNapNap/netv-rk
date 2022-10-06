using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSystem
{
    public class Pad : GameObject
    {
        private bool isLocal = false;
        NetworkHandler networkHandler;
        public Pad(string contentPath, ContentManager Content, Vector2 startPos, bool islocal = false, NetworkHandler networkHandler = null) : base(contentPath, Content)
        {
            this.position = startPos;
            this.isLocal = islocal;
            this.networkHandler = networkHandler;
        }
        public override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            if (isLocal)
            {
                if (kstate.IsKeyDown(Keys.Up))
                {
                    networkHandler.SendMessageToServer(new PlayerMovemenUpdate() { direction = Direction.up }, MessageType.movement);
                }
                if (kstate.IsKeyDown(Keys.Down))
                {

                    networkHandler.SendMessageToServer(new PlayerMovemenUpdate() { direction = Direction.down }, MessageType.movement);
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            base.Draw(gameTime, sb);
        }
    }
}
