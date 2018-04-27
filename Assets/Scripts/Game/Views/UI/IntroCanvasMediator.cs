using strange.extensions.mediation.impl;

namespace DroneDefender.Game.Views.UI {
	class IntroCanvasMediator : Mediator {
		[Inject] public IntroCanvasView View { get; set; }
		
		public override void OnRegister() {
			base.OnRegister();

			View.Init();
		}
	}
}
