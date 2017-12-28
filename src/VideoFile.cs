using System;

namespace Cams
{
	public abstract class VideoFile
	{
		public string FilePath { get; private set; }
		public DateTime Date { get; private set; }

		public VideoFile(string path, DateTime date)
		{
			FilePath = path;
			Date = date;
		}

		public abstract void Cleanup();

		public override string ToString()
		{
			return FilePath;
		}
	}
}