﻿namespace Shared
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string IntendedFor { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }
        public int RepliesNumber { get; set; }


    }
}