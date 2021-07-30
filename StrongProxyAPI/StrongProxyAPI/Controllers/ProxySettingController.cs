using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrongProxyAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace StrongProxyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxySettingController : ControllerBase
    {
        [HttpGet]
        public ActionResult<ProxySetting> Get()
        {
            ProxySetting proxySetting = ReadSetting();
            return proxySetting;
        }

        private ProxySetting ReadSetting()
        {
            ProxySetting setting = null;
            string file = Constant.SETTING_PATH;
            if (System.IO.File.Exists(file))
            {
                using (StreamReader reader = new StreamReader(file))
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    IEnumerable<ProxySetting> records = csv.GetRecords<ProxySetting>();

                    setting = records.FirstOrDefault();
                }
            }
            return setting;
        }
    }
}
