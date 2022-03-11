using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Post
    {
        public Post(string id, string media, string desc, string poster)
        {
            this.id = id;
            this.media = media;
            this.desc = desc;
            this.poster = poster;
        }

        public string id { get; set; }
        public string media { get; set; }
        public string desc { get; set; }
        public string poster { get; set; }
    }
}
