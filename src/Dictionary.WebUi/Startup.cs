using System.IO;
using AutoMapper;
using Dictionary.Database;
using Dictionary.Database.Repositories.Word;
using Dictionary.Excel.Parsers;
using Dictionary.Excel.Parsers.Word;
using Dictionary.Services.Services.Import;
using Dictionary.Services.Services.Word;
using Dictionary.WebUi.AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dictionary.WebUi
{
    public class Startup
    {
        /// <summary>
        /// Absolute path to the database file.
        /// </summary>
        private readonly string _dbFileAbsolutePath;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            string dbFileDirectory = Path.Combine(env.ContentRootPath, Configuration["SQLite:folder"]);
            Directory.CreateDirectory(dbFileDirectory);
            _dbFileAbsolutePath = Path.Combine(dbFileDirectory, Configuration["SQLite:file"]);
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "npm-build"; });
            services.AddDbContext<DictionaryDb>(options => options.UseSqlite($"Data Source={_dbFileAbsolutePath}"));
            
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
            services.AddSingleton(config.CreateMapper());

            services.AddTransient<IWordRepository, WordRepository>();
            services.AddTransient<IWordService, WordService>();
            services.AddTransient<IExcelParser<WordImportModel>, WordsImportParser>();
            services.AddTransient<IImportService, ImportService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapDefaultControllerRoute();
            // });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}