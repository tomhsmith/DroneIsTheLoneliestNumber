namespace DroneDefender.Game.Damage {
	public interface IDamageParameter {
		float Amount { get; }
		int PlayerId { get; }
	}
}