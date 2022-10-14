using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiChat.models;

namespace WebApiChat
{
    [Route("api/[controller]")]
    [ApiController]

    public class ChatController : ControllerBase
    {
        private readonly ChatDb _chatDb;

        public ChatController(ChatDb chatDb)
        {
            this._chatDb = chatDb;

        }

        [HttpGet]

        public ActionResult<IEnumerable<Chat>> Get()
        {
            return Ok(_chatDb.Values);
        }

        // POST: ChatController/Edit/5
        [HttpGet("{text}")]

        public ActionResult<string> Get(string text)
        {
            if (_chatDb.TryGetValue(text, out Chat? chat))
            {
                return Ok(chat);
            }

            return NotFound();    
        }


        [HttpPost]
        public IActionResult Post([FromBody] Chat? chat)
        {
            if (chat == null)
            {
                return BadRequest();
            }
            if (_chatDb.ContainsKey(chat.text))
            {
                return Conflict();
            }

            _chatDb.Add(chat.text, chat);

            return Created(Request.Path + "/" +chat.text, null);
        }

    }
}
