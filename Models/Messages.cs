using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Messages
    {
        public Messages(string Id, string sender, string time, string content, string type)
        {
            this.Id = Id;
            this.sender = sender;
            this.time = time;
            this.content = content;
            this.type = type;
        }

        public string Id { get; set; }
        public string sender { get; set; }
        public string time { get; set; }
        public string content  { get; set; }
        public string type { get; set; }
    }
}
