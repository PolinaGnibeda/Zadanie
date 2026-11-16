using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HelpDesk.Data.Models;

namespace ЗаданиеЮшкова
{
    /// <summary>
    /// Логика взаимодействия для Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        private readonly DBContext _context;

        public Reports()
        {
            InitializeComponent();
            _context = (DBContext)App.ServiceProvider.GetService(typeof(DBContext));
            LoadReports();
        }

        private void LoadReports()
        {
            // 1. Количество заявок по статусам
            var statusCounts = _context.Tickets
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();
            StatusCountGrid.ItemsSource = statusCounts;

            // 2. Среднее время решения (упрощенно считаем количество)
            var total = _context.Tickets.Count();
            AvgTimeText.Text = $"Всего заявок в базе: {total}";
        }
    }
}

