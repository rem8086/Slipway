using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShipwayStatus;
using ShipwayStatus.Models;

namespace ShipwayStatus.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ViewResult Index()
        {
            string[] PHASE = new string[] {
                    "Вертикальная планировка участка плита",
                    "Устройство щебеночного основания плита",
                    "Устройство бетонной подготовки плита",
                    "Армирование с установкой закладных деталей и анкерных болтов плиты",
                    //"Установка опалубки плиты",
                    "Бетонирование плиты",
                    "Бетонирование покрытия плиты",
                    "Устройство пунктов подключения",
                    "Устройство футляров электроснабжения"
                    };

            Dictionary<string, string> phases = new Dictionary<string, string>
            {
                {"wells", "Устройство пунктов подключения"},
                {"pipes", "Устройство футляров электроснабжения"},
                {"planing", "Вертикальная планировка участка плита"},
                {"stone_base",  "Устройство щебеночного основания плита"},
                {"concrete_prepare",  "Устройство бетонной подготовки плита"},
                {"reinforcement",  "Армирование с установкой закладных деталей и анкерных болтов плиты"},
                //"Установка опалубки плиты",
                {"concreting", "Бетонирование плиты"},
                {"covering", "Бетонирование покрытия плиты"}  
            };

            DataPull dp = new DataPull("sqltest", "PMDB_EPPM", "PMDB_EPPM_privuser", "privuser123");
            string date = dp.ExecuteRecalcDate("EPC.3 ИН-0748.15");
            List<Pad> shipway = dp.ExecuteQuery(phases, "EPC.3 ИН-0748.15");
            Slipway s = new Slipway(shipway, date);
            return View(s);
        }

        public ViewResult TableSlipway()
        {
            Dictionary<string, string> phases = new Dictionary<string, string>
            {
                {"Вертикальная планировка", "Вертикальная планировка участка плита"},
                {"Щебеночное основание",  "Устройство щебеночного основания плита"},
                {"Бетонная подготовка",  "Устройство бетонной подготовки плита"},
                {"Армирование плиты",  "Армирование с установкой закладных деталей и анкерных болтов плиты"},
                {"Установка опалубки", "Установка опалубки плиты" },
                {"Бетонирование плиты", "Бетонирование плиты"},
                {"Бетонирование покрытия", "Бетонирование покрытия плиты"},
                {"Пункты подключения", "Устройство пунктов подключения"},
                {"Футляры электроснабжения", "Устройство футляров электроснабжения"}
            };

            DataPull dp = new DataPull("sqltest", "PMDB_EPPM", "PMDB_EPPM_privuser", "privuser123");
            string date = dp.ExecuteRecalcDate("EPC.3 ИН-0748.15");
            List<Pad> shipway = dp.ExecuteQuery(phases, "EPC.3 ИН-0748.15");
            Slipway s = new Slipway(shipway, date);
            return View(s);
        }
    }
}