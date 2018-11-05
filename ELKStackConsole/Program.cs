using log4net;
using Nest;
using System;
using System.Threading;

namespace ELKStackConsole
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            //Display actions available
            Console.WriteLine("1 - Generate Logs");
            Console.WriteLine("2 - View Logs");
            var option = Console.ReadLine();

            if (option == "1")
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    //Add a divide by zero exception to ElasticSearch
                    try
                    {
                        throw new DivideByZeroException();
                    }
                    catch (DivideByZeroException ex)
                    {
                        Console.WriteLine("Generating log...");
                        logger.Error("Attemped to divide by Zero.", ex);
                    }

                    //Add an invalid operation exception to ElasticSearch
                    try
                    {
                        throw new InvalidOperationException();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("Generating log...");
                        logger.Error("Invalid operation.", ex);
                    }

                    //Add a generic exception to ElasticSearch
                    try
                    {
                        throw new Exception("Generic exception");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Generating log...");
                        logger.Error("An exception was thrown.", ex);
                    }
                }
            }
            else
            {
                //Init ElasticSearch client
                var node = new Uri("http://localhost:9200");
                var settings = new ConnectionSettings(node);
                settings.DefaultIndex("log-2018.11.03");
                var client = new ElasticClient(settings);

                //Query logs
                var response = client.Search<Message>(s => s
                    .Type("logEvent")
                    .From(0)
                    .Size(5000)
                );

                //Display results
                var docsFound = response.Documents;
                if (docsFound != null)
                {
                    foreach (var docFound in docsFound)
                    {
                        Console.WriteLine($"{docFound} : {docFound.exception.Message}");
                    }
                }

                Console.Write($"Documents found: {response.Documents.Count}");
                Console.Read();
            }
        }
    }
}
