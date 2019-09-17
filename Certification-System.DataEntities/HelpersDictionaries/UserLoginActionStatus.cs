using System.Collections.Generic;

namespace Certification_System.Entities
{
    public static class UserLoginActionStatus
    {
        public static readonly Dictionary<int, string> UserLoginStatus= new Dictionary<int, string>()
         {
           {0, "Login successfull"},
           {1, "Login failed"},
           {2, "Email address not confirmed"},
           {3, "Logout successfull"},
        };
    }
}
