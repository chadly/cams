using System;
using System.Collections.Generic;
using System.IO;

namespace Cams
{
	public abstract class Camera
	{
		const string CONVERT_FOOTAGE_FORMAT = "mp4";

		public string Name { get; private set; }
		public string RawFilePath { get; private set; }
		public abstract string Type { get; }

		public Camera(string path)
		{
			var d = new DirectoryInfo(path);
			Name = Settings.MapCameraName(d.Name);
			RawFilePath = path;
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
				Console.WriteLine($"File was written less than {Settings.MinutesToWaitBeforeProcessing} minutes ago: {file.FilePath}");
				return false;
			}

			string outputFile = Path.Combine(outputBase, file.Date.ToString("yyyy-MM-dd"), Name, $"{file.Date.ToString("HH-mm-ss")}.{CONVERT_FOOTAGE_FORMAT}");
			string outputDir = new FileInfo(outputFile).DirectoryName;
			Directory.CreateDirectory(outputDir);

			bool converted = true;

			if (file.HasExtension(CONVERT_FOOTAGE_FORMAT))
			{
				//save some time & just copy the file rather than going through ffmpeg
				File.Copy(file.FilePath, outputFile, true);
			}
			else if (!VideoConverter.CodecCopy(file.FilePath, outputFile))
			{
				converted = false;
				FallbackCopy(file, outputFile);
			}

			Cleanup(file);

			Console.WriteLine(converted ? $"Converted {outputFile}" : $"Failed to convert {outputFile}; copied instead");
			return true;
		}

		protected abstract IEnumerable<VideoFile> DiscoverVideoFiles();
		protected abstract void FallbackCopy(VideoFile file, string outputFile);
		protected abstract void Cleanup(VideoFile file);

		public override string ToString()
		{
			return Name;
		}
	}
}