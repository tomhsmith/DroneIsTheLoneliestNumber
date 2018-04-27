using DroneDefender.Game.Signals;
using DroneDefender.Game.Views.Projectile;
using DroneDefender.Resource.Prefab;
using UnityEngine;

namespace DroneDefender.Game.Inputs {
	internal class VRInputRightView : VRInputViewBase {
		// Settings
		public ushort VibrationDuration = 800;
		public string OtherControllerContainerName = "Secondary Controller Top";

		internal override ControllerTypes ControllerType => ControllerTypes.VRRight;

		// Local Components
		private GameObject _otherControllerGO;
		private GunView _gunView;
		private ControllerDisplayModes _displayMode;

		internal override void Init() {
			base.Init();

			_otherControllerGO = new GameObject(OtherControllerContainerName);
			_otherControllerGO.transform.parent = transform;
			_otherControllerGO.transform.localPosition = Vector3.zero;

			_gunView = _otherControllerGO.AddComponent<GunView>()
				.ConfigureGun(PrefabResources.Gun)
				.ConfigureBullet<BulletView>(PrefabResources.Bullet);

			SetDisplayMode(ControllerDisplayModes.FIRST);
		}

		protected override void FixedUpdate() {
			base.FixedUpdate();

			if(SteamVRController.padTouched) {
				ControllerSignal.Dispatch(InputContexts.PAD_TOUCHED);
			}
		}

		protected override void TriggerClickedHandler(object sender, ClickedEventArgs e) {
			switch(_displayMode) {
				case ControllerDisplayModes.FIRST:
					ControllerSignal.Dispatch(InputContexts.TRIGGER_CLICKED);
					break;
				case ControllerDisplayModes.SECOND:
					_gunView.FireSignal.Dispatch();
					break;
			}

			OnVibrateHandler(VibrationDuration);
		}

		protected override void GrippedHandler(object sender, ClickedEventArgs e) {
			ControllerSignal.Dispatch(InputContexts.GRIPPED);
			OnVibrateHandler(VibrationDuration);

			switch(_displayMode) {
				case ControllerDisplayModes.FIRST:
					SetDisplayMode(ControllerDisplayModes.SECOND);
					break;
				case ControllerDisplayModes.SECOND:
					SetDisplayMode(ControllerDisplayModes.FIRST);
					break;
			}
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

		private void SetDisplayMode(ControllerDisplayModes mode) {
			switch (mode) {
				case ControllerDisplayModes.FIRST:
					_otherControllerGO.SetActive(false);
					break;
				case ControllerDisplayModes.SECOND:
					_otherControllerGO.SetActive(true);
					break;
			}

			_displayMode = mode;
		}
	}
}
