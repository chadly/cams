using System;

namespace Cams
{
	public class VideoFile
	{
		public string FilePath { get; private set; }
		public DateTime Date { get; private set; }

		public VideoFile(string path, DateTime date)
		{
			FilePath = path;
			Date = date;
		}

		public override string ToString()
		{
			return FilePath;
		}
	}
}