using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Identity.Mongo;
using Certification_System.Entities;
using Certification_System.Services;
using Certification_System.ServicesInterfaces;
using Certification_System.Repository.DAL;
using Certification_System.RepositoryInterfaces;
using Certification_System.Repository;
using Certification_System.Repository.Context;
using AutoMapper;
using Certification_System.Services.Models;
using Certification_System.ServicesInterfaces.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Certification_System
{
    public class Startup
    {
        private string ConnectionString => Configuration.GetConnectionString("AtlasConnection");

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<DataProtectionTokenProviderOptions>(o =>
              o.TokenLifespan = TimeSpan.FromHours(3));

            services.AddIdentityMongoDbProvider<CertificationPlatformUser>(mongo =>
            {
                mongo.ConnectionString = ConnectionString;

            }).AddTokenProvider<DataProtectorTokenProvider<CertificationPlatformUser>>("DeletionOfEntity")
              .AddTokenProvider<DataProtectorTokenProvider<CertificationPlatformUser>>("AssignToCourseQueue");

            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.Name = "Certification-Cookie";
                o.ExpireTimeSpan = TimeSpan.FromHours(5);
                o.SlidingExpiration = true;
            });

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            BsonSerializer.RegisterSerializer(DateTimeSerializer.LocalInstance);

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Services Layer
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton<IEmailConfiguration>
                                 (Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());

            services.AddTransient<IGeneratorQR, GeneratorQR>();
            services.AddTransient<IKeyGenerator, KeyGenerator>();

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

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddDataAnnotationsLocalization()
            .AddSessionStateTempDataProvider();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(5);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Account/Error");

                app.UseHsts();
            }

            var supportedCultures = new[] { new CultureInfo("pl-PL") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pl-PL"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Certificates}/{action=BlankMenu}/{id?}");   
            });
        }
    }
}
