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
using Orleans;
using Test.Interfaces;

namespace Test.Client
{
    /// <summary>
    /// Orleans test silo host
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // The Orleans environment is initialized in its own app domain in order to more
            // closely emulate the distributed situation, when the client and the server cannot
            // pass data via shared memory.
            AppDomain hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = args,
            });

            Orleans.OrleansClient.Initialize("DevTestClientConfiguration.xml");

            var johnGrain = PersonFactory.GetGrain(1);

            // If the name is set, we've run this code before.
            if (johnGrain.GetFirstName().Result == null)
            {
                johnGrain.Register(new PersonalAttributes { FirstName = "John", LastName = "Doe", Gender = GenderType.Male }).Wait();
                Console.WriteLine("We just wrote something to the persistent store (Id: {0}). Please verify!", johnGrain.GetPrimaryKey());
            }
            else
            {
                Console.WriteLine("\n\nThis was found in the persistent store: {0}, {1}, {2}\n\n",
                    johnGrain.GetFirstName().Result,
                    johnGrain.GetLastName().Result,
                    johnGrain.GetGender().Result.ToString());
            }

            var aliceGrain = PersonFactory.GetGrain(2);

            // If the name is set, we've run this code before.
            if (aliceGrain.GetFirstName().Result == null)
            {
                aliceGrain.Register(new PersonalAttributes { FirstName = "Alice", LastName = "Williams", Gender = GenderType.Female }).Wait();
                Console.WriteLine("We just wrote something to the persistent store (Id: {0}). Please verify!", aliceGrain.GetPrimaryKey());
            }
            else
            {
                Console.WriteLine("\n\nThis was found in the persistent store: {0}, {1}, {2}\n\n",
                    aliceGrain.GetFirstName().Result,
                    aliceGrain.GetLastName().Result,
                    aliceGrain.GetGender().Result.ToString());
            }

            aliceGrain.Marry(johnGrain).Wait();


            Console.WriteLine("Alice " + aliceGrain.GetLastName().Result);


            var bobGrain = PersonFactory.GetGrain(3);
            bobGrain.Register(new PersonalAttributes { FirstName = "Bob", LastName = "Hoskins", Gender = GenderType.Male }).Wait();

            aliceGrain.Marry(bobGrain).Wait();

            Console.WriteLine("Alice " + aliceGrain.GetLastName().Result);

            Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
            Console.ReadLine();

            hostDomain.DoCallBack(ShutdownSilo);
        }

        static void InitSilo(string[] args)
        {
            hostWrapper = new OrleansHostWrapper(args);

            if (!hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        static void ShutdownSilo()
        {
            if (hostWrapper != null)
            {
                hostWrapper.Dispose();
                GC.SuppressFinalize(hostWrapper);
            }
        }

        private static OrleansHostWrapper hostWrapper;
    }
}
