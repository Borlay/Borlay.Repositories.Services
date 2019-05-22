using Borlay.Arrays;
using Borlay.Protocol;
using Borlay.Repositories.RocksDb;
using Borlay.Serialization.Notations;
using RocksDbSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting started...");

            //LaunchServerTest();
            //SaveAndEntityOrderAscGet();

            LaunchServer();
            TryLaunchClient();

            Console.ReadLine();
            Console.ReadLine();
        }

        private static async void LaunchServer()
        {
            var host = new ProtocolHost();
            host.LoadFromReference<Program>();
           
            var dbOptions = new DbOptions();
            dbOptions.SetCreateIfMissing();
            var rocksDb = RocksDbSharp.RocksDb.Open(dbOptions, @"C:\rocks\primaryservicetest\");
            host.Resolver.Register(rocksDb, false);

            var primaryRepository = new RocksPrimaryRepository(rocksDb, nameof(TestEntity));
            primaryRepository.WriteOptions.SetSync(false);
            host.Resolver.Register(primaryRepository);

            var indexMaps = new IndexMapProvider<TestEntity>();
            host.Resolver.Register(indexMaps);

            indexMaps.AddScoreIndex(e => e.Score, IndexLevel.Entity, OrderType.Asc | OrderType.Desc);

            host.RegisterHandler<PrimaryRepositoryService<TestEntity>>(true);

            var serverTask = host.StartServerAsync("127.0.0.1", 8080, CancellationToken.None);

            Console.WriteLine("Service Started");
        }

        public static async void TryLaunchClient()
        {
            try
            {
                await LaunchClient();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task LaunchClient()
        {
            var host = new ProtocolHost();
            host.LoadFromReference<Program>();

            using (var session = await host.StartClientAsync("127.0.0.1", 8080, CancellationToken.None))
            {
                Console.WriteLine("Client connected");

                var repository = session.CreateChannel<IPrimaryRepositoryService<TestEntity>>();

                var entityId = Enumerable.Range(0, 10000).Select(i => ByteArray.New(32)).ToArray();

                List<Task> tasks = new List<Task>();

                var rand = new Random();

                var watch = Stopwatch.StartNew();
                //for (int i = 0; i < entityId.Length; i++)
                //{
                //    var score = rand.Next();

                //    var task = repository.SaveAsync(new TestEntity()
                //    {
                //        Id = entityId[i],
                //        Name= "card 1",
                //        Value = 1,
                //        Score = i,
                //    });
                //    tasks.Add(task);

                //    //var card = await repository.Get(entityId[i]);
                //}

                //await Task.WhenAll(tasks);

                for (int i = 0; i < 3; i++)
                {
                    await repository.SaveAsync(new TestEntity()
                    {
                        Id = ByteArray.New(32),
                        Name = "card 1",
                        Value = 1,
                        Score = i,
                    });
                }

                var firstTen = await repository.GetAsync(OrderType.Asc, false, 0, 10);

                watch.Stop();
                // save async: 10k 0.82s

                Console.WriteLine($"Done in {watch.Elapsed}");
                Console.WriteLine($"{watch.Elapsed.TotalMilliseconds} ms.");
            }
        }
    }

    [Data(10)]
    public class TestEntity : IEntity
    {
        [Include(0)]
        public ByteArray Id { get; set; }

        [Include(1)]
        public string Name { get; set; }

        [Include(2)]
        public int Value { get; set; }

        [Include(3)]
        public long Score { get; set; }
    }
}
