﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqChat.Contracts
{
    public class SampleRequest
    {
        public string Text { get; set; }
        public string User { get; set; }
    }
}
