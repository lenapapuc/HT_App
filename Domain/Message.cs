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
        public ApplicationUser? CreatedBy { get; set; }
        public string IntendedFor { get; set; }

        public Message? OriginalMessage { get; set; }


        public void AddReply(Message reply)
        {
            reply.OriginalMessage = this;

        }
    }


}
