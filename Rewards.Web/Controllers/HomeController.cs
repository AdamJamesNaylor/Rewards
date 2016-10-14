
namespace Rewards.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Newtonsoft.Json;

    public class HomeController : Controller
    {
        public ActionResult Index() {
            var data = ReadFile();

            var model = DeserialiseModel(data);

            return View(model);
        }

        public JsonResult Update(string date) {
            var dataFile = Server.MapPath("~/data.json");
            var timestamp = System.IO.File.GetLastWriteTime(dataFile);
            var lastcheck = DateTime.ParseExact(date, "yyyy/M/d H:m:s", CultureInfo.InvariantCulture);
            return Json(timestamp > lastcheck, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Admin() {
            var data = ReadFile();

            var model = DeserialiseModel(data);

            return View(model);
        }

        [HttpPost]
        public ActionResult Admin(HomeModel form) {
            string json = JsonConvert.SerializeObject(form, Formatting.Indented);
            var dataFile = Server.MapPath("~/data.json");
            System.IO.File.WriteAllText(dataFile, json);

            var data = ReadFile();

            var model = DeserialiseModel(data);
            return View(model);
        }

        private static HomeModel DeserialiseModel(string data)
        {
            var serialiser = new JavaScriptSerializer();
            var model = serialiser.Deserialize<HomeModel>(data);

            return model;
        }

        private string ReadFile()
        {
            string data;
            var dataFile = Server.MapPath("~/data.json");

            using (var stream = System.IO.File.OpenText(dataFile))
            {
                data = stream.ReadToEnd();
            }
            return data;
        }

    }

    public class KidModel {
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
    }

    public class HomeModel {
        public IEnumerable<KidModel> Kids { get; set; }
    }
}