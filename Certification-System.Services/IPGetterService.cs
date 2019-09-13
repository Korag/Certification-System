using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Sockets;

namespace Certification_System.Services
{
    public class IPGetterService : IIPGetterService
    {
        private IHttpContextAccessor _accessor;

        public IPGetterService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }

        public string GetGlobalIPAddress()
        {
            return _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}


