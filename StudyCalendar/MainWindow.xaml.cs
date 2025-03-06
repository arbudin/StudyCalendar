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


    public MainWindow()
    {
        InitializeComponent();
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