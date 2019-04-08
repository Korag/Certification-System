using MongoDB.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Certification_System.DAL
{
    interface IDatabaseOperations
    {
        List<IdentityUser> GetUsers();
    }
}