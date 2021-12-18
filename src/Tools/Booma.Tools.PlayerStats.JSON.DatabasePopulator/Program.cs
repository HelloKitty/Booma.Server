using System;
using Booma.Server.CharacterDataService;
using Microsoft.Extensions.DependencyInjection;

namespace Booma.Tools.PlayerStats.JSON.DatabasePopulator
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceCollection services = new ServiceCollection();
			var mvcBuilder = services.AddMvc();
			Startup.RegisterGladerRPGSystem(services, mvcBuilder);
			var provider = services.BuildServiceProvider();
		}
	}
}
