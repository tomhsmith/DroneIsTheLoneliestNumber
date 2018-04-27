using strange.extensions.context.impl;

namespace DroneDefender.Game {
	public class SelectionScreenRoot : ContextView {
		void Awake() {
			context = new SelectionScreenContext(this);
		}
	}
}