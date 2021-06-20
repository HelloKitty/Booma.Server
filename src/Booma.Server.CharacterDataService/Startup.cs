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
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using GGDBF;
using Glader.Essentials;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Glader.ASP.RPG;

namespace Booma.Server.CharacterDataService
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
			services.AddControllers()
				.RegisterCharacterDataController<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>, CharacterRace, CharacterClass>()
				.RegisterGGDBFController()
				.AddNewtonsoftJson(options =>
				{
					//Required for serializing GGDBF tables
					options.RegisterGGDBFSerializers();
				});

			//Required to register the GGDBF over HTTP system.
			services.RegisterGGDBFContentServices<EntityFrameworkGGDBFDataSource, AutoMapperGGDBFDataConverter, RPGStaticDataContext<DefaultTestSkillType, CharacterRace, CharacterClass, PsobbProportionSlots, PsobbCustomizationSlots, CharacterStatType>>();

			services.RegisterCharacterDatabase<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>, CharacterRace, CharacterClass, DefaultTestSkillType, CharacterStatType>(builder =>
			{
				builder.UseMySql("server=127.0.0.1;port=3306;Database=booma.game;Uid=root;Pwd=test;", optionsBuilder =>
				{
					optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName);
				});
			}, true); //true required for GGDBF

			services.RegisterGladerASP();

			//TODO: Support real certs. This is a complete DEV ONLY HACK!
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					//TODO: This is demo test signing code from: https://github.com/openiddict/openiddict-core/blob/7e1c9dd1307f8127288682459b0b1d82ea804e4f/src/OpenIddict.Server/OpenIddictServerBuilder.cs#L647
					using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
					store.Open(OpenFlags.ReadWrite);

					var subject = new X500DistinguishedName("CN=OpenIddict Server Signing Certificate");
					var certificate = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, subject.Name, validOnly: false)
						.OfType<X509Certificate2>()
						.SingleOrDefault();

					options.TokenValidationParameters
						.IssuerSigningKey = new X509SecurityKey(certificate);

					//TODO: This audience stuff is ALL WRONG.
					options.Audience = "auth-server";
					options.TokenValidationParameters.ValidIssuers = new [] {"https://localhost:5003/", "https://127.0.0.1:5003/"};
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
