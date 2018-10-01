using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cams
{
	public abstract class Camera
	{
		public string Name { get; private set; }
		public string RawFilePath { get; private set; }
		public abstract string Type { get; }

		public Camera(string path)
		{
			var d = new DirectoryInfo(path);
			Name = Settings.MapCameraName(d.Name);
			RawFilePath = path;
		}

		public async Task Process(string outputDir)
		{
			Console.WriteLine($"{Name} ({Type}): {RawFilePath}");

			var files = DiscoverVideoFiles();

			var tasks = new List<Task>();

			foreach (var file in files)
				tasks.Add(ProcessFile(file, outputDir));

			await Task.WhenAll(tasks);

			Console.WriteLine();
		}

		async Task<bool> ProcessFile(VideoFile file, string outputBase)
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

			bool converted = true;

			if (!await VideoConverter.CodecCopy(file.FilePath, outputFile))
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