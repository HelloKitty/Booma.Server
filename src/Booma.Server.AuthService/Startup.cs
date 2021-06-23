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
using Autofac;
using Glader.ASP.Authentication;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using Microsoft.EntityFrameworkCore;
using Refit;

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

			//Just add the Auth controller
			services.AddControllers()
				.RegisterAuthenticationController()
				.AddNewtonsoftJson();

			//And this is the Auth database and database services.
			services.RegisterAuthenticationDatabase(builder =>
			{
				builder.UseMySql("server=127.0.0.1;port=3306;Database=booma.auth;Uid=root;Pwd=test;", optionsBuilder =>
				{
					//Required for external migrations to run.
					optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName);
				});
			});

			//This feature was created to add claims to the JWT for the character
			//Claims appenders
			services.AddTransient<IAuthorizedClaimsAppender, AuthorizedCharacterClaimAppender>();

			services.RegisterGladerIdentity();
		}

		//See: https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html
		// ConfigureContainer is where you can register things directly
		// with Autofac. This runs after ConfigureServices so the things
		// here will override registrations made in ConfigureServices.
		// Don't build the container; that gets done for you by the factory.
		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule<ServiceDiscoveryServiceModule>();
			builder.RegisterModule(new BoomaServiceDiscoverableServiceModule<ICharacterDataQueryService<StubEnum, StubEnum>>(BoomaServiceType.CharacterDataService)
			{
				//TODO: Eventually we'll need to Auth the auth server (lol) somehow so that we can Role check requests
				Mode = ServiceDiscoveryModuleMode.Default
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

			//Must call UseAuthentication for Glader Identity library to work.
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
