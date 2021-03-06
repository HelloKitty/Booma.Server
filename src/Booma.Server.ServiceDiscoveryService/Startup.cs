using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Glader.ASP.ServiceDiscovery;
using Microsoft.EntityFrameworkCore;

namespace Booma
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
			//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.CheckCertificateRevocationList = false;

			//Don't remember why this is needed, BUT old Auth service had it.
			services.Configure<IISOptions>(options =>
			{
				options.AutomaticAuthentication = false;
			});

			//Just add the SD controller
			services.AddControllers()
				.RegisterServiceDiscoveryController()
				.AddNewtonsoftJson();

			//And this is the SD database and database services.
			services.RegisterServiceDiscoveryDatabase(builder =>
			{
				builder.UseMySql("server=127.0.0.1;port=3306;Database=booma.global;Uid=root;Pwd=test;", optionsBuilder =>
				{
					//Required for external migrations to run.
					optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName);
				});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
