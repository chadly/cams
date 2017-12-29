using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Cams
{
	public class FoscamCamera : Camera
	{
		public override string Type => "Foscam";

		public FoscamCamera(string path)
			: base(path) { }

		protected override IEnumerable<VideoFile> DiscoverVideoFiles()
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

					yield return new VideoFile(file, date);
				}
			}
		}

		protected override void FallbackCopy(VideoFile file, string outputFile)
		{
			var outputFileInfo = new FileInfo(outputFile);
			string newOutputFile = outputFile.Replace(outputFileInfo.Extension, ".mkv");

			File.Copy(file.FilePath, newOutputFile, true);
		}

		protected override void Cleanup(VideoFile file)
		{
			File.Delete(file.FilePath);
		}
	}
}