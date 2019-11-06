using RabbitMqChat.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqChat.Models
{
    public class MemberJoined
    {
        public string Name { get; set; }

        public string GroupName { get; set; }
    }
}
