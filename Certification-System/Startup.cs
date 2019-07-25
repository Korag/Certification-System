using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Identity.Mongo;
using Certification_System.Entities;
using Certification_System.ServicesInterfaces.IEmailSender;
using Certification_System.Services;
using Certification_System.ServicesInterfaces.IGeneratorQR;
using Certification_System.Repository.DAL;
using Certification_System.RepositoryInterfaces;
using Certification_System.Repository;
using Certification_System.Repository.Context;

namespace Certification_System
{
    public class Startup
    {
        private string ConnectionString => Configuration.GetConnectionString("DefaultConnection");

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


            services.AddIdentityMongoDbProvider<CertificationPlatformUser>(mongo =>
            {
                mongo.ConnectionString = ConnectionString;
            });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IGeneratorQR, GeneratorQR>();
            services.AddSingleton<MongoOperations, MongoOperations>();
            services.AddSingleton<MongoContext, MongoContext>();

            services.AddTransient<IBranchRepository, BranchRepository>();
            services.AddTransient<ICertificateRepository, CertificateRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<IDegreeRepository, DegreeRepository>();
            services.AddTransient<IGivenCertificateRepository, GivenCertificateRepository>();
            services.AddTransient<IGivenDegreeRepository, GivenDegreeRepository>();
            services.AddTransient<IInstructorRepository, InstructorRepository>();
            services.AddTransient<IMeetingRepository, MeetingRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
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
                app.UseExceptionHandler("/Home/Error");
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
