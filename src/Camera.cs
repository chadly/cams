using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Cams
{
	public class Camera
	{
		public string Name { get; private set; }
		public string RawFilePath { get; private set; }
		public CameraType Type { get; private set; }

		public Camera(string path)
		{
			var d = new DirectoryInfo(path);

			bool isAmcrest = File.Exists(Path.Combine(path, "DVRWorkDirectory"));

			Name = Settings.MapCameraName(d.Name);
			RawFilePath = path;
			Type = File.Exists(Path.Combine(path, "DVRWorkDirectory")) ? CameraType.Amcrest : CameraType.Foscam;
		}

		public void Process(string outputDir)
		{
			Console.WriteLine($"{Name} ({Type}): {RawFilePath}");

			var files = DiscoverVideoFiles();

			foreach (var file in files)
				ProcessFile(file, outputDir);

			Console.WriteLine();
		}

		bool ProcessFile(VideoFile file, string outputBase)
		{
			DateTime threshold = DateTime.Now.AddMinutes(Settings.MinutesToWaitBeforeProcessing);

			if (file.Date >= threshold)
			{
				Console.WriteLine($"File was written less than five minutes ago: {file.FilePath}");
				return false;
			}

			string outputFile = Path.Combine(outputBase, file.Date.ToString("yyyy-MM-dd"), Name, $"{file.Date.ToString("HH-mm-ss")}.mp4");
			string outputDir = new FileInfo(outputFile).DirectoryName;
			Directory.CreateDirectory(outputDir);

			if (!VideoConverter.CodecCopy(file.FilePath, outputFile))
			{
				Console.WriteLine($"Failed to convert {file}");
				return false;
			}

			file.Cleanup();

			Console.WriteLine($"Converted {outputFile}");
			return true;
		}

		IEnumerable<VideoFile> DiscoverVideoFiles()
		{
			switch (Type)
			{
				case CameraType.Amcrest:
					return DiscoverAmcrestFiles();

				case CameraType.Foscam:
					return DiscoverFoscamFiles();
			}

			return new VideoFile[0];
		}

		IEnumerable<VideoFile> DiscoverAmcrestFiles()
		{
			var dateDirs = Directory.GetDirectories(RawFilePath);

			foreach (string dateDir in dateDirs)
			{
				DateTime date = DateTime.Parse(new DirectoryInfo(dateDir).Name);

				var timeDirs = Directory.GetDirectories(Path.Combine(dateDir, "001", "dav"));

				foreach (string timeDir in timeDirs)
				{
					var files = Directory.GetFiles(timeDir, "*.dav");

					foreach (string file in files)
					{
						var match = Regex.Match(file, @"(\d{2})\.(\d{2})\.(\d{2})-\d{2}\.\d{2}\.\d{2}");
						if (!match.Success)
						{
							Console.WriteLine($"Filename is in an unexpected format: {file}");
						}
						else
						{
							int hour = int.Parse(match.Groups[1].Value);
							int minute = int.Parse(match.Groups[2].Value);
							int second = int.Parse(match.Groups[3].Value);
							date = new DateTime(date.Year, date.Month, date.Day, hour, minute, second);

							yield return new AmcrestVideoFile(file, date);
						}
					}
				}
			}
		}

		IEnumerable<VideoFile> DiscoverFoscamFiles()
		{
			var files = Directory.GetFiles(Path.Combine(RawFilePath, "record"), "*.mkv");

			foreach (string file in files)
			{
				var match = Regex.Match(file, @"alarm_(\d{4})(\d{2})(\d{2})_(\d{2})(\d{2})(\d{2}).mkv");
				if (!match.Success)
				{
					Console.WriteLine($"Filename is in an unexpected format: {file}");
				}
				else
				{
					int year = int.Parse(match.Groups[1].Value);
					int month = int.Parse(match.Groups[2].Value);
					int day = int.Parse(match.Groups[3].Value);
					int hour = int.Parse(match.Groups[4].Value);
					int minute = int.Parse(match.Groups[5].Value);
					int second = int.Parse(match.Groups[6].Value);
					DateTime date = new DateTime(year, month, day, hour, minute, second);

					yield return new FoscamVideoFile(file, date);
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}