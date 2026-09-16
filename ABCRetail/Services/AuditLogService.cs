using System.Security.Claims;

namespace ABCRetail.Services
{
    public class AuditLogService
    {
        private readonly FileStorageService _fileStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(
            FileStorageService fileStorageService,
            IHttpContextAccessor httpContextAccessor)
        {
            _fileStorageService = fileStorageService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(
            string action)
        {
            var user =
                _httpContextAccessor.HttpContext?
                    .User;

            string userName =
                user?.Identity?.IsAuthenticated == true
                    ? user.Identity.Name ?? "Authenticated User"
                    : "Anonymous User";

            string timestamp =
                DateTime.Now.ToString(
                    "yyyy-MM-dd HH:mm:ss");

            string logMessage =
                $"[{timestamp}] User: {userName} | Action: {action}";

            await _fileStorageService.WriteLogAsync(
                "application-log.txt",
                logMessage);
        }
    }
}