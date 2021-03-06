﻿using System;
using System.IO;
using System.Linq;

namespace Cams
{
	class Program
	{
		static void Main(string[] args)
		{
			string rawDir = Path.Combine(Settings.Path, "raw");
			string processedDir = Path.Combine(Settings.Path, "processed");

			var cameras = Directory.GetDirectories(rawDir).Select(BuildCamera);
			foreach (var cam in cameras)
				cam.Process(processedDir);

			var dates = Directory.GetDirectories(processedDir).Select(p => new VideoDate(p));
			foreach (var date in dates)
				date.Summarize();

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
