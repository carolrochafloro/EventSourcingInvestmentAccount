﻿using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;
public interface IQueue
{
    Task Produce(BaseEvent accountEvent);
    Task<BaseEvent> Consume();
}
