using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace StudyCalendar.Models
{
	class CalendarData
	{
		public List<DayActivity> Activities { get; set; } = new List<DayActivity>();

		// загрузка информации
		public void LoadData(string filename)
		{
			if (!File.Exists(filename))
			{
				File.WriteAllText(filename, "[]");
			}

			string json = File.ReadAllText(filename);
			var data = JsonConvert.DeserializeObject<List<DayActivity>>(json);
			if (data != null)
			{
				Activities = data;
			}
		}

		// сохранение информации 
		public void SaveData(string filename)
		{
			string json = JsonConvert.SerializeObject(Activities, Formatting.Indented);
			File.WriteAllText(filename, json);
		}

		// отметка занимался сегодня или нет
		public void MarkDayAsStudied(int year, int month, int day)
		{
			var existingEntry = Activities.FirstOrDefault(a => a.Year == year && a.Month == month && a.Day == day);

			if (existingEntry == null)
			{
				Activities.Add(new DayActivity { Year = year, Month = month, Day = day, IsStudied = true });
			}
			else
			{
				existingEntry.IsStudied = !existingEntry.IsStudied;
			}

			SaveData("calendarData.json");
		}
	}

}
