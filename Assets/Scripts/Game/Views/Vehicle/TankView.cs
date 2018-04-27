using DroneDefender.Common;
using DroneDefender.Common.Extension;
using DroneDefender.Game.Ai.Controller;
using DroneDefender.Game.Constants;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Resource.Prefab;
using DroneDefender.Resource.Tag;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Views.Vehicle {
	public class TankView : View, IControllable {
		public const string ContainerName = "Tank Container";
		public const int IgnoreRaycastLayer = 2;
		// Settings
		public float WheelRotationSpeed = 10f;
		public float DieTTLS = 5f;

		// Cross Context 
		// TODO: Move these to the mediator.
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject] public EffectsQueue MainEffectsQueue { get; set; }

		// Signals
		public Signal DieSignal { get; set; }

		// IControllable
		public GameObject MovementGO { get; private set; }
		public GameObject ProjectileContainerGO { get; private set; }
		public GameObject ProjectileSpawnGO { get; private set; }
		public GameObject DisplayCanvasGO { get; private set; }
		public TextMesh HealthTextMesh { get; private set; }

		// Local Components
		private NullableResultDictionary<GameObject> _children;
		private GameObject _currentGO;
		private List<GameObject> _wheels;
		private int _startingWheelCount;
		//	Health
		private string _healthText;
		private Color _healthColor;

		#region Base View

		internal void Init() {
			// Setup current object
			_currentGO = Instantiate(Resources.Load<GameObject>(PrefabResources.Tank), transform);
			_currentGO.name = ContainerName;
			_currentGO.transform.SetPositionAndRotation(transform.position, transform.rotation);

			ConfigureChildren();

			// Death
			DieSignal = new Signal();
			DieSignal.AddListener(Die);
		}

		private void ConfigureChildren() {
			// Setup Children
			_children = this.ChildrenByTagDictionary();

			//	Wheels
			//	TODO: Implement movement enhancements based on wheels.
			_wheels = _children.GetNullable(TagResources.Wheel);
			_startingWheelCount = _wheels.Count;
			Debug.Log($"New tank with {_startingWheelCount} wheels.");

			//	Controllable
			MovementGO = _children.GetNullableFirst(TagResources.ControllableMover);
			ProjectileContainerGO = _children.GetNullableFirst(TagResources.ProjectileContainer);
			ProjectileSpawnGO = _children.GetNullableFirst(TagResources.ProjectileSpawn);
			DisplayCanvasGO = _children.GetNullableFirst(TagResources.DisplayCanvas);

			HealthTextMesh = _children.GetNullableFirst(TagResources.HealthText)?.GetComponent<TextMesh>();
		}

		private void FixedUpdate() {
			DecorationRotationHandler();
		}

		private void Update() {
			DisplayCanvasGO?.transform.LookAt(Camera.main.transform);

			if (!string.IsNullOrEmpty(_healthText)) {
				HealthTextMesh.text = _healthText;
				HealthTextMesh.color = _healthColor;
				_healthText = string.Empty;
			}
		}

		private void DecorationRotationHandler() {
			if (_wheels != null && _wheels.Count > 0) {
				foreach (var wheel in _wheels) {
					if (wheel == null || wheel.gameObject == null || wheel.transform == null) {
						_wheels.Remove(wheel);
					}
					wheel.transform.Rotate(Vector3.right * WheelRotationSpeed);
				}
			}
		}

		#endregion

		internal void Die() {
			if (!enabled) return;
			enabled = false;

			HealthTextMesh.text = "DEAD";
			HealthTextMesh.color = CommonColors.GetHealthColor(0f);

			var componentGOs = _children.SelectMany(x => x.Value).ToList();

			foreach (GameObject go in componentGOs) {
				if(go == null) { return; }
				go.layer = IgnoreRaycastLayer;

				var rb = go.GetOrAddComponent<Rigidbody>();
				if (rb == null) { return; }
				rb.isKinematic = false;
				rb.useGravity = true;
			}

			transform.parent.SendMessageUpwards(MethodNames.Die, SendMessageOptions.DontRequireReceiver);
		}

		public IControllable SetTextMesh(string text, Color color) {
			_healthText = text;
			_healthColor = color;
			return this;
		}
	}
}

