using System.Diagnostics;

namespace Cams
{
	public static class VideoConverter
	{
		public static bool CodecCopy(string inputFile, string outputFile)
		{
			return Run($"-y -i {inputFile} -codec copy {outputFile}");
		}

		public static bool Concat(string listFilePath, string outputFile)
		{
			return Run($"-y -safe 0 -f concat -i {listFilePath} -c copy {outputFile}");
		}

		public static bool FastForward(string inputFile, string outputFile)
		{
			return Run($"-y -i {inputFile} -filter:v \"setpts=0.01*PTS, scale=-1:720\" -an {outputFile}");
		}

		public static bool CheckValidVideoFile(string inputFile)
		{
			return Run($"-v error -i {inputFile} -f null -");
		}

		static bool Run(string args)
		{
			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "ffmpeg",
					Arguments = args
				}
			})
			{
				process.Start();
				process.WaitForExit();

				return process.ExitCode == 0;
			}
		}
	}
}
