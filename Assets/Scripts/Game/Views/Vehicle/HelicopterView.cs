using DroneDefender.Common;
using DroneDefender.Common.Extension;
using DroneDefender.Game.Ai.Controller;
using DroneDefender.Game.Signals.Explosion;
using DroneDefender.Game.Views.Drone;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Game.Views.Projectile;
using DroneDefender.Resource.Prefab;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Views.Vehicle {
	public class HelicopterView : View, IControllable {
		public const string ContainerName = "Helicopter Container";
		public const int IgnoreRaycastLayer = 2;
		// Settings
		public float DieTTLS = 5f;
		public float RotationSpeed = 200f;

		// Cross Context
		[Inject] public AddExplosionSignal AddExplosionSignal { get; set; }
		[Inject] public EffectsQueue MainEffectsQueue { get; set; }

		// IControllable
		public GameObject MovementGO { get; private set; }
		public GameObject ProjectileContainerGO { get; private set; }
		public GameObject ProjectileSpawnGO { get; private set; }
		public GameObject DisplayCanvasGO { get; private set; }
		public TextMesh HealthTextMesh { get; private set; }
		public ILaunchableToDestination Projectile { get; private set; }

		// Local Components
		private GameObject _currentGO;
		private NullableResultDictionary<GameObject> _children;
		private string _healthText;
		private Color _healthColor;

		#region Base View

		internal void Init() {
			_currentGO = Instantiate(Resources.Load<GameObject>(PrefabResources.Helicopter),
				transform);
			_currentGO.transform.localPosition = Vector3.zero;

			_children = this.ChildrenByTagDictionary();

			// Controllable
			MovementGO = _children.GetNullableFirst(TagResources.ControllableMover);
			ProjectileContainerGO = _children.GetNullableFirst(TagResources.ProjectileContainer);
			ProjectileSpawnGO = _children.GetNullableFirst(TagResources.ProjectileSpawn);
			DisplayCanvasGO = _children.GetNullableFirst(TagResources.DisplayCanvas);
			// Components
			Projectile = _children.GetNullableFirst(TagResources.Projectile)
				?.AddComponent<MissileView>() as ILaunchableToDestination;
			HealthTextMesh = _children.GetNullableFirst(TagResources.HealthText)
				?.GetComponent<TextMesh>();
		}

		private void FixedUpdate() {
			DecorationRotationHandler();
		}

		private void Update() {
			DisplayCanvasGO.transform.LookAt(Camera.main.transform);

			if(!string.IsNullOrEmpty(_healthText)) {
				HealthTextMesh.text = _healthText;
				HealthTextMesh.color = _healthColor;
				_healthText = string.Empty;
			}
		}

		#endregion

		#region View Helpers

		private void DecorationRotationHandler() {
			var rotateables = _children.GetNullable(TagResources.RotationDecoration);
			if (rotateables == null) { return; }

			foreach (var child in rotateables) {
				child.transform.Rotate(RotationSpeed, 0, 0, Space.Self);
			}
		}

		internal void Die() {
			if(!enabled) { return; }
			enabled = false;

			HealthTextMesh.text = "DEAD";
			HealthTextMesh.color = Constants.CommonColors.GetHealthColor(0f); ;

			if (MovementGO == null) { return; }
			AddExplosionSignal.Dispatch(
				MovementGO.transform.position, 
				MovementGO.transform, 
				DieTTLS
			);

			var componentGOs = _children.SelectMany(x => x.Value).ToList();

			foreach (GameObject go in componentGOs) {
				if (go == null) { continue; }
				go.layer = IgnoreRaycastLayer;

				var rb = go.AddComponent<Rigidbody>() ?? go.GetComponent<Rigidbody>();
				if (rb == null) { return; }
				rb.isKinematic = false;
				rb.useGravity = true;
			}
		}

		public IControllable SetTextMesh(string text, Color color) {
			_healthText = text;
			_healthColor = color;
			return this;
		}

		#endregion
	}
}
