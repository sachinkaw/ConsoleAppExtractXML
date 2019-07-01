using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    class CostCenter
    {
        private const double gstRate = 10.0 / 100;

        public CostCenter(double cost, string costCenter)
        {
            Cost = Math.Round(cost, 2);
            Center = costCenter;
        }

        public double Cost { get; set; }
        public string Center { get; set; }

        public double CostWithoutGst
        {
            get
            {
                return Math.Round(Cost - (Cost * gstRate), 2);
            }
        }

        public double Gst
        {
            get
            {
                return Math.Round(Cost * gstRate, 2);
            }
        }
    }
}
