using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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


    public MainWindow()
    {
        InitializeComponent();
		_calendarData = new CalendarData();
		_calendarData.LoadData("calendarData.json"); // Загружаем данные

		// Устанавливаем текущий год и месяц на сегодня
		currentYear = DateTime.Now.Year;
        currentMonth = DateTime.Now.Month;
        currentYear = DateTime.Now.Year;
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
        int startDay = (int)firstDay.DayOfWeek;

        // создаем 6 строк
        for (int i = 0; i < 6; i++)
            CalendarGrid.RowDefinitions.Add(new RowDefinition());

        // создаем 7 колонок
        for (int i = 0; i < 7; i++)
            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition());

        int row = 0, col = startDay;

        for(int day = 1; day <= daysInMonth; day++)
        {
            Button dayButton = new Button
            {
                Content = day.ToString(),
                Width = 50,
                Height = 50,
                Background = Brushes.AliceBlue,
                Margin = new Thickness(5)
            };

            Grid.SetRow(dayButton, row);
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