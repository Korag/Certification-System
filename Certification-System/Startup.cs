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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddDefaultUI(UIFramework.Bootstrap4)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<DataProtectionTokenProviderOptions>(o =>
              o.TokenLifespan = TimeSpan.FromHours(3));

            services.AddIdentityMongoDbProvider<CertificationPlatformUser>(mongo =>
            {
                mongo.ConnectionString = ConnectionString;
            });

            services.ConfigureApplicationCookie(o => {
                o.Cookie.Name = "Certification-System";
                o.ExpireTimeSpan = TimeSpan.FromHours(10);
                o.SlidingExpiration = true;
            });

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Services Layer
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());

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

            services.AddTransient<ILogRepository, LogRepository>();

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddDataAnnotationsLocalization();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

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
