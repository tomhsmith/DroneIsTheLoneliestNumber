using DroneDefender.Game.Signals;

namespace DroneDefender.Game.Inputs {
	internal class VRInputLeftView : VRInputViewBase {
		// Settings
		public ushort VibrationDuration = 1000;

		internal override ControllerTypes ControllerType { get { return ControllerTypes.VRLeft; } }

		protected override void FixedUpdate() {
			base.FixedUpdate();

			if (SteamVRController.padTouched) {
				ControllerSignal.Dispatch(InputContexts.PAD_TOUCHED);
			}

			if(SteamVRController.triggerPressed) {
				ControllerSignal.Dispatch(InputContexts.TRIGGER_CLICKED);
				OnVibrateHandler(VibrationDuration);
			}
		}

		protected override void TriggerClickedHandler(object sender, ClickedEventArgs e) {
			ControllerSignal.Dispatch(InputContexts.TRIGGER_CLICKED);
			OnVibrateHandler(VibrationDuration);
		}

		protected override void GrippedHandler(object sender, ClickedEventArgs e) {
			ControllerSignal.Dispatch(InputContexts.GRIPPED);
		}

		protected override void PadTouchedHandler(object sender, ClickedEventArgs e) {
			base.PadTouchedHandler(sender, e);

			ControllerSignal.Dispatch(InputContexts.PAD_TOUCHED);
		}

		protected override void PadUntouchedHandler(object sender, ClickedEventArgs e) {
			base.PadUntouchedHandler(sender, e);

			ControllerSignal.Dispatch(InputContexts.PAD_UNTOUCHED);
		}

		protected override void MenuButtonClickedHandler(object sender, ClickedEventArgs e) {
			base.MenuButtonClickedHandler(sender, e);

			ControllerSignal.Dispatch(InputContexts.MENU_BUTTON_CLICKED);
		}

	}
}
