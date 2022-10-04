using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ChatSystem
{
    internal class PlayerInput
    {
        KeyboardState oldState;
        ContentManager content;
        public const int MAX_MESSAGES = 7;
        SpriteFont arial;
        string playerChatText = "";
        public string PlayerChatText { get { return playerChatText; } set { playerChatText = value; } }

        public List<string> messages = new List<string>();

        string url = "https://localhost:7235/api/Chat";


        public PlayerInput(ContentManager content)
        {
            this.content = content;
        }

        protected async void PostApi(string text)
        {
            HttpClient client = new HttpClient();

            try
            {            
                int idPost = 1;
                idPost++;

                var chat = new Chat() { name = "test", id = idPost, text = text };
                var data = new StringContent(JsonConvert.SerializeObject(chat), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, data);   

            }
            catch (Exception)
            {
                
                Debug.Print("err");
            }


        }


        public void handleOverallInput() //get updated 16.6 times every 1 second
        {
            KeyboardState key = new KeyboardState();
            Keys[] multipleKeys;
            key = Keyboard.GetState();
            multipleKeys = key.GetPressedKeys();
            handleChatBoxInput(multipleKeys);
        }

        public void handleChatBoxInput(Keys[] keys)
        {
            KeyboardState newState = Keyboard.GetState();
            foreach (Keys key in keys)
            {
                if (newState.IsKeyDown(key))
                {
                    if (!oldState.IsKeyDown(key))
                    {
                        int keyValue = (int)key;
                        if ((keyValue >= 0x30 && keyValue <= 0x39) // numbers
                            || (keyValue >= 0x41 && keyValue <= 0x5A) // letters
                            || (keyValue >= 0x60 && keyValue <= 0x69)) //numpad
                        {
                            PlayerChatText += key;
                        }
                        switch (keyValue)
                        {
                            case 0x20:
                                PlayerChatText += " "; //we do this because otherwise we'll get like SPACE instead of " "
                                break;
                            case 0xBF:
                                PlayerChatText += ":";
                                break;
                            case 0xDB:
                                PlayerChatText += ")";
                                break;
                            case 8:
                                if (PlayerChatText.Length > 0)
                                    playerChatText = playerChatText.Substring(0, playerChatText.Length - 1);
                                break;
                            case 13:
                                if (!PlayerChatText.Equals(""))
                                    messages.Add(playerChatText.ToLower());
                                PostApi(PlayerChatText);
                                playerChatText = "";
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            oldState = newState;
        }

        public void LoadContent()
        {
            arial = content.Load<SpriteFont>("Arial");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(arial, PlayerChatText.ToLower(), new Vector2(1400, 80), Color.White);
            
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                if (i < PlayerInput.MAX_MESSAGES)
                {
                    int d = 250 - (i * 20);
                    spriteBatch.DrawString(arial, messages[i], new Vector2(1400, d), Color.White);   
                }
                else
                {
                    messages.RemoveAt(0);
                }
            }
        }
    }
}
