using DroneDefender.Common;
using DroneDefender.Common.Extension;
using DroneDefender.Game.Damage;
using DroneDefender.Game.Signals.Generic;
using DroneDefender.Game.Views;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Resource.Prefab;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Inputs {
	public class VRCameraRigView : View {
		// Settings
		public float CameraSpeed = 15f;
		public float RotateSensitivity = 16f;
		public float HUDCanvasBackgroundAlpha = .4f;

		// Cross Context
		[Inject] public EffectsQueue EffectsQueue { get; set; }

		[Inject(InjectableTypes.TeamContainer)]
		public NullableResultDictionary<GameObject> TeamContainer { get; set; }

		// Needed for raycasting.
		internal Transform MainCamera => _mainCamera;
		private Transform _mainCamera;

		// Local Signals
		internal MoveSignal MoveSignal { get; private set; }
		internal RotateSignal RotateSignal { get; private set; }

		// Local Components
		private GameObject _currentGo;
		private NullableResultDictionary<GameObject> _children;
		private TextMesh _scoreTextMesh;
		private Rigidbody _rigidbody;
		private Transform _controller0;
		private Transform _controller1;
		private float _startingScore;
		private float _currentScore;

		#region Base View 

		public void Init() {
			_currentGo = Instantiate(
				Resources.Load<GameObject>(PrefabResources.CameraRig), 
					transform);

			ConfigureChildren();
			ConfigureSignals();
			UpdateScore();
		}

		private void FixedUpdate() {
			EffectsQueue.ExecuteNextEffectChunk();

			UpdateScore();
		}

		private void ConfigureSignals() {
			MoveSignal = new MoveSignal();
			MoveSignal.AddListener(MoveHandler);

			RotateSignal = new RotateSignal();
			RotateSignal.AddListener(RotateHandler);
		}

		private void ConfigureChildren() {
			_rigidbody = _currentGo.GetOrAddComponent<Rigidbody>();

			_children = this.ChildrenByTagDictionary();

			_controller0 = _children.GetNullableFirst(TagResources.Controller0)?.transform;
			_controller1 = _children.GetNullableFirst(TagResources.Controller1)?.transform;
			_mainCamera = _children.GetNullableFirst(TagResources.MainCamera)?.transform;
			_scoreTextMesh = _children.GetNullableFirst(TagResources.HUD)?.GetComponent<TextMesh>();

			_controller0?.gameObject?.AddComponent<VRInputLeftView>();
			_controller1?.gameObject?.AddComponent<VRInputRightView>();
		}

		private void UpdateScore() {
			var newScore = 
				TeamContainer.GetNullable(TagResources.Team00)
					?.Sum(x => x.GetComponent<TeamAssignerView>()?.Health ?? 0f) ?? 0f;

			// Fire fire.
			if(_currentScore == 0) {
				_startingScore = _currentScore = newScore;
				return;
			}

			// Score increased..
			if (newScore > _currentScore) {
				// Lets disabled this for now.
				Debug.LogError("Health increased.");
				return;
			}

			_currentScore = newScore;
			UpdateScoreUI();
		}

		private void UpdateScoreUI() {
			var integrityPercent = (_startingScore > 0f) 
										? _currentScore / _startingScore
										: 0f;

			// Update text.
			_scoreTextMesh.text =
				$"City Integrity{System.Environment.NewLine}" +
				$"{(integrityPercent * 100).ToString("##.00")}%";

			var backgroundColor = Constants.CommonColors.GetHealthColor(integrityPercent);

			// Get background material and set color.
			var backgroundRenderer = _scoreTextMesh.transform.GetChild(0).GetComponent<Renderer>();
			backgroundRenderer.material.SetTransparent(HUDCanvasBackgroundAlpha, backgroundColor);
		}

		#endregion

		#region Handlers

		internal void MoveHandler(Vector3 moveTo) {
			var moveToVectorForward = _mainCamera.transform.forward * moveTo.z;
			var moveToVectorRight = _mainCamera.transform.right * moveTo.x;
			var moveToVertorUp = _mainCamera.transform.up * moveTo.y;

			moveTo = CameraSpeed * (moveToVectorForward + moveToVertorUp + moveToVectorRight);
			moveTo += _rigidbody.transform.position;

			_rigidbody.MovePosition(moveTo);
		}

		private void RotateHandler(Vector3 rotateTo) {
			_rigidbody.transform.Rotate(Vector3.up, rotateTo.y);
		}

		#endregion
	}
}