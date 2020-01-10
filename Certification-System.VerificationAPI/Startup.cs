using AutoMapper;
using Certification_System.Repository;
using Certification_System.Repository.Context;
using Certification_System.Repository.DAL;
using Certification_System.RepositoryInterfaces;
using Certification_System.Services;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Certification_System.VerificationAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // DAL Layer
            services.AddSingleton<MongoOperations, MongoOperations>();
            services.AddSingleton<MongoContext, MongoContext>();

            // Repository Layer
            services.AddTransient<IBranchRepository, BranchRepository>();
            services.AddTransient<ICertificateRepository, CertificateRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<IDegreeRepository, DegreeRepository>();
            services.AddTransient<IExamRepository, ExamRepository>();
            services.AddTransient<IExamTermRepository, ExamTermRepository>();
            services.AddTransient<IExamResultRepository, ExamResultRepository>();
            services.AddTransient<IGivenCertificateRepository, GivenCertificateRepository>();
            services.AddTransient<IGivenDegreeRepository, GivenDegreeRepository>();
            services.AddTransient<IMeetingRepository, MeetingRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            // Repository Logs
            services.AddTransient<IPersonalLogRepository, PersonalLogRepository>();
            services.AddTransient<ILogRepository, LogRepository>();

            // Logs Services
            services.AddTransient<IIPGetterService, IPGetterService>();
            services.AddTransient<ILogService, LogService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Certification-Platform Verification API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CPV API V1");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
