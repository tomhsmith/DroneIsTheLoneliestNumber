using strange.extensions.context.impl;

namespace DroneDefender.Game {
	public class MainGameRoot : ContextView {
		void Awake() {
			context = new MainGameContext(this);
		}
	}
}