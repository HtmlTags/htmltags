using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HtmlTags.AspNetCore.TestSite.Controllers
{
    public enum Blarg
    {
        Foo = 1,
        Bar = 2
    }
    public class HomeIndexModel
    {
        public HomeIndexModel()
        {
            Blorgs = new[] { new BlargModel { Blorg = "blorg"} };
        }
        public class BlargModel
        {
            public string Blorg { get; set; }
        }
        public string Value { get; set; }
        public Blarg Blarg { get; set; }

        public BlargModel Blorg { get; set; }
        public BlargModel[] Blorgs { get; set; }
    }

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new HomeIndexModel {Value = "asdfa"};
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
