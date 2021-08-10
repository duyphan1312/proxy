using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using StrongProxyAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StrongProxyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticIPController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<StaticIP>> Get()
        {
            IEnumerable<StaticIP> staticIPList = ReadStaticIP();
            return Ok(staticIPList);
        }

        private List<StaticIP> ReadStaticIP()
        {
            List<StaticIP> staticIPList = null;
            string file = Constant.STATIC_IP_PATH;
            if (System.IO.File.Exists(file))
            {
                using (StreamReader reader = new StreamReader(file))
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    staticIPList = csv.GetRecords<StaticIP>().ToList();
                }
            }
            return staticIPList;
        }
    }
}
