using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StudyCalendar.Models;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StudyCalendar;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private bool isAnimating = false;
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
		AnimateMonthTransition(false);
	}

	private void NextMonthButton_Click(object sender, RoutedEventArgs e)
	{
		AnimateMonthTransition(true);
	}

	// АНИМАЦИЯ
	private void AnimateMonthTransition(bool isNext)
	{
		if (isAnimating) return;
		isAnimating = true;

		double fromX = isNext ? 0 : 0; // Начальная позиция
		double toX = isNext ? -700 : 700; // Сдвиг влево (следующий) или вправо (предыдущий)

		// Создаём анимацию
		var slideAnimation = new DoubleAnimation
		{
			From = fromX,
			To = toX,
			Duration = TimeSpan.FromSeconds(0.4), // Длительность анимации
			EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut } // Смягчение движения
		};

		// Запуск анимации
		slideAnimation.Completed += (s, e) =>
		{
			// Меняем месяц ПОСЛЕ завершения анимации
			if (isNext)
			{
				currentMonth++;
				if (currentMonth > 12)
				{
					currentMonth = 1;
					currentYear++;
				}
			}
			else
			{
				currentMonth--;
				if (currentMonth < 1)
				{
					currentMonth = 12;
					currentYear--;
				}
			}

			UpdateCalendar(); // Обновляем календарь
			CalendarTransform.X = isNext ? 700 : -700; // Ставим новую начальную позицию

			// Анимация появления
			var slideInAnimation = new DoubleAnimation
			{
				From = isNext ? 700 : -700, // Начинает с противоположной стороны
				To = 0, // Возвращается в центр
				Duration = TimeSpan.FromSeconds(0.4),
				EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
			};

			// Запуск анимации прихода
			CalendarTransform.BeginAnimation(TranslateTransform.XProperty, slideInAnimation);

			isAnimating = false;
		};

		CalendarTransform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
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
				Content = isStudied ? $"{day}" : day.ToString(), 
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