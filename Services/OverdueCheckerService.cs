using LibraryManagement.Repositories;

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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var borrowedBookRepository = scope.ServiceProvider.GetRequiredService<IBorrowedBookRepository>();
                    int updated = await borrowedBookRepository.UpdateOverdueStatusAsync();
                    _logger.LogInformation($"[OverdueChecker] {updated} records updated to 'Overdue' status.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // kiểm tra mỗi giờ
            }
        }
    }
}
