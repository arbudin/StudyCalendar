using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyCalendar.Models
{
    class DayActivity
    {
		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public bool IsStudied { get; set; }
	}
}
