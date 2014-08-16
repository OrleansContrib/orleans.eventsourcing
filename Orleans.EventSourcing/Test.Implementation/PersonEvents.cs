using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Test.Interfaces;

namespace Test.Implementation
{
    public class PersonRegistered
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }

        public void Apply(IPersonState state)
        {
            state.FirstName = this.FirstName;
            state.LastName = this.LastName;
            state.Gender = this.Gender;
        }
    }

    public class PersonMarried
    {
        public Guid SpouseId { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseLastName { get; set; }

        public void Apply(IPersonState state)
        {
            state.IsMarried = true;
        }
    }

    public class PersonLastNameChanged
    {
        public string LastName { get; set; }

        public void Apply(IPersonState state)
        {
            state.LastName = this.LastName;
        }
    }
}
