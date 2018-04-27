namespace DroneDefender.Game.Model {
	[System.Serializable]
	public class SettingsModel {
		public GameModes GameMode { get; internal set; } = GameModes.FPS;
	}
}
