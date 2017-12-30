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

				//it's a time subdirectory or a weird 001 directory or something
				//just need to loop through each subdir looking for a dav or mp4 directory
				var subdirs = Directory.GetDirectories(dateDir);

				foreach (string subdir in subdirs)
				{
					string mp4dir = Path.Combine(subdir, "mp4");
					string davdir = Path.Combine(subdir, "dav");

					IEnumerable<string> files = new string[0];

					if (Directory.Exists(mp4dir))
					{
						files = Directory.GetFiles(mp4dir, "*.mp4");
					}
					else if (Directory.Exists(davdir))
					{
						var moreDamnSubdirs = Directory.GetDirectories(davdir);

						files = new List<string>();
						foreach (string anotherSubdir in moreDamnSubdirs)
						{
							((List<string>)files).AddRange(Directory.GetFiles(anotherSubdir, "*.dav"));
						}
					}

					foreach (string file in files)
					{
						var vid = ProcessFile(date, file);
						if (vid != null)
							yield return vid;
					}
				}
			}
		}

		VideoFile ProcessFile(DateTime date, string file)
		{
			var match = Regex.Match(file, @"(\d{2})\.(\d{2})\.(\d{2})-\d{2}\.\d{2}\.\d{2}");
			if (!match.Success)
			{
				Console.WriteLine($"Filename is in an unexpected format: {file}");
				return null;
			}

			int hour = int.Parse(match.Groups[1].Value);
			int minute = int.Parse(match.Groups[2].Value);
			int second = int.Parse(match.Groups[3].Value);
			date = new DateTime(date.Year, date.Month, date.Day, hour, minute, second);

			return new VideoFile(file, date);
		}

		protected override void FallbackCopy(VideoFile file, string outputFile)
		{
			var outputFileInfo = new FileInfo(outputFile);

			string newOutputFile = outputFile.Replace(outputFileInfo.Extension, file.Info.Extension);
			File.Copy(file.FilePath, newOutputFile, true);

			string newOutputIndex = outputFile.Replace(outputFileInfo.Extension, ".idx");
			File.Copy(file.FilePath.Replace(file.Info.Extension, ".idx"), newOutputIndex, true);
		}

		protected override void Cleanup(VideoFile file)
		{
			File.Delete(file.FilePath);
			File.Delete(file.FilePath.Replace(file.Info.Extension, ".idx"));

			//keep going up directories until you get back to the date dir
			string dir = file.Info.DirectoryName;
			while (!Regex.Match(dir, @"\d{4}-\d{2}-\d{2}$").Success)
			{
				ClearDirectoryIfEmpty(dir);
				dir = Path.GetFullPath(Path.Combine(dir, ".."));
			}

			ClearDirectoryIfEmpty(dir);
		}

		void ClearDirectoryIfEmpty(string dir)
		{
			if (!Directory.GetFileSystemEntries(dir).Any())
				Directory.Delete(dir);
		}
	}
}