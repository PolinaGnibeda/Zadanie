using HelpDesk.Core.Exceptions;
using HelpDesk.Core.Interfaces;
using HelpDesk.Core.Models;

namespace HelpDesk.Core.Services
{
    public class TicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly StatusTransitionValidator _transitionValidator;

        public TicketService(
            ITicketRepository ticketRepo,
            IEmployeeRepository employeeRepo,
            StatusTransitionValidator transitionValidator)
        {
            _ticketRepo = ticketRepo;
            _employeeRepo = employeeRepo;
            _transitionValidator = transitionValidator;
        }

        // 1. Создание заявки (с валидацией)
        public async Task<int> CreateTicketAsync(Ticket ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket.Title))
                throw new DomainException("Заголовок не может быть пустым.");

            if (ticket.Title.Length > 200)
                throw new DomainException("Заголовок не должен превышать 200 символов.");

            if (ticket.Description?.Length > 2000)
                throw new DomainException("Описание не должно превышать 2000 символов.");

            if (ticket.CreatedAt == default)
                ticket.CreatedAt = DateTimeOffset.UtcNow;

            return await _ticketRepo.CreateAsync(ticket);
        }

        // 2. Назначение исполнителя
        public async Task AssignExecutorAsync(int ticketId, int employeeId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new NotFoundException($"Заявка с ID {ticketId} не найдена.");

            ticket.AssigneeId = employeeId;
            await _ticketRepo.UpdateAsync(ticket);
        }

        // 3. Смена статуса
        public async Task ChangeStatusAsync(int ticketId, Status newStatus)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new NotFoundException($"Заявка с ID {ticketId} не найдена.");

            if (!_transitionValidator.IsTransitionAllowed(ticket.Status, newStatus))
                throw new DomainException($"Недопустимый переход из {ticket.Status} в {newStatus}.");

            ticket.Status = newStatus;
            await _ticketRepo.UpdateAsync(ticket);
        }

        // 4. Получение просроченных заявок
        public Task GetOverdueTicketsAsync()
        {
            // В рамках лабораторной можно реализовать позже
            throw new NotImplementedException("Будет реализовано на следующем этапе.");
        }

        // 5. Получение статистики
        public Task GetStatisticsAsync()
        {
            // В рамках лабораторной можно реализовать позже
            throw new NotImplementedException("Будет реализовано на следующем этапе.");
        }

        // 6. Мягкое удаление сотрудника
        public async Task SoftDeleteEmployeeAsync(int employeeId)
        {
            if (await _employeeRepo.HasOpenTicketsAsync(employeeId))
                throw new DomainException("Нельзя удалить сотрудника, на котором висят незакрытые заявки.");

            await _employeeRepo.SoftDeleteAsync(employeeId);
        }
    }
}

