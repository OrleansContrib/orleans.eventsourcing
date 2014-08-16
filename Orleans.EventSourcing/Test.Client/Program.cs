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

            var maleGrain = PersonFactory.GetGrain(1);

            // If the name is set, we've run this code before.
            if (maleGrain.FirstName.Result == null)
            {
                maleGrain.Register(new PersonalAttributes { FirstName = "John", LastName = "Doe", Gender = GenderType.Male }).Wait();
                Console.WriteLine("We just wrote something to the persistent store (Id: {0}). Please verify!", maleGrain.GetPrimaryKey());
            }
            else
            {
                Console.WriteLine("\n\nThis was found in the persistent store: {0}, {1}, {2}\n\n",
                    maleGrain.FirstName.Result,
                    maleGrain.LastName.Result,
                    maleGrain.Gender.Result.ToString());
            }

            var femaleGrain = PersonFactory.GetGrain(2);

            // If the name is set, we've run this code before.
            if (femaleGrain.FirstName.Result == null)
            {
                femaleGrain.Register(new PersonalAttributes { FirstName = "Alice", LastName = "Williams", Gender = GenderType.Female }).Wait();
                Console.WriteLine("We just wrote something to the persistent store (Id: {0}). Please verify!", femaleGrain.GetPrimaryKey());
            }
            else
            {
                Console.WriteLine("\n\nThis was found in the persistent store: {0}, {1}, {2}\n\n",
                    femaleGrain.FirstName.Result,
                    femaleGrain.LastName.Result,
                    femaleGrain.Gender.Result.ToString());
            }

            femaleGrain.Marry(maleGrain.GetPrimaryKey(), maleGrain.LastName.Result);

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
