using HelpDesk.Core.Interfaces;
using HelpDesk.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using HelpDesk.Data.Models;
using HelpDesk.Data.Repositories; 
using Microsoft.EntityFrameworkCore;

namespace ЗаданиеЮшкова
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // 1. Читаем appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // 2. Настраиваем сервисы (DI)
            var serviceCollection = new ServiceCollection();

            // Подключаем базу данных
            serviceCollection.AddDbContext<DBContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // Регистрируем репозитории и сервисы
            serviceCollection.AddTransient<ITicketRepository, TicketRepository>();
            serviceCollection.AddTransient<IEmployeeRepository, EmployeeRepository>();
            serviceCollection.AddTransient<SlaCalculator>();
            serviceCollection.AddTransient<StatusTransitionValidator>();
            serviceCollection.AddTransient<TicketService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // ВАЖНО: Чтобы окно открывалось после настройки БД, удалите StartupUri из App.xaml!
            base.OnStartup(e);
        }
    }
}

