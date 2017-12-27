using System;
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
							Console.WriteLine($"Summarizing {camName} for {Date.ToString("yyyy-MM-dd")}");

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
			DateTime threshold = DateTime.Now.AddDays(-Settings.DaysToKeep).Date;

			if (Date < threshold)
			{
				Directory.Delete(DirPath, true);
				Console.WriteLine($"Cleared old videos: {Date.ToString("yyyy-MM-dd")}");
				return true;
			}

			return false;
		}

		bool CreateSummaryFile(string camDir, string summaryFilePath)
		{
			var files = Directory.GetFiles(camDir, "*.mp4");
			if (files.Length < 2)
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
			if (!VideoConverter.Concat(summaryFile, outputFile))
			{
				Console.WriteLine($"Failed to summarize {camName} for {Date.ToString("yyyy-MM-dd")}");
				return false;
			}

			return true;
		}

		bool FastForwardSummaryFile(string camName, string mergedFile, string outputFile)
		{
			if (!VideoConverter.FastForward(mergedFile, outputFile))
			{
				Console.WriteLine($"Failed to summarize {camName} for {Date.ToString("yyyy-MM-dd")}");
				return false;
			}

			return true;
		}
	}
}