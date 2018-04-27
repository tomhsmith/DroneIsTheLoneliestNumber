using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Ai.Action {
	class AiRotationalMovementActionMediator : Mediator {
		[Inject] public AiRotationalMovementActionView View { get; set; }

		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
