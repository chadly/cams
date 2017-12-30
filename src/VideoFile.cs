using System;
using System.IO;

namespace Cams
{
	public class VideoFile
	{
		public string FilePath { get; private set; }
		public DateTime Date { get; private set; }

		FileInfo info;

		public FileInfo Info
		{
			get
			{
				if (info == null)
					info = new FileInfo(FilePath);
				return info;
			}
		}

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