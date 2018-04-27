using DroneDefender.Game.Signals.Generic;

namespace DroneDefender.Game.Model {
	public class DroneMothershipModel {
		public MoveSignal MoveSignal { get; set; }

		public DroneMothershipModel() {
			MoveSignal = new MoveSignal();
		}
	}
}
