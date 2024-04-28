using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Message : BaseClass
    {
        public string Content { get; set; }
        public IdentityUser CreatedBy { get; set; }
        public string IntendedFor { get; set; }
        public List<Message> Replies { get; set; }

    }
}
