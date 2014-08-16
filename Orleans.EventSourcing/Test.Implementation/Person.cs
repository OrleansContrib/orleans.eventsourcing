//*********************************************************
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Orleans;
using Test.Interfaces;
using Orleans.EventSourcing;

namespace Test.Implementation
{
    [StorageProvider(ProviderName = "EventStore")]
    public class Person : AggregateGrainBase<IPersonState>, Test.Interfaces.IPerson
    {
        Task IPerson.Register(PersonalAttributes props)
        {
            return this.RaiseEvent(new PersonRegistered
            {
                FirstName = props.FirstName,
                LastName = props.LastName,
                Gender = props.Gender
            });
        }

        Task IPerson.Marry(Guid spouseId, string newLastName)
        {
            return this.RaiseEvent(new PersonMarried
            {
                SpouseId = spouseId
            }, store: false) // We are not storing the first event here
                .ContinueWith(_ => this.RaiseEvent(new PersonLastNameChanged
            {
                LastName = newLastName
            })); // Both events will be persisted here
        }

        Task<string> IPerson.FirstName
        {
            get { return Task.FromResult(State.FirstName); }
        }

        Task<string> IPerson.LastName
        {
            get { return Task.FromResult(State.LastName); }
        }

        Task<GenderType> IPerson.Gender
        {
            get { return Task.FromResult(State.Gender); }
        }

        Task<bool> IPerson.IsMarried
        {
            get { return Task.FromResult(State.IsMarried); }
        }
    }

    public interface IPersonState : IAggregateState
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        GenderType Gender { get; set; }
        bool IsMarried { get; set; }
    }
}
