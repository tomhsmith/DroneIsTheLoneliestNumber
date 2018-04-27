using DroneDefender.Game.Constants.Prefab;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Views.Explosion {
	public class SimpleExplosionView : View {
		public const string ContainerName = "Explosion Container";
		// Settings
		public int MaxItterations = 5;
		public float DestructionDelay = .3f; 

		// Cross Context
		[Inject(InjectableTypes.Explosion)]
		public GameObject ExplosionPrefab { get; set; }
		
		[Inject(AudioContexts.ExplosionRobotic)]
		public AudioClip ExplosionAudioClip { get; set; }

		// Local Components
		private GameObject _currentGO;
		private AudioSource _audioSource;
		private int _itterationCount = 0;

		internal void Init() {
			var explosionPrefab = Instantiate(ExplosionPrefab, transform);
			explosionPrefab.transform.localPosition = Vector3.zero;

			_currentGO = gameObject;
			_currentGO.name = ContainerName;

			var rb = _currentGO.AddComponent<Rigidbody>();
			rb.useGravity = false;
			rb.isKinematic = true;
			rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

			_audioSource = _currentGO.AddComponent<AudioSource>();
			_audioSource.PlayOneShot(ExplosionAudioClip);
		}

		private void FixedUpdate() {
			// Explosion Finished
			if(_itterationCount > MaxItterations) {
				Destroy(_currentGO, DestructionDelay);
				return;
			}

			_itterationCount++;

			// Scale the Explosion
			var scale = _currentGO.transform.localScale.x + _itterationCount;
			_currentGO.transform.localScale = new Vector3(scale, scale, scale);
		}
	}
}
