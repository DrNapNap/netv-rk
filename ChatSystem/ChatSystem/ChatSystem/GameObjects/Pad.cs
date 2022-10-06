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
        public Direction test;

        public Pad(string contentPath, ContentManager Content, Vector2 startPos, bool islocal = false) : base(contentPath, Content)
        {
            this.position = startPos;
            this.isLocal = islocal;
        }
        public override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            if (isLocal)
            {
                if (kstate.IsKeyDown(Keys.Up))
                {
                    position =  new Vector2(0,1);
                }
                if (kstate.IsKeyDown(Keys.Down))
                {

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
