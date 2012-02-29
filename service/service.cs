/* 
 * Sources: 
 * http://msdn.microsoft.com/en-us/library/ms751519.aspx
 * http://msdn.microsoft.com/en-us/library/ms750530.aspx
 * 
 * TODO: 
 * Compile into a .exe for easy running (run exe as admin)
 * 
 * Recuring TODO: 
 * run service and generate proxy in VS console for client to use
 * svcutil.exe /language:cs /config:app.config http://localhost:8000/ServiceModelSamples/service
 * (source: http://msdn.microsoft.com/en-us/library/ms733133.aspx)
 */

using System;
using System.Configuration;
using System.ServiceModel;
using YouMote;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples")]
    public interface IChannelService
    {
        [OperationContract]
        Channel[] GetChannels();
        [OperationContract]
        Boolean UpdatePerson(int personID, int mediaID, double currentTime, bool isPlaying);
    }

    // Service class which implements the service contract.
    // Added code to write output to the console window
    public class ChannelService : IChannelService
    {
        public Channel[] GetChannels()
        {
            // make Channel class CLS compatible somehow(http://msdn.microsoft.com/en-us/library/system.clscompliantattribute)
            // then generate the proxy with svcutil again and test
            Console.WriteLine("GetChannels called!") ;
            return null;
        }

        public bool UpdatePerson(int personID, int mediaID, double currentTime, bool isPlaying)
        {
            Console.WriteLine("UpdatePerson({0},{1},{2},{3})", personID, mediaID, currentTime, isPlaying);
            return true;
        }

        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(ChannelService)))
            {


                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

            }
        }

    }

}

