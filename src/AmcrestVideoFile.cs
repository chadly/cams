using System;
using System.IO;
using System.Linq;

namespace Cams
{
	public class AmcrestVideoFile : VideoFile
	{
		public AmcrestVideoFile(string path, DateTime date)
			: base(path, date) { }

		public override void Cleanup()
		{
			File.Delete(FilePath);
			File.Delete(FilePath.Replace(".dav", ".idx"));

			string timeDir = Path.Combine(FilePath, "..");
			if (!Directory.GetFileSystemEntries(timeDir).Any())
			{
				Directory.Delete(timeDir);

				string dateDir = Path.Combine(timeDir, "..");
				if (!Directory.GetFileSystemEntries(dateDir).Any())
				{
					Directory.Delete(dateDir);
				}
			}
		}
	}
}
