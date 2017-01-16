
namespace Rewards.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
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
            SaveImages(form, Request.Files);

            string json = JsonConvert.SerializeObject(form, Formatting.Indented);
            var dataFile = Server.MapPath("~/data.json");
            System.IO.File.WriteAllText(dataFile, json);

            var data = ReadFile();

            var model = DeserialiseModel(data);
            return View(model);
        }

        private void SaveImages(HomeModel model, HttpFileCollectionBase files) {

            for (int i = 0; i < model.Participants.Count(); i++) {
                var file = files[i];
                if (file == null || file.ContentLength <= 0)
                    continue;

                file.SaveAs(Server.MapPath("~/") + file.FileName);
                model.Participants.ElementAt(i).Image = file.FileName;
            }
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

    public class ParticipantModel {
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
    }

    public class HomeModel {
        public IEnumerable<ParticipantModel> Participants { get; set; }
    }
}