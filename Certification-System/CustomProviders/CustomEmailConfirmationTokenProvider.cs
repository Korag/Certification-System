using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Certification_System.CustomProviders
{
    public class CustomEmailConfirmationTokenProvider<TUser>
                                        : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<EmailConfirmationTokenProviderOptions> options)
                                                            : base(dataProtectionProvider, options)
        {

        }
    }
}
