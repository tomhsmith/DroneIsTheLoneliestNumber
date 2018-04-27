using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace DroneDefender.Game.Views {
	public class ClickDetector : View {
		public Signal clickSignal = new Signal();

		void OnMouseDown() {
			clickSignal.Dispatch();
		}
	}
}

