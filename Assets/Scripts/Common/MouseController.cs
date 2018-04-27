using System.Runtime.InteropServices;

namespace DroneDefender.Common {
	public static class MouseController {
		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out Point pos);
	}

	public struct Point {
		public int x;
		public int y;
	}
}
