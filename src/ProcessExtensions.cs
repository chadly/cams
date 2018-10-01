using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Cams
{
	public static class ProcessExtensions
	{
		/// <remarks>
		/// https://stackoverflow.com/a/50461641/316108
		/// </remarks>
		public static async Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
		{
			var tcs = new TaskCompletionSource<bool>();

			void Process_Exited(object sender, EventArgs e)
			{
				tcs.TrySetResult(true);
			}

			process.EnableRaisingEvents = true;
			process.Exited += Process_Exited;

			try
			{
				if (process.HasExited)
				{
					return;
				}

				using (cancellationToken.Register(() => tcs.TrySetCanceled()))
				{
					await tcs.Task;
				}
			}
			finally
			{
				process.Exited -= Process_Exited;
			}
		}
	}
}
