﻿using System;

namespace RabbitMqChat.Models
{
    public class MemberLeave
    {
        public string Name { get; set; }

        public string GroupName { get; set; }

        public DateTime Time { get; set; }
    }
}
