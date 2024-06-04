using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public int Strikes {  get; set; }
        
    }
}