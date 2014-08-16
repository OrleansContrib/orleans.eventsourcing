﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.EventSourcing
{
    public interface IAggregateState : IGrainState
    {
        List<object> UncommitedEvents { get; set; }
        int Version { get; set; }
    }
}
