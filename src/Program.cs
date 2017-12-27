using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cams
{
	class Program
	{
		static void Main(string[] args)
		{
			var cameras = GetCameras(Settings.Path);
			foreach (var cam in cameras)
				cam.Process(Settings.Path);

			var dates = GetVideoDates(Settings.Path);
			foreach (var date in dates)
				date.Summarize();

			Console.ReadKey();
		}

		static IEnumerable<Camera> GetCameras(string basePath)
		{
			string dir = Path.Combine(basePath, "raw");
			var paths = Directory.GetDirectories(dir);
			return paths.Select(p => Camera.FromPath(p));
		}

		static IEnumerable<VideoDate> GetVideoDates(string basePath)
		{
			var paths = Directory.GetDirectories(basePath);
			return paths.Where(p => new DirectoryInfo(p).Name != "raw").Select(p => new VideoDate(p));
		}
	}
}
