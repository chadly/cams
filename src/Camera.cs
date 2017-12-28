﻿using System;
using System.Collections.Generic;
using System.IO;

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

			Cleanup(file);

			Console.WriteLine($"Converted {outputFile}");
			return true;
		}

		protected abstract IEnumerable<VideoFile> DiscoverVideoFiles();
		protected abstract void Cleanup(VideoFile file);

		public override string ToString()
		{
			return Name;
		}
	}
}