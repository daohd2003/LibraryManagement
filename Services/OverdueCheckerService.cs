using LibraryManagement.Hubs;
using LibraryManagement.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace LibraryManagement.Services
{
    public class OverdueCheckerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OverdueCheckerService> _logger;

        public OverdueCheckerService(IServiceProvider serviceProvider, ILogger<OverdueCheckerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var borrowedBookRepository = scope.ServiceProvider.GetRequiredService<IBorrowedBookRepository>();
                        int updated = await borrowedBookRepository.UpdateOverdueStatusAsync();
                        _logger.LogInformation($"[OverdueChecker] {updated} records updated to 'Overdue' status.");

                        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<LibraryHub>>();
                        await hubContext.Clients.All.SendAsync("BookOverdue", $"{updated} sách đã bị quá hạn.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[OverdueChecker] Error occurred while checking overdue books.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Service is stopping
                }
            }
        }
    }
}
