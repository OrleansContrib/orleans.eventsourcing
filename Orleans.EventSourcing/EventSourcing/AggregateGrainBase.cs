using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.EventSourcing
{
    public abstract class AggregateGrainBase<S> : GrainBase<S>
        where S : class, IAggregateState
    {
        protected Task RaiseEvent(dynamic @event, bool store = true)
        {
            this.State.UncommitedEvents.Add(@event);

            StateTransformer.ApplyEvent(@event, this.State);

            return store
                 ? this.State.WriteStateAsync()
                 : TaskDone.Done;
        }
    }
}
