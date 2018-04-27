using DroneDefender.Common;
using DroneDefender.Common.Extension;
using DroneDefender.Game.Constants;
using DroneDefender.Game.Signals;
using DroneDefender.Game.Signals.Generic;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Resource.Prefab;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Inputs {
	public class FPSCameraRigView : View {
		// Settings
		public float ForwardSpeed = 6f;
		public float StraffSpeed = 4f;
		public float HorizontalLookSpeed = 1f;
		public float HorizontalRotationLimit = 0f;
		public float VerticalSpeed = 1f;
		public float VerticalLookSpeed = 1f;
		public float VerticalRotationLimit = 90f;
		public float ScrollMovementSpeed = 50f;
		public float SiteMovementThreshold = .01f;

		// Cross Context
		[Inject] public EffectsQueue EffectsQueue { get; set; }

		internal Camera MainCamera => _camera;
		internal MoveSignal MoveSignal { get; private set; }
		internal ControllerSignal ControllerSignal { get; private set; }

		// Local Components
		private GameObject _currentGO;
		private NullableResultDictionary<GameObject> _children;
		private Camera _camera;
		private GameObject _site;
		private Rigidbody _rigidbody;
		private Vector3 _currentRotation;
		private Vector3 _initialSitePosition;
		private Point _initialMousePosition;
		private bool _releaseMouse;

		#region Base View

		public void Init() {
			_currentGO = Instantiate(
				Resources.Load<GameObject>(PrefabResources.FPSCameraRig),
					transform);

			_rigidbody = _currentGO.GetComponent<Rigidbody>();
			_currentRotation = Vector3.zero;

			// Chilren
			_children = this.ChildrenByTagDictionary();
			_camera = _children.GetNullableFirst(TagResources.MainCamera)?.GetComponent<Camera>();
			_site = _children.GetNullableFirst(TagResources.Site);
			_initialSitePosition = _site.transform.localPosition;

			// Signals
			MoveSignal = new MoveSignal();
			MoveSignal.AddListener(Move);

			ControllerSignal = new ControllerSignal();

			// Setup
			ConfigureMouse();
			ResetLookPoint();
		}

		private void Update() {
			MouseClickHandler();
			MouseMovementHandler();
			ArrowKeysHandler();
			ReleaseMouseHandler();
		}

		private void FixedUpdate() {
			MovementHandler();

			EffectsQueue.ExecuteNextEffectChunk();
		}

		#endregion

		#region Handlers

		private void ReleaseMouseHandler() {
			if(!Input.GetKeyDown(KeyCode.Tab)
				|| !Debug.isDebugBuild) { return; }

			_releaseMouse = !_releaseMouse;
			Cursor.visible = _releaseMouse;
		}

		private void ArrowKeysHandler() {
			if (!Input.anyKey) { return; }

			// TODO: Move to settings.
			var upPressed = Input.GetKey(KeyCode.UpArrow);
			var leftPressed = Input.GetKey(KeyCode.LeftArrow);
			var backPressed = Input.GetKey(KeyCode.DownArrow);
			var rightPressed = Input.GetKey(KeyCode.RightArrow);

			if (upPressed) {
				ControllerSignal.Dispatch(InputContexts.SECONDARY_FORWARD_PRESSED);
			}

			if (leftPressed) {
				ControllerSignal.Dispatch(InputContexts.SECONDARY_LEFT_PRESSED);
			}

			if (backPressed) {
				ControllerSignal.Dispatch(InputContexts.SECONDARY_BACK_PRESSED);
			}

			if (rightPressed) {
				ControllerSignal.Dispatch(InputContexts.SECONDARY_RIGHT_PRESSED);
			}
		}

		private void MouseMovementHandler() {
			var mouseMovement = new Vector2(
					Input.GetAxis(InputAxisNames.MouseX),
					Input.GetAxis(InputAxisNames.MouseY)
				);

			if (mouseMovement.magnitude != 0) {
				LookHandler(mouseMovement);
			}
		}

		private void MouseClickHandler() {
			// TODO: Move to settings.
			var mouse0 = Input.GetKey(KeyCode.Mouse0);
			var mouse1 = Input.GetKeyDown(KeyCode.Mouse1);

			if (mouse0) {
				ControllerSignal.Dispatch(InputContexts.MOUSE_0_PRESSED);
			}

			if (mouse1) {
				ControllerSignal.Dispatch(InputContexts.MOUSE_1_PRESSED);
			}
		}

		private void LookHandler(Vector2 look) {
			var nextXRotation = _currentRotation.x + look.x;
			var nextYRotation = _currentRotation.y + look.y;

			// Check if looks limits set and hit if not rotate.
			//	Horizontal
			if (HorizontalRotationLimit == 0
				|| (nextXRotation > -HorizontalRotationLimit
					&& nextXRotation < HorizontalRotationLimit)) {
				_rigidbody.transform.Rotate(Vector3.up, look.x, Space.World);
				_currentRotation.x = nextXRotation;
			}
			//	Vertical
			if (VerticalRotationLimit == 0
				|| (nextYRotation > -VerticalRotationLimit
					&& nextYRotation < VerticalRotationLimit)) {
				_rigidbody.transform.Rotate(Vector3.left, look.y, Space.Self);
				_currentRotation.y = nextYRotation;
			}

			// Check if past threshold.
			if ((_site.transform.localPosition - _initialSitePosition)
				.magnitude < SiteMovementThreshold) { return; }

			ResetLookPoint();
			ResetMouse();
		}

		private void MovementHandler() {
			var scroll = Input.GetAxis(InputAxisNames.MouseScrollWheel);
			if (scroll == 0 && !Input.anyKey) { return; }

			// TODO: Move to settings.
			var wPressed = Input.GetKey(KeyCode.W);
			var aPressed = Input.GetKey(KeyCode.A);
			var sPressed = Input.GetKey(KeyCode.S);
			var dPressed = Input.GetKey(KeyCode.D);

			Vector3 move = Vector3.zero;
			var look = _site.transform.position - _rigidbody.transform.position;
			
			// Allows for multiple inputs to sum a movement.
			if(wPressed || scroll < 0) {
				Move(Vector3.forward);
			}

			if (aPressed) {
				Move(Vector3.left);
			}

			if (sPressed || scroll > 0) {
				Move(Vector3.back);
			}

			if (dPressed) {
				Move(Vector3.right);
			}

			Move(move);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Moves the camera in relation to the direction pointing.
		/// </summary>
		/// <param name="move">Adjustment in look from current rotation.</param>
		internal void Move(Vector3 move) {
			var moveToVectorForward = _camera.transform.forward * move.z * ForwardSpeed;
			var moveToVectorRight = _camera.transform.right * move.x * StraffSpeed;
			var moveToVertorUp = _camera.transform.up * move.y * VerticalSpeed;

			move = moveToVectorForward + moveToVertorUp + moveToVectorRight;
			move += _camera.transform.position;

			_rigidbody.transform.position = move;
			ResetLookPoint();
		}

		private void ResetLookPoint() {
			_camera.transform.LookAt(_site.transform);
		}

		private void ConfigureMouse() {
			// Set first mouse position.
			MouseController.GetCursorPos(out _initialMousePosition);

			Cursor.visible = false;
		}

		private void ResetMouse() {
			// Lets build in an escape route..
			if(_releaseMouse) { return; }

			MouseController.SetCursorPos(_initialMousePosition.x, _initialMousePosition.y);
		}

		#endregion
	}
}
