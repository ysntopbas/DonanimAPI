using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonanimAPI.Models
{
    public class GpuInfo
    {
        public string Name { get; set; }
        public float? Value { get; set; }
        public bool? isTemp { get; set; }
    }
}
