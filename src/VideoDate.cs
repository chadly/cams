using System;
using System.Diagnostics;
using System.IO;

namespace Cams
{
	public class VideoDate
	{
		public DateTime Date { get; private set; }
		public string DirPath { get; private set; }

		public VideoDate(string path)
		{
			Date = DateTime.Parse(new DirectoryInfo(path).Name);
			DirPath = path;
		}

		public void Summarize()
		{
			if (DeleteIfOld())
				return;

			DateTime threshold = DateTime.Now.AddDays(-1).Date;

			if (Date > threshold)
				return;

			var cameras = Directory.GetDirectories(DirPath);

			foreach (string camDir in cameras)
			{
				string camName = new DirectoryInfo(camDir).Name;

				if (Directory.Exists(camDir))
				{
					string outputFile = Path.Combine(DirPath, $"{camName}.mp4");

					if (!File.Exists(outputFile))
					{
						string summaryFile = Path.Combine(camDir, "summary.txt");
						string tmpMergedFile = Path.Combine(DirPath, $"{camName}-tmp.mp4");

						if (CreateSummaryFile(camDir, summaryFile))
						{
							if (MergeFilesForSummary(camName, summaryFile, tmpMergedFile))
							{
								if (FastForwardSummaryFile(camName, tmpMergedFile, outputFile))
								{
									Console.WriteLine($"Summarized {outputFile}");
								}
							}
						}

						//cleanup always; will create the files again if necessary
						File.Delete(summaryFile);
						File.Delete(tmpMergedFile);
					}
				}
			}
		}

		bool DeleteIfOld()
		{
			DateTime threshold = DateTime.Now.AddDays(-90).Date;

			if (Date < threshold)
			{
				Directory.Delete(DirPath, true);
				return true;
			}

			return false;
		}

		bool CreateSummaryFile(string camDir, string summaryFilePath)
		{
			var files = Directory.GetFiles(camDir, "*.mp4");
			if (files.Length < 1)
				return false;

			using (var summaryFile = File.Open(summaryFilePath, FileMode.Create, FileAccess.Write))
			{
				using (var writer = new StreamWriter(summaryFile))
				{
					foreach (var file in files)
					{
						writer.WriteLine($"file '{file}'");
					}
				}
			}

			return true;
		}

		bool MergeFilesForSummary(string camName, string summaryFile, string outputFile)
		{
			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					CreateNoWindow = true,
					FileName = @"D:\Downloads\ffmpeg-20171219-c94b094-win64-static\bin\ffmpeg.exe",
					Arguments = $"-y -safe 0 -f concat -i {summaryFile} -c copy {outputFile}",
				}
			})
			{
				process.Start();
				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					Console.WriteLine($"Failed to summarize {camName} for {Date.ToString("yyyy-MM-dd")}");
					return false;
				}
			}

			return true;
		}

		bool FastForwardSummaryFile(string camName, string mergedFile, string outputFile)
		{
			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					CreateNoWindow = true,
					FileName = @"D:\Downloads\ffmpeg-20171219-c94b094-win64-static\bin\ffmpeg.exe",
					Arguments = $"-i {mergedFile} -filter:v \"setpts = 0.01 * PTS\" {outputFile}",
				}
			})
			{
				process.Start();
				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					Console.WriteLine($"Failed to summarize {camName} for {Date.ToString("yyyy-MM-dd")}");
					return false;
				}
			}

			return true;
		}
	}
}