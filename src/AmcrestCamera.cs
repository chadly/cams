using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cams
{
	public class AmcrestCamera : Camera
	{
		public override string Type => "Amcrest";

		public AmcrestCamera(string path)
			: base(path) { }

		protected override IEnumerable<VideoFile> DiscoverVideoFiles()
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

							yield return new VideoFile(file, date);
						}
					}
				}
			}
		}

		protected override void FallbackCopy(VideoFile file, string outputFile)
		{
			var outputFileInfo = new FileInfo(outputFile);

			string newOutputFile = outputFile.Replace(outputFileInfo.Extension, ".dav");
			File.Copy(file.FilePath, newOutputFile, true);

			string newOutputIndex = outputFile.Replace(outputFileInfo.Extension, ".idx");
			File.Copy(file.FilePath.Replace(".dav", ".idx"), newOutputIndex, true);
		}

		protected override void Cleanup(VideoFile file)
		{
			File.Delete(file.FilePath);
			File.Delete(file.FilePath.Replace(".dav", ".idx"));

			string timeDir = Path.Combine(file.FilePath, "..");
			if (!Directory.GetFileSystemEntries(timeDir).Any())
			{
				Directory.Delete(timeDir);

				string dateDir = Path.Combine(timeDir, "..");
				if (!Directory.GetFileSystemEntries(dateDir).Any())
				{
					//the date directory is actually up two more levels (e.g. 2017-12-27/001/dav/11/whatever.mp4)
					Directory.Delete(Path.Combine(dateDir, "../../"), true);
				}
			}
		}
	}
}