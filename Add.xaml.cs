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
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Window
    {
        private readonly DBContext _context;
        private readonly Ticket _ticketToEdit;

        public Add()
        {
            InitializeComponent();
            _context = (DBContext)App.ServiceProvider.GetService(typeof(DBContext));
            LoadComboBoxes();
        }

        public Add(Ticket ticket) : this()
        {
            _ticketToEdit = ticket;

            // Заполняем поля при редактировании
            TitleBox.Text = ticket.Title;
            DescriptionBox.Text = ticket.Description;

            // Выбираем нужный приоритет
            foreach (ComboBoxItem item in PriorityBox.Items)
            {
                if (item.Content.ToString() == ticket.Priority)
                {
                    PriorityBox.SelectedItem = item;
                    break;
                }
            }

            // Выбираем нужного автора и оборудование
            AuthorBox.SelectedValue = ticket.AuthorId;
            EquipmentBox.SelectedValue = ticket.EquipmentId;
            AssigneeBox.SelectedValue = ticket.AssigneeId;
        }

        private void LoadComboBoxes()
        {
            // Загружаем авторов (сотрудников) из БД
            AuthorBox.ItemsSource = _context.Employees.Where(e => !e.IsDeleted).ToList();
            AuthorBox.DisplayMemberPath = "FullName"; // Отображаем ФИО
            AuthorBox.SelectedValuePath = "Id";       // Значение - это ID

            // Загружаем исполнителей
            AssigneeBox.ItemsSource = _context.Employees.Where(e => !e.IsDeleted).ToList();
            AssigneeBox.DisplayMemberPath = "FullName";
            AssigneeBox.SelectedValuePath = "Id";

            // Загружаем оборудование
            EquipmentBox.ItemsSource = _context.Equipment.Where(eq => !eq.IsDeleted).ToList();
            EquipmentBox.DisplayMemberPath = "Name";
            EquipmentBox.SelectedValuePath = "Id";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleBox.Text))
                {
                    MessageBox.Show("Заголовок не может быть пустым!", "Ошибка");
                    return;
                }

                if (AuthorBox.SelectedValue == null || EquipmentBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите автора и оборудование!", "Ошибка");
                    return;
                }

                if (_ticketToEdit == null) // Создание новой
                {
                    var newTicket = new Ticket
                    {
                        Title = TitleBox.Text,
                        Description = DescriptionBox.Text,
                        Priority = ((ComboBoxItem)PriorityBox.SelectedItem)?.Content.ToString() ?? "Normal",
                        Status = "New",
                        CreatedAt = DateTime.UtcNow,
                        AuthorId = (int)AuthorBox.SelectedValue,
                        EquipmentId = (int)EquipmentBox.SelectedValue,
                        AssigneeId = AssigneeBox.SelectedValue as int? // Может быть null
                    };
                    _context.Tickets.Add(newTicket);
                }
                else // Редактирование
                {
                    _ticketToEdit.Title = TitleBox.Text;
                    _ticketToEdit.Description = DescriptionBox.Text;
                    _ticketToEdit.Priority = ((ComboBoxItem)PriorityBox.SelectedItem)?.Content.ToString() ?? "Normal";
                    _ticketToEdit.AuthorId = (int)AuthorBox.SelectedValue;
                    _ticketToEdit.EquipmentId = (int)EquipmentBox.SelectedValue;
                    _ticketToEdit.AssigneeId = AssigneeBox.SelectedValue as int?;
                }

                _context.SaveChanges();
                MessageBox.Show("Успешно сохранено!", "Успех");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}", "Ошибка");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
