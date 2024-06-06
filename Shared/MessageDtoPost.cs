using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class MessageDtoPost
    {
        public string Content { get; set; }
        public string IntendedFor { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }

   

    }
}
