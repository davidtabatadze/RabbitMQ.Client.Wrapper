﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Client.Wrapper.Test
{
    class ModelResultPublisher : RabbitPublisher<Model>
    {
        public ModelResultPublisher(RabbitPublisherConfiguration configuration)
            : base(configuration) { }


    }
}
