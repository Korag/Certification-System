using Microsoft.AspNetCore.Identity;
using System;

namespace Certification_System.CustomProviders
{
    public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public EmailConfirmationTokenProviderOptions()
        {
            Name = "EmailDataProtectorTokenProvider";
            TokenLifespan = TimeSpan.FromHours(3);
        }
    }
}
