using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cams
{
	class Program
	{
		static async Task Main(string[] args)
		{
			string rawDir = Path.Combine(Settings.Path, "raw");
			string processedDir = Path.Combine(Settings.Path, "processed");

			var cameraTasks = new List<Task>();

			var cameras = Directory.GetDirectories(rawDir).Select(BuildCamera);
			foreach (var cam in cameras)
				cameraTasks.Add(cam.Process(processedDir));

			await Task.WhenAll(cameraTasks);

			var dateTasks = new List<Task>();

			var dates = Directory.GetDirectories(processedDir).Select(p => new VideoDate(p));
			foreach (var date in dates)
				dateTasks.Add(date.Summarize());

			await Task.WhenAll(dateTasks);

#if DEBUG
			Console.WriteLine();
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
#endif
		}

		static Camera BuildCamera(string path)
		{
			bool isAmcrest = File.Exists(Path.Combine(path, "DVRWorkDirectory"));

			if (isAmcrest)
				return new AmcrestCamera(path);

			return new FoscamCamera(path);
		}
	}
}
