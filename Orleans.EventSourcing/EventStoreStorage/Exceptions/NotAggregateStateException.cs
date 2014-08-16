using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.EventSourcing.EventStoreStorage.Exceptions
{
    [Serializable]
    public class NotAggregateStateException : Exception
    {
        public Type StateType { get; private set; }

        public NotAggregateStateException(Type stateType) 
        {
            this.StateType = stateType;
        }
        
        protected NotAggregateStateException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
