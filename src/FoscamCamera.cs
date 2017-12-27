using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Cams
{
	public class FoscamCamera : Camera
	{
		public override void Process(string outputDir)
		{
			var files = Directory.GetFiles(Path.Combine(RawFilePath, "record"), "*.mkv");

			foreach (string file in files)
			{
				ProcessFile(file, outputDir);
			}
		}

		bool ProcessFile(string file, string outputBase)
		{
			var match = Regex.Match(file, @"alarm_(\d{4})(\d{2})(\d{2})_(\d{2})(\d{2})(\d{2}).mkv");
			if (!match.Success)
			{
				Console.WriteLine($"Filename is in an unexpected format: {file}");
				return false;
			}

			int year = int.Parse(match.Groups[1].Value);
			int month = int.Parse(match.Groups[2].Value);
			int day = int.Parse(match.Groups[3].Value);
			int hour = int.Parse(match.Groups[4].Value);
			int minute = int.Parse(match.Groups[5].Value);
			int second = int.Parse(match.Groups[6].Value);
			DateTime date = new DateTime(year, month, day, hour, minute, second);

			DateTime threshold = DateTime.Now.AddMinutes(-5);

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

			Console.WriteLine($"Converted {outputFile}");
			return true;
		}
	}
}