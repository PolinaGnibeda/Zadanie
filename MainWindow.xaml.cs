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
using HelpDesk.Data.Models;

namespace ЗаданиеЮшкова
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DBContext _context;

        public MainWindow()
        {
            InitializeComponent();
            // Берем контекст базы данных из App.xaml.cs
            _context = (DBContext)App.ServiceProvider.GetService(typeof(DBContext));

            LoadData();
        }

        public void LoadData()
        {
            var tickets = _context.Tickets.ToList();

            if (tickets == null || tickets.Count == 0)
            {
                TicketsGrid.ItemsSource = null;
                return;
            }

            TicketsGrid.ItemsSource = tickets;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new Add();
            addWindow.ShowDialog(); // Открываем окно создания и ждем закрытия
            LoadData(); // Обновляем таблицу после создания
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли заявка в таблице
            if (TicketsGrid.SelectedItem is Ticket selectedTicket)
            {
                var statusWindow = new Status(selectedTicket);
                statusWindow.ShowDialog();
                LoadData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заявку в таблице!", "Внимание");
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            var query = _context.Tickets.AsQueryable();

            // Фильтр по статусу
            if (FilterStatus.SelectedIndex > 0)
            {
                string status = ((ComboBoxItem)FilterStatus.SelectedItem).Content.ToString();
                query = query.Where(t => t.Status == status);
            }

            // Фильтр по приоритету
            if (FilterPriority.SelectedIndex > 0)
            {
                string priority = ((ComboBoxItem)FilterPriority.SelectedItem).Content.ToString();
                query = query.Where(t => t.Priority == priority);
            }

            TicketsGrid.ItemsSource = query.OrderByDescending(t => t.CreatedAt).ToList();
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            var reportsWindow = new Reports();
            reportsWindow.ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (TicketsGrid.SelectedItem is HelpDesk.Data.Models.Ticket selectedTicket)
            {
                // Создаем окно редактирования, передавая туда выбранную заявку
                var editWindow = new Add(selectedTicket);
                editWindow.ShowDialog();

                // Обновляем таблицу после того, как окно закроется
                LoadData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, сначала выберите заявку в таблице!", "Внимание");
            }
        }
    }
}