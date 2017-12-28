using System;
using System.IO;

namespace Cams
{
	public class FoscamVideoFile : VideoFile
	{
		public FoscamVideoFile(string path, DateTime date)
			: base(path, date) { }

		public override void Cleanup()
		{
			File.Delete(FilePath);
		}
	}
}