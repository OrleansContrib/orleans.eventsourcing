using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.EventSourcing
{
    public static class StateTransformer
    {
        public static void ApplyEvent(dynamic @event, IAggregateState grainState)
        {
            dynamic state = grainState;
            @event.Apply(state);

            grainState.Version++;
        }
    }
}
