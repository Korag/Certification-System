using Autofac;
using Certification_System.DAL;

namespace Certification_System.App_Start
{
    public class AutofacConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MongoOperations>().As<IDatabaseOperations>().InstancePerRequest();
            builder.RegisterType<MongoContext>().AsSelf().InstancePerRequest();
            var container = builder.Build();
        }
    }
}