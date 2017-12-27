using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Cams
{
	public class AmcrestCamera : Camera
	{
		public override void Process(string outputDir)
		{
			var dateDirs = Directory.GetDirectories(RawFilePath);

			Console.WriteLine($"{Name} (Amcrest): {RawFilePath}");

			foreach (string dir in dateDirs)
				ProcessDate(dir, outputDir);

			Console.WriteLine();
		}

		void ProcessDate(string path, string outputDir)
		{
			DateTime date = DateTime.Parse(new DirectoryInfo(path).Name);

			var timeDirs = Directory.GetDirectories(Path.Combine(path, "001", "dav"));

			bool allGood = true;

			foreach (string dir in timeDirs)
			{
				if (!ProcessTime(date, dir, outputDir))
					allGood = false;
			}

			if (allGood)
				Directory.Delete(path, true);
		}

		bool ProcessTime(DateTime date, string path, string outputDir)
		{
			var files = Directory.GetFiles(path, "*.dav");

			bool allGood = true;

			foreach (string file in files)
			{
				if (!ProcessFile(date, file, outputDir))
					allGood = false;
			}

			if (allGood)
				Directory.Delete(path, true);

			return allGood;
		}

		bool ProcessFile(DateTime date, string file, string outputBase)
		{
			var match = Regex.Match(file, @"(\d{2})\.(\d{2})\.(\d{2})-\d{2}\.\d{2}\.\d{2}");
			if (!match.Success)
			{
				Console.WriteLine($"Filename is in an unexpected format: {file}");
				return false;
			}

			int hour = int.Parse(match.Groups[1].Value);
			int minute = int.Parse(match.Groups[2].Value);
			int second = int.Parse(match.Groups[3].Value);
			date = new DateTime(date.Year, date.Month, date.Day, hour, minute, second);

			DateTime threshold = DateTime.Now.AddMinutes(Settings.MinutesToWaitBeforeProcessing);

			if (date >= threshold)
			{
				Console.WriteLine($"File was written less than five minutes ago: {file}");
				return false;
			}

			string outputFile = Path.Combine(outputBase, date.ToString("yyyy-MM-dd"), Name, $"{date.ToString("HH-mm-ss")}.mp4");
			string outputDir = new FileInfo(outputFile).DirectoryName;
			Directory.CreateDirectory(outputDir);

			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					CreateNoWindow = true,
					FileName = @"D:\Downloads\ffmpeg-20171219-c94b094-win64-static\bin\ffmpeg.exe",
					Arguments = $"-y -i {file} -codec copy {outputFile}",
				}
			})
			{
				process.Start();
				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					Console.WriteLine($"Failed to convert {file}");
					return false;
				}
			}

			//cleanup
			File.Delete(file);
			File.Delete(file.Replace(".dav", ".idx"));

			Console.WriteLine($"Converted {outputFile}");
			return true;
		}
	}
}