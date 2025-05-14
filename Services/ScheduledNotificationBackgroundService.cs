
namespace LibraryManagement.Services
{
    public class ScheduledNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ScheduledNotificationBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var scheduledTime = DateTime.Today.AddHours(8);
                if (now > scheduledTime)
                    scheduledTime = scheduledTime.AddDays(1);

                var delay = scheduledTime - now;

                // Delay tới đúng 8h sáng
                await Task.Delay(delay, stoppingToken);

                // Thực thi công việc
                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    await service.SendDueBookNotificationsAsync();
                }

                // Chờ đúng 24 giờ nữa
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }

        }
    }
}
