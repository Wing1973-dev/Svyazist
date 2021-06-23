using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Svyazist.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Svyazist.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config) {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index() {
            var items = GetResultRequest();
            return View(items);
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IDbConnection Connection {
            get {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        List<EntityRow> GetResultRequest() {
            using (IDbConnection db = Connection) {
                var result = db.Query<EntityRow>("with  NumbersOfSales as (select saleDate, count(*) as numbersOfSales, count(*) * cost as costsOfSales from Sales join Products on Sales.productId = Products.id where Products.id = 5 group by saleDate, cost) select ROW_NUMBER() over(order by NumbersOfSales.saleDate) as numbersInOrder, NumbersOfSales.saleDate, SUM(NumbersOfSales.numbersOfSales) over(order by NumbersOfSales.saleDate) as runNumbersOfSales, SUM(NumbersOfSales.costsOfSales) over(order by NumbersOfSales.saleDate) as runCostsOfSales from NumbersOfSales order by saleDate").ToList();
                return result;
            }
        }

        public class EntityRow {
            public int numbersInOrder { get; set; }
            public DateTime saleDate { get; set; }
            public int runNumbersOfSales { get; set; }
            public double runCostsOfSales { get; set; }
        }
        
    }
}
