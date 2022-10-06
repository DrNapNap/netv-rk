using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSystem
{
    public class Ball : GameObject
    {   
        public Ball(string contentPath, ContentManager Content, Vector2 startPos) : base(contentPath, Content)
        {
            this.position = startPos;
        }

        public override void Update(GameTime gameTime)
        {
          
            base.Update(gameTime);
        }
    }
}
