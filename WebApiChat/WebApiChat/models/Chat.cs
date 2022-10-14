using Microsoft.AspNetCore.Mvc;

namespace WebApiChat.models
{
    public class Chat
    {
        public int id { get; set;}

        public string? name {get; set;}   

        public string? text { get; set;} 

    }
}

