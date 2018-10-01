using System;
using System.IO;
using System.Linq;

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

		public bool HasExtension(string extension)
		{
			if (string.IsNullOrWhiteSpace(extension))
				return false;

			string actual = Info.Name.Split('.').Last();
			return extension.Equals(actual, StringComparison.CurrentCultureIgnoreCase);
		}

		public override string ToString()
		{
			return FilePath;
		}
	}
}