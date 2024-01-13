using Order.Domain.Validations.Base;

namespace Order.Request
{
    public class CreateUserRequest
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool Success { get; set; }
        public Report Report { get; set; }
    }
}