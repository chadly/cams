using System.IO;
using Microsoft.Extensions.Configuration;

namespace Cams
{
	public static class Settings
	{
		static readonly IConfigurationRoot config;

		static Settings()
		{
			config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("settings.json", optional: true)
				.Build();
		}

		public static string Path => config["path"];
		public static int MinutesToWaitBeforeProcessing => int.Parse(config["minutesToWaitBeforeProcessing"]);
		public static int DaysToKeep => int.Parse(config["daysToKeep"]);

		public static string MapCameraName(string cam)
		{
			string map = config[$"mappings:{cam}"];
			return map ?? cam;
		}
	}
}