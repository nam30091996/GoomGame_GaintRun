//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.IO;

namespace ArborEditor
{
	public static class PathUtility
	{
		public static string Combine(string path1, string path2)
		{
			string path = Path.Combine(path1, path2);
			if (Path.DirectorySeparatorChar != Path.AltDirectorySeparatorChar)
			{
				path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			}
			return path;
		}

		public static string Combine(params string[] paths)
		{
			if (paths == null || paths.Length == 0)
			{
				return string.Empty;
			}

			string path = paths[0];
			int count = paths.Length;
			for (int i = 1; i < count; ++i)
			{
				path = Path.Combine(path, paths[i]);
			}
			if (Path.DirectorySeparatorChar != Path.AltDirectorySeparatorChar)
			{
				path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			}
			return path;
		}
	}
}