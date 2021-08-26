using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderBook.BL.Models;
using OrderBook.WebapiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderBook.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void OrderEngineWorkerTest_PushOrdersFromCSVAndGetAllBooks_ValidSnapshotOfOneBook()
        {
            //testing the bl 
            var str = GetCSV();
            using WebClient webClient = new WebClient();
            //var str = webClient.DownloadString("https://raw.githubusercontent.com/DougieHauser/SolidusOrderbookTestData/master/solidus_orderbook_challenge_data.csv");
            var lines = str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Skip(1).SkipLast(1).ToList();
            var entries = lines.Select(ParseToEntry);
            var entries2 = new List<OrderBookEntry>(entries);
            var worker = new OrderEngineWorker();

            worker.PushOrders(entries);
            
            var books = worker.GetAllBooks().ToList();

            string jsonString = JsonSerializer.Serialize(books);

            var x = 5;
        }

        [TestMethod]
        public void OrderEngineWorkerTest_PushManyOrders_OrdersPushed()
        {
            //testing the bl 

            var str = GetCSV();
            using WebClient webClient = new WebClient();
            //var str = webClient.DownloadString("https://raw.githubusercontent.com/DougieHauser/SolidusOrderbookTestData/master/solidus_orderbook_challenge_data.csv");
            var lines = str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Skip(1).SkipLast(1).ToList();
            var entries = lines.Select(ParseToEntry);
            var entries2 = new List<OrderBookEntry>(entries);
            var entries3 = new List<OrderBookEntry>(entries);
            entries2.AddRange(entries3);
            var worker = new OrderEngineWorker();
            var onlyNew = entries.Where(e => e.OrderStatus.Equals(OrderStatus.New)).ToList();
            onlyNew.AddRange(entries2.Where(e => e.OrderStatus.Equals(OrderStatus.New)));

            foreach (var entry in entries2)
            {
                entry.Id = Guid.NewGuid().ToString();
                entry.TimeStamp += 2;
                entry.Price *= 1.0013;
            }

            Parallel.ForEach(entries, e => worker.PushOrders(new[] { e }));
            Parallel.ForEach(entries2, e => worker.PushOrders(new[] { e }));

            var books = worker.GetAllBooks().ToList();

            var x = 5;
        }

        [TestMethod]
        public async Task OrderController_PushOrdersFromCSVAndGetAllBooks_ValidSnapshotOfOneBook()
        {
            //testing the web api controller - need to run service first with `dotnet run`

            var str = GetCSV();
            using WebClient webClient = new WebClient();
            //var str = webClient.DownloadString("https://raw.githubusercontent.com/DougieHauser/SolidusOrderbookTestData/master/solidus_orderbook_challenge_data.csv");
            var lines = str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Skip(1).SkipLast(1);
            var entries = lines.Select(ParseToEntry);
            var first = entries.First();
            using var http = new HttpClient();
            await PostEntry(first, http);
            foreach(var entry in entries.Skip(1))
            {
                await PostEntry(entry, http);
            }

            var response = await http.GetAsync("https://localhost:5001/orderbook/");
            var responseString = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<OrderBookModel>>(responseString);
            var bookX = books.First();
            
            var response1 = await http.GetAsync($"https://localhost:5001/orderbook/fetch?exchange={first.Exchange}&symbol={first.Symbol}");
            var responseString1 = await response1.Content.ReadAsStringAsync();
        }

        private static async Task PostEntry(OrderBookEntry first, HttpClient http)
        {
            string jsonString = JsonSerializer.Serialize(first);
            var data = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonString));

            var response = await http.PostAsync("https://localhost:5001/orderbook/update", data);

            var responseString = await response.Content.ReadAsStringAsync();
        }

        private static OrderBookEntry ParseToEntry(string line)
        {
            var fields = line.Split(',');
            var timeStamp = long.Parse(fields[0]);
            return new OrderBookEntry()
            {
                TimeStamp = timeStamp,
                Exchange = fields[1],
                Symbol = fields[2],
                Side = Enum.Parse<OrderSide>(fields[3], ignoreCase: true),
                Price = double.Parse(fields[4]),
                Quantity = double.Parse(fields[5]),
                OrderStatus = Enum.Parse<OrderStatus>(fields[6], ignoreCase: true),
                OrderType = Enum.Parse<OrderType>(fields[7], ignoreCase: true),
            };
        }

        private static string GetCSV()
        {
            return "timestamp,exchange,symbol,side,price,quantity,status,type\r\n1500717600563,GMAX,ETH / USD,BUY,315.7,52.78,New,LIMIT\r\n1500717601063,GMAX,ETH / USD,BUY,315,100,New,LIMIT\r\n1500717601083,GMAX,ETH / USD,SELL,317.5,35.5,New,LIMIT\r\n1500717601993,GMAX,ETH / USD,BUY,315.7,52.78,Cancel,LIMIT\r\n1500717681952,GMAX,ETH / USD,SELL,317.2,42.3,New,LIMIT\r\n1500717682274,GMAX,ETH / USD,BUY,315.8,23.9,New,LIMIT\r\n1500717682342,GMAX,ETH / USD,BUY,314.75,31.1,New,LIMIT\r\n1500717682499,GMAX,ETH / USD,SELL,317.15,10,New,LIMIT\r\n1500717682608,GMAX,ETH / USD,SELL,315.8,10,New,LIMIT\r\n1500717682608,GMAX,ETH / USD,BUY,315.8,10,Execute,LIMIT\r\n1500717682608,GMAX,ETH / USD,SELL,315.8,10,Execute,LIMIT\r\n1500717682667,GMAX,ETH / USD,SELL,319,50,New,LIMIT\r\n1500717682777,GMAX,ETH / USD,BUY,313.95,15,New,LIMIT\r\n1500717682862,GMAX,ETH / USD,BUY,310,7.3,New,LIMIT\r\n1500717683188,GMAX,ETH / USD,BUY,300,3.9,New,LIMIT\r\n1500717683275,GMAX,ETH / USD,BUY,250,40,New,LIMIT\r\n1500717683397,GMAX,ETH / USD,BUY,7,22.8,New,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,7,500,New,LIMIT\r\n1500717683411,GMAX,ETH / USD,BUY,315.8,13.9,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,315.8,13.9,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,BUY,315,100,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,315,100,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,BUY,314.75,31.1,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,314.75,31.1,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,BUY,313.95,15,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,313.95,15,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,BUY,310,7.3,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,310,7.3,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,BUY,300,3.9,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,300,3.9,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,BUY,250,40,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,250,40,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,BUY,7,22.8,Execute,LIMIT\r\n1500717683411,GMAX,ETH / USD,SELL,7,22.8,Execute,LIMIT\r\n";
        }
    }
}
