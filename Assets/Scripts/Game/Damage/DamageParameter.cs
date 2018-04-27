namespace DroneDefender.Game.Damage {
	public class DamageParameter : IDamageParameter {
		public float Amount { get { return _amount; } }
		private float _amount;

		public int PlayerId { get {	return _playerId; }	}
		private int _playerId;

		public DamageParameter(float amount, int playerId) {
			_amount = amount;
			_playerId = playerId;
		}
	}
}
