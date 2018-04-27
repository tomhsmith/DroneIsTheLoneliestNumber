using DroneDefender.Common;
using DroneDefender.Common.Extension;
using DroneDefender.Game.Damage;
using DroneDefender.Game.Signals.Explosion;
using DroneDefender.Game.Views.Drone;
using DroneDefender.Resource.Prefab;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Views.Explosion {
	public class LaserView : View, ILaunchableToDestination {
		public const string ContainerName = "Laser Container";
		// Settings
		public float MaxLightRange = 7f;
		public float MaxScale = 10f;
		public float LaserPowerUpVectorTolerance = 40f;
		public float ChangeIncreaseRate = .25f;
		public float StartcWidth = 1f;
		public float EndWidth = 5f;
		public Color StartColor = Color.clear;
		public Color EndColor = Color.red;
		public float ContactPointTTLS = .25f;
		public float DPS = 20f;
		public float DestructionTTLS = 2f;

		public GameObject ContactPointGO { get { return _contactPointGO; } }
		private GameObject _contactPointGO;

		// Cross Context
		[Inject] public EffectsQueue MainEffectsQueue { get; set; }
		[Inject] public DamageSignal DamageSignal { get; set; }

		// Local Components
		private GameObject _currentGO;
		private NullableResultDictionary<GameObject> _children;
		private Rigidbody _rigidbody;
		private Material _lineMaterial;
		private LineRenderer _lineRenderer;
		private Light _glowLight;
		private Vector3 _contactPointStartingScale;
		private Vector3 _lastEndPosition;
		private float _lastUpdateAge;

		#region Base View

		internal void Init() {
			_currentGO = gameObject;
			_rigidbody = _currentGO.AddComponent<Rigidbody>();
			_rigidbody.useGravity = false;
			_rigidbody.isKinematic = true;
			_rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

			// Add Laser Point
			_contactPointGO = Instantiate(
				Resources.Load<GameObject>(PrefabResources.LaserContactPoint), transform);
			_contactPointGO.name = ContainerName;
			_contactPointStartingScale = _contactPointGO.transform.localScale;

			ConfigureChildren();
		

			ConfigureLine();
		}

		private void FixedUpdate() {
			_lastUpdateAge += Time.fixedDeltaTime;

			if (_lastUpdateAge > ContactPointTTLS) {
				SetLaserEnabled(false);
			}
		}

		private void OnTriggerEnter(Collider other) {
			var otherGO = other.gameObject;
			if (otherGO.tag == TagResources.Floor) { return; }

			Debug.LogWarning($"Laser trigger entered {other.name} - tag {other.gameObject.tag}");
			DamageSignal.Dispatch(other.gameObject, new DamageParameter(DPS, 0));
		}

		#endregion

		#region Configurations

		private void ConfigureLine() {
			_lineMaterial = new Material(Shader.Find("Particles/Additive"));

			// Add Renderer
			_lineRenderer = gameObject.AddComponent<LineRenderer>();

			// Configure Renderer.
			_lineRenderer.enabled = false;
			_lineRenderer.material = _lineMaterial;
			_lineRenderer.startWidth = StartcWidth;
			_lineRenderer.endWidth = EndWidth;
			_lineRenderer.startColor = StartColor;
			_lineRenderer.endColor = EndColor;
			_lineRenderer.gameObject.tag = TagResources.Explosion;

			// Add points to draw.
			_lineRenderer.positionCount = 2;
		}

		private void ConfigureChildren() {
			_children = this.ChildrenByTagDictionary();

			_glowLight = _children
				.GetNullableFirst(TagResources.Explosion)
				?.GetOrAddComponent<Light>();

			if (_glowLight == null) {
				Debug.LogWarning($"No light component found in {ContainerName}.");
			}
		}

		#endregion

		#region ILaunchable

		public GameObject Launch(Vector3? from, Vector3 to) {
			Vector3 laserContactMovement = _lastEndPosition - to;
			_lastEndPosition = to;

			// Reset, didn't keep in charging zone.
			if (laserContactMovement.magnitude > LaserPowerUpVectorTolerance) {
				ResetView();
				return gameObject;
			}

			// Increase light / size.
			if(_glowLight != null) {
				var lightRange = _glowLight.range + ChangeIncreaseRate;
				_glowLight.range = (lightRange < MaxLightRange) ? lightRange : MaxLightRange;
			}

			// Let's not blow up too big now..
			if (_contactPointGO.transform.localScale.x < MaxScale) {
				// Change Scale of Contact Point
				_contactPointGO.transform.localScale +=
					new Vector3(ChangeIncreaseRate, ChangeIncreaseRate, ChangeIncreaseRate);
			}

			// Draw line with start and end.
			_lineRenderer?.SetPositions(new[] { from.Value, to });
			// Bring contact prefab to end point.
			_contactPointGO.transform.position = to;

			SetLaserEnabled(true);
			return gameObject;
		}

		#endregion

		#region Helpers

		private void SetLaserEnabled(bool to) {
			if (_lineRenderer == null) { return; }

			if (to) {
				_lastUpdateAge = 0f;
			} else {
				ResetView();
			}

			_lineRenderer.enabled = to;
		}

		private void ResetView() {
			_contactPointGO.transform.localScale = _contactPointStartingScale;
			_contactPointGO.transform.position = Vector3.down * 100f;

			if(_glowLight != null) {
				_glowLight.range = 0f;
			}
		}

		#endregion
	}
}