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
			string path = args[0];

			var cameras = GetCameras(path);
			foreach (var cam in cameras)
				cam.Process(path);

			var dates = GetVideoDates(path);
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
