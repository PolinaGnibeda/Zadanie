using HelpDesk.Core.Services;
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
    /// Логика взаимодействия для Status.xaml
    /// </summary>
    public partial class Status : Window
    {
        private readonly DBContext _context;
        private readonly StatusTransitionValidator _validator;
        private readonly Ticket _ticket;

        public Status(Ticket ticket)
        {
            InitializeComponent();
            _context = (DBContext)App.ServiceProvider.GetService(typeof(DBContext));
            _validator = (StatusTransitionValidator)App.ServiceProvider.GetService(typeof(StatusTransitionValidator));
            _ticket = ticket;

            // Заполняем информацию о заявке
            TicketInfo.Text = $"#{ticket.Id} - {ticket.Title}";
            CurrentStatus.Text = ticket.Status;

            // Загружаем все возможные статусы в ComboBox
            NewStatusBox.ItemsSource = Enum.GetValues(typeof(HelpDesk.Core.Models.Status));
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (NewStatusBox.SelectedItem == null)
            {
                ErrorText.Text = "Выберите новый статус.";
                return;
            }

            var newCoreStatus = (HelpDesk.Core.Models.Status)NewStatusBox.SelectedItem;
            var currentCoreStatus = Enum.Parse<HelpDesk.Core.Models.Status>(_ticket.Status);

            // Проверяем по матрице переходов (из Core)
            if (!_validator.IsTransitionAllowed(currentCoreStatus, newCoreStatus))
            {
                ErrorText.Text = $"Недопустимый переход из {currentCoreStatus} в {newCoreStatus}!";
                return;
            }

            try
            {
                // Меняем статус в БД
                _ticket.Status = newCoreStatus.ToString();
                _context.SaveChanges();

                MessageBox.Show("Статус успешно изменен!", "Успех");
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
