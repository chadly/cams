using System.IO;

namespace Cams
{
	public abstract class Camera
	{
		public string Name { get; private set; }
		public string RawFilePath { get; private set; }

		public static Camera FromPath(string path)
		{
			var d = new DirectoryInfo(path);

			bool isAmcrest = File.Exists(Path.Combine(path, "DVRWorkDirectory"));

			Camera cam = isAmcrest ? new AmcrestCamera() : (Camera)new FoscamCamera();

			cam.Name = d.Name;
			cam.RawFilePath = path;
			return cam;
		}

		public abstract void Process(string outputDir);

		public override string ToString()
		{
			return Name;
		}
	}
}