using DroneDefender.Common;
using DroneDefender.Common.Extension;
using DroneDefender.Game.Signals;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace DroneDefender.Game.Inputs {
	public abstract class VRInputViewBase : View {
		// Settings
		public float TouchpadSensitivity = .1f;
		public float TouchpadGORotationMultiplier = 45f;

		// Vibration
		public Signal<ushort> VibrateControllerSignal { get; private set; }

		// Device
		protected SteamVR_Controller.Device SteamVRDevice { get; private set; }
		protected SteamVR_TrackedController SteamVRController { get; private set; }

		internal ControllerSignal ControllerSignal;

		abstract internal ControllerTypes ControllerType { get; }

		// Trigger
		public Vector3 TriggerGOStartingScale { get; private set; }

		public float TriggerCickStrength => _triggerClickStrength;
		private float _triggerClickStrength;

		#region SteamVR Wrapper

		// Wrappers for Steam VR Interaction
		internal Vector2 TouchpadVector => SteamVRDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
		internal bool Gripped => SteamVRController.gripped;
		internal bool TriggerPressed => SteamVRController.triggerPressed;
		protected Vector3 ControllerPosition => SteamVRController.transform.position;
		private bool _touchpadTouched => SteamVRDevice.GetTouch(SteamVR_Controller.ButtonMask.Touchpad);

		#endregion

		// Local Components
		private NullableResultDictionary<GameObject> _children;
		private GameObject _touchpadGO;
		private Quaternion? _startTouchpadRotation;

		#region Base View

		internal virtual void Init() {
			SteamVRController = GetComponent<SteamVR_TrackedController>()
				?? gameObject.AddComponent<SteamVR_TrackedController>();

			_children = this.ChildrenByTagDictionary();
			_touchpadGO = _children.GetNullableFirst(TagResources.Touchpad);
			_startTouchpadRotation = _touchpadGO?.transform.localRotation;

			TriggerGOStartingScale = _children?.GetNullableFirst(TagResources.Trigger)?.transform.localScale ?? Vector3.one;

			// Add SteamVR Control Handlers
			SteamVRController.TriggerClicked += TriggerClickedHandler;
			SteamVRController.PadTouched += PadTouchedHandler;
			SteamVRController.PadUntouched += PadUntouchedHandler;
			SteamVRController.MenuButtonClicked += MenuButtonClickedHandler;
			SteamVRController.Gripped += GrippedHandler;

			VibrateControllerSignal = new Signal<ushort>();
			VibrateControllerSignal.AddListener(OnVibrateHandler);

			ControllerSignal = new ControllerSignal();
		}

		protected virtual void FixedUpdate() {
			SteamVRDevice = SteamVR_Controller.Input((int)SteamVRController.controllerIndex);
		}

		protected void Update() {
			_triggerClickStrength = SteamVRDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
			TriggerPressedHandler();

			if (_touchpadTouched) {
				PadTouchedHandler(this, new ClickedEventArgs());
			}
		}

		protected override void OnDestroy() {
			// Remove SteamVR Controller Handlers
			SteamVRController.TriggerClicked -= TriggerClickedHandler;
			SteamVRController.PadUntouched -= PadTouchedHandler;
			SteamVRController.PadUntouched -= PadUntouchedHandler;
			SteamVRController.MenuButtonClicked -= MenuButtonClickedHandler;
			SteamVRController.Gripped -= GrippedHandler;

			base.OnDestroy();
		}

		#endregion

		#region Handlers

		private void TriggerPressedHandler() {
			var triggerGO = _children?.GetNullableFirst(TagResources.Trigger);

			if (triggerGO == null) { return; }

			if (TriggerCickStrength > 0) {
				triggerGO.transform.localScale = TriggerGOStartingScale * (1 + TriggerCickStrength);
			} else {
				triggerGO.transform.localScale = TriggerGOStartingScale;
			}
		}

		protected abstract void TriggerClickedHandler(object sender, ClickedEventArgs e);
		protected abstract void GrippedHandler(object sender, ClickedEventArgs e);

		protected virtual void MenuButtonClickedHandler(object sender, ClickedEventArgs e) {
			_children.GetNullable(TagResources.OptionalText)?.ForEach(x => x.SetActive(!x.activeSelf));
		}

		protected virtual void PadTouchedHandler(object sender, ClickedEventArgs e) {
			ResetTouchpadRotation();
			if(_touchpadGO != null) {
				_touchpadGO.transform.Rotate(Vector3.back, TouchpadVector.x * TouchpadGORotationMultiplier);
				_touchpadGO.transform.Rotate(Vector3.right, TouchpadVector.y * TouchpadGORotationMultiplier);
			}
		}

		protected virtual void PadUntouchedHandler(object sender, ClickedEventArgs e) {
			ResetTouchpadRotation();
		}

		protected void OnVibrateHandler(ushort durationMS) {
			SteamVRDevice.TriggerHapticPulse(durationMS, Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
		}

		private void ResetTouchpadRotation() {
			if (_touchpadGO != null) {
				_touchpadGO.transform.localRotation = _startTouchpadRotation.Value;
			}
		}

		#endregion
	}
}
