using AspNetCoreHero.ToastNotification;
using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.Application.Common.Files;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ChoNongSan
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient();

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
			{
				opt.AccessDeniedPath = "/Account/Forbidden";
				opt.LoginPath = new PathString("/User/Login/");
			});

			//services.AddControllersWithViews();
			services.AddControllersWithViews().AddRazorRuntimeCompilation();
			services.AddSession(opt =>
			{
				opt.IdleTimeout = TimeSpan.FromHours(3);
			});

			services.AddTransient<IUserApi, UserApi>();
			services.AddTransient<ICategoryApi, CategoryApi>();
			services.AddTransient<ICtvApi, CtvApi>();
			services.AddTransient<IPostApi, PostApi>();
			services.AddTransient<IStorageService, FileStorageService>();
			services.AddTransient<IWeightApi, WeightApi>();
			services.AddTransient<IMeetApi, MeetApi>();
			services.AddTransient<IReviewApi, ReviewApi>();
			services.AddTransient<INapTienApi, NapTienApi>();

			IMvcBuilder builder = services.AddRazorPages();
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
#if DEBUG
            if (environment == Environments.Development)
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif

			services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseAuthentication();

			app.UseRouting();

			app.UseAuthorization();
			app.UseSession();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}