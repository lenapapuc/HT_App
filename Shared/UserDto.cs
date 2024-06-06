using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UserDto
    {
        public DateTime? DateCreated {  get; set; }
        public string Name { get; set; }
        public int Strikes {  get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
    }
}
