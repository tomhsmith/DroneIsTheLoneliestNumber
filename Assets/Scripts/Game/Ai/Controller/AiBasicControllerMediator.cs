using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Ai.Controller {
	class AiBasicControllerMediator : Mediator {
		[Inject] public AiBasicControllerView View { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
