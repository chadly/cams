using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

		public async Task Summarize()
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

						Console.WriteLine($"Summarizing {camName} for {Date.ToString("yyyy-MM-dd")}");

						if (await CreateSummaryFile(camDir, summaryFile))
						{
							if (await MergeFilesForSummary(camName, summaryFile, tmpMergedFile))
							{
								if (await FastForwardSummaryFile(camName, tmpMergedFile, outputFile))
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

		async Task<bool> CreateSummaryFile(string camDir, string summaryFilePath)
		{
			//https://stackoverflow.com/questions/10806951/how-to-limit-the-amount-of-concurrent-async-i-o-operations

			var calcValidFiles = new List<Task<FileValidity>>();
			foreach (var file in Directory.GetFiles(camDir, "*.mp4"))
				calcValidFiles.Add(CalculateFileValidity(file));

			var files = (await Task.WhenAll(calcValidFiles))
				.Where(f => f.IsValid)
				.Select(f => f.FilePath)
				.OrderBy(f => f)
				.ToArray();

			if (files.Length < 2)
			{
				Console.WriteLine($"Nothing to summarize for {camDir}");
				return false;
			}

			using (var summaryFile = File.Open(summaryFilePath, FileMode.Create, FileAccess.Write))
			{
				using (var writer = new StreamWriter(summaryFile))
				{
					foreach (var file in files)
					{
						await writer.WriteLineAsync($"file '{file}'");
					}
				}
			}

			return true;
		}

		async Task<FileValidity> CalculateFileValidity(string file)
		{
			var info = new FileInfo(file);
			Console.WriteLine($"Checking {info.Name} for validity...");
			bool isValid = await VideoConverter.CheckValidVideoFile(file);
			Console.WriteLine(isValid ? $"{info.Name} Valid" : $"{info.Name} Invalid");

			return new FileValidity
			{
				FilePath = file,
				IsValid = isValid
			};
		}

		class FileValidity
		{
			public string FilePath { get; set; }
			public bool IsValid { get; set; }
		}

		async Task<bool> MergeFilesForSummary(string camName, string summaryFile, string outputFile)
		{
			if (!await VideoConverter.Concat(summaryFile, outputFile))
			{
				Console.WriteLine($"Failed to summarize {camName} for {Date.ToString("yyyy-MM-dd")}");
				return false;
			}

			return true;
		}

		async Task<bool> FastForwardSummaryFile(string camName, string mergedFile, string outputFile)
		{
			if (!await VideoConverter.FastForward(mergedFile, outputFile))
			{
				Console.WriteLine($"Failed to summarize {camName} for {Date.ToString("yyyy-MM-dd")}");
				return false;
			}

			return true;
		}
	}
}