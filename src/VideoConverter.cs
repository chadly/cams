using System.Diagnostics;
using System.Threading.Tasks;

namespace Cams
{
	public static class VideoConverter
	{
		public static Task<bool> CodecCopy(string inputFile, string outputFile)
		{
			return Run($"-y -i {inputFile} -codec copy {outputFile}");
		}

		public static Task<bool> Concat(string listFilePath, string outputFile)
		{
			return Run($"-y -safe 0 -f concat -i {listFilePath} -c copy {outputFile}");
		}

		public static Task<bool> FastForward(string inputFile, string outputFile)
		{
			return Run($"-y -i {inputFile} -filter:v \"setpts = 0.01 * PTS\" -an {outputFile}");
		}

		public static Task<bool> CheckValidVideoFile(string inputFile)
		{
			return Run($"-v error -i {inputFile} -f null -");
		}

		static async Task<bool> Run(string args)
		{
			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					CreateNoWindow = true,
					FileName = "ffmpeg",
					Arguments = args
				}
			})
			{
				process.Start();
				await process.WaitForExitAsync();

				return process.ExitCode == 0;
			}
		}
	}
}
