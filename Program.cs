using System;
using Elasticsearch.Net;
using Nest;

namespace ElasticsearchCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKey key;
            do
            {
                key = Menu();
            } while (key != ConsoleKey.Enter);
        }

        static ConsoleKey Menu()
        {
            Console.WriteLine("Selecione uma opção:");
            Console.WriteLine("1 - Importar dados");
            Console.WriteLine("2 - Pesquisar");
            Console.WriteLine("Enter - Sair");

            var key = Console.ReadKey();

            var elasticsearchRepository = new ElasticsearchRepository();
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    elasticsearchRepository.Create();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    elasticsearchRepository.GetLocation();
                    break;
            }
            return key.Key;
        }
    }

    public class ElasticsearchRepository
    {
        public ElasticClient Connection()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            StaticConnectionPool connectionPool;

            //Multiple node for fail over (cluster addresses)
            var nodes = new Uri[]
            {
                new Uri("http://localhost:9200"),
                //new Uri("Add server 2 address")   //Add cluster addresses here
                //new Uri("Add server 3 address")
            };

            connectionPool = new StaticConnectionPool(nodes);
            connectionSettings = new ConnectionSettings(connectionPool);
            connectionSettings.DefaultIndex("geocoding");

            elasticClient = new ElasticClient(connectionSettings);

            return elasticClient;
        }

        public void Create()
        {
            //Example
            var address = new Address()
            {
                Id = Guid.NewGuid().ToString(),
                Location = "Varginha - MG",
                Latitude = -21.5595272,
                Longitude = -45.4339099
            };

            //Create bulk insert

            var response = Connection().IndexDocument<Address>(address);

            if (response.IsValid)
            {
                Console.WriteLine("Salvo com sucesso!");
            }
        }

        public void GetLocation()
        {
            Console.WriteLine($"\nDigite o local:");
            var location = Console.ReadLine();          

            var response = Connection().Search<Address>(x => x                            
                .Size(5)
                .Query(q => q
                .Match(m => m.Field(x => x.Location).Query(location)
            )));
            
            if (response.Documents != null){

                Console.WriteLine("-----");
                foreach (var document in response.Documents)
                {
                    Console.WriteLine($"{ document.Id} { document.Location } { document.Latitude.ToString()} { document.Longitude.ToString()}");
                }
                Console.WriteLine("-----");
            }

        }
    }

    public class Address
    {
        public string Id { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }


}
