using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StudyCalendar.Models;
using System.Linq;
using System.Windows.Media;

namespace StudyCalendar;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int currentYear;
    private int currentMonth;
    private CalendarData _calendarData;

	private void SaveData()
	{
		_calendarData.SaveData("calendarData.json");
	}

	private void DayButton_Click(object sender, RoutedEventArgs e)
	{
		Button clickedButton = (Button)sender;
		int day = (int)clickedButton.Tag;

		// Добавляем день в базу данных занятий
		_calendarData.MarkDayAsStudied(currentYear, currentMonth, day);

		// Перерисовываем календарь, чтобы обновить кнопку
		UpdateCalendar();
	}

    private void PrevMonthButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentMonth == 1)
        {
            currentMonth = 12;
            currentYear--;
        }
        else currentMonth--;

        UpdateCalendar();
    }

	private void NextMonthButton_Click(object sender, RoutedEventArgs e)
	{
        if (currentMonth == 1)
        {
            currentMonth = 12;
            currentYear++;
        }
        else currentMonth++;

		UpdateCalendar();
	}

	public MainWindow()
    {
		InitializeComponent();
		_calendarData = new CalendarData();
		_calendarData.LoadData("calendarData.json"); // Загружаем данные

		// Устанавливаем текущий год и месяц на сегодня
		currentYear = DateTime.Now.Year;
		currentMonth = DateTime.Now.Month;

        // Регистрация кнопок назад и вперед
        PrevMonthButton.Click += PrevMonthButton_Click;
		NextMonthButton.Click += NextMonthButton_Click;

		UpdateCalendar();
	}

	private void UpdateCalendar()
    {
        // очистка старого календаря
        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();
        CalendarGrid.ColumnDefinitions.Clear();

        // определяем начало недели
        int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        DateTime firstDay = new DateTime(currentYear, currentMonth, 1);
        int startDay = ((int)firstDay.DayOfWeek + 6) % 7; // здесь начинаем календарь с ПН

        CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

        // создаем 6 строк и 7 колонок
        for (int i = 0; i < 6; i++)
            CalendarGrid.RowDefinitions.Add(new RowDefinition());
        for (int i = 0; i < 7; i++)
            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition());

        // добавлям заголовок с днями недели
        string[] dayNames = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
        for (int coll = 0; coll < 7; coll++)
        {
            TextBlock dayNameText = new TextBlock
            {
                Text = dayNames[coll],
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Normal
            };
            Grid.SetRow(dayNameText, 0);
            Grid.SetColumn(dayNameText, coll);
            CalendarGrid.Children.Add(dayNameText);
        }

        int row = 0, col = startDay;

        // отрисовка дней
        for(int day = 1; day <= daysInMonth; day++)
        {
            bool isStudied = _calendarData.Activities.Any(a => a.Year == currentYear 
                                                            && a.Month == currentMonth 
                                                            && a.Day == day 
                                                            && a.IsStudied);

			Button dayButton = new Button
            {
				Content = isStudied ? $"{day}" : day.ToString(), // Добавляем галочку, если занимался
				Style = (Style)Application.Current.TryFindResource("RoundedButtonStyle"),
				Width = 50,
                Height = 50,
                Margin = new Thickness(3),
                Cursor = Cursors.Hand,
                Tag = day
            };

            if (isStudied)
                dayButton.Background = Brushes.LimeGreen;

            // обработка клика
            dayButton.Click += DayButton_Click;

            Grid.SetRow(dayButton, row+1);
            Grid.SetColumn(dayButton, col);
			CalendarGrid.Children.Add(dayButton);

            col++;
            if(col == 7)
            {
                col = 0;
                row++;
            }
        }

        MonthLabel.Text = $"{firstDay.ToString("MMMM yyyy")}";
    }
}