using ChoNongSan.Application.AddressSv;
using ChoNongSan.Application.Admin.ManagementCategories;
using ChoNongSan.Application.Admin.ManagementCTVes;
using ChoNongSan.Application.Common;
using ChoNongSan.Application.Common.Accounts;
using ChoNongSan.Application.Common.Banners;
using ChoNongSan.Application.Common.DonVi;
using ChoNongSan.Application.Common.Files;
using ChoNongSan.Application.CTV;
using ChoNongSan.Application.KhachHang.Posts;
using ChoNongSan.Application.LichHen;
using ChoNongSan.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChoNongSan.Api
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
			services.AddDbContext<ChoNongSanContext>(opt =>
				opt.UseSqlServer(Configuration.GetConnectionString("ChoNongSanDB")));

			services.AddTransient<IAccountService, AccountService>();
			services.AddTransient<IManagementCtvService, ManagementCtvService>();
			services.AddTransient<ICatService, CatService>();
			services.AddTransient<IPostService, PostService>();
			services.AddTransient<ICtvService, CtvService>();
			services.AddTransient<IStorageService, FileStorageService>();
			services.AddTransient<IBannerService, BannerService>();
			services.AddTransient<IMailService, MailService>();
			services.AddTransient<IWeightService, WeightService>();
			services.AddTransient<IAddressService, AddressService>();
			services.AddTransient<IMeetService, MeetService>();
			//services.AddTransient<UserManager<Account>, UserManager<Account>>();

			//services.AddTransient<SignInManager<Account>, SignInManager<Account>>();

			services.AddControllers();
			//services.AddSession();

			//var jwtSection = Configuration.GetSection("JWTSettings");
			//services.Configure<JWTSettings>(jwtSection);

			////to validate the token which has been sent by clients
			//var appSettings = jwtSection.Get<JWTSettings>();
			//var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

			////add authentication
			//services.AddAuthentication(x =>
			//{
			//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			//})
			//.AddJwtBearer(x =>
			//{
			//    x.RequireHttpsMetadata = true;
			//    x.SaveToken = true;
			//    x.TokenValidationParameters = new TokenValidationParameters
			//    {
			//        ValidateIssuerSigningKey = true,
			//        IssuerSigningKey = new SymmetricSecurityKey(key),
			//        ValidateIssuer = false,
			//        ValidateAudience = false,
			//        ClockSkew = TimeSpan.Zero
			//    };
			//});

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "Swagger ChoNongSan", Version = "v1.0" });

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement()
				  {
					{
					  new OpenApiSecurityScheme
					  {
						Reference = new OpenApiReference
						  {
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						  },
						  Scheme = "oauth2",
						  Name = "Bearer",
						  In = ParameterLocation.Header,
					   },
						new List<string>()
					}
				});
			});

			string issuer = Configuration.GetValue<string>("Tokens:Issuer");
			string signingKey = Configuration.GetValue<string>("Tokens:Key");
			byte[] signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);

			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidIssuer = issuer,
					ValidateAudience = true,
					ValidAudience = issuer,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.Zero,
					IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
				};
			});
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
			//app.UseSession();

			app.UseAuthorization();

			app.UseSwagger();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Swagger ChoNongSan v1.0");
				//c.RoutePrefix = string.Empty;
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}