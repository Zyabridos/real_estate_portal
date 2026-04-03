using RealEstate.Domain.Enums.Users;

namespace RealEstate.Domain.Entities.Users
{
    public sealed class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRoles Role { get; set; }
        public int? BrokerId { get; set; }
        public int? AgencyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
    }
}