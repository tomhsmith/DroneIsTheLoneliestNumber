using DroneDefender.Game.Constants.Prefab;
using DroneDefender.Game.Views.Explosion;
using DroneDefender.Resource.Prefab;
using DroneDefender.Resource.Tag;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Views.Drone {
	public class DroneMothershipView : View {
		public const string ContainerName = "Drone Mothership Container";
		// Settings
		public float DecorationRotationSpeed = 20f;
		public float AudioVolume = .25f;
		public float Thrust = 10f;
		
		// Cross Context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject(AudioContexts.LaserBeam)]
		public AudioClip LaserBeamSound { get; set; }

		// Local Components
		private GameObject _currentGo;
		private Rigidbody _rigidbody;
		private Transform _spawnTransform;
		private List<Transform> _rotationDecorations;

		// Laser
		private GameObject _laser;
		private LaserView _laserView;
		private AudioSource _laserBeamAudioSource;
		private bool _laserIsLasering; 

		#region Base View

		public void Init() {
			_currentGo = Instantiate(
				Resources.Load<GameObject>(PrefabResources.DroneMothership), 
				transform);
			_currentGo.name = ContainerName;

			_rigidbody = _currentGo.GetComponent<Rigidbody>();

			ConfigureChildren();
			ConfigureLaser();
		}

		private void FixedUpdate() {

			SetAudio(_laserIsLasering);

			if(_rotationDecorations != null) {
				foreach(Transform decoration in _rotationDecorations) {
					decoration.Rotate(Vector3.up, DecorationRotationSpeed);
					decoration.Rotate(Vector3.right, DecorationRotationSpeed);
					decoration.Rotate(Vector3.forward, DecorationRotationSpeed);
				}	
			}

			_laserIsLasering = false;
		}

		#endregion

		#region Configuration

		private void ConfigureChildren() {
			_rotationDecorations = new List<Transform>();

			foreach(Transform child in _currentGo.transform) {
				if(child.gameObject.tag == TagResources.RotationDecoration) {
					_rotationDecorations.Add(child);
				}
				if(child.gameObject.tag == TagResources.ProjectileSpawn) {
					// TODO: Get this working correctly.
				}
			}
			_spawnTransform = _currentGo.transform;
		}

		private void ConfigureLaser() {
			var containerName = "Laser Top";

			// Setup new object.
			_laser = new GameObject(containerName);
			_laser.transform.parent = transform;

			_laserView = _laser.AddComponent<LaserView>();

			_laserBeamAudioSource = _laserView.ContactPointGO.AddComponent<AudioSource>();
			_laserBeamAudioSource.clip = LaserBeamSound;
			_laserBeamAudioSource.volume = AudioVolume;
			_laserBeamAudioSource.playOnAwake = true;
		} 

		#endregion

		#region Drone Features

		internal void Move(Vector3 vector) {
			var cameraPosition = Camera.main.transform.position;

			// Move up, down
			if(vector.z < 0f) {
				_rigidbody.transform.LookAt(cameraPosition);
				_rigidbody.AddRelativeForce(Vector3.forward * Thrust, ForceMode.Impulse);
			} else if(vector.z > 0f) {
				_rigidbody.transform.LookAt(
					new Vector3(
						cameraPosition.x,
						_rigidbody.transform.position.y,
						cameraPosition.z
					)
				);
				_rigidbody.AddRelativeForce(Vector3.back * Thrust, ForceMode.Impulse);
			}
			// Move right, left
			if (vector.x < 0f) {
				_rigidbody.AddRelativeForce(Vector3.right * Thrust, ForceMode.Impulse);
			} else if(vector.x > 0f) {
				_rigidbody.AddRelativeForce(Vector3.left * Thrust, ForceMode.Impulse);
			}
		}

		internal void LaunchDrones(List<Vector3> targets) {
			var containerName = "Drone Top";

			foreach(var target in targets) {
				var droneGO = new GameObject(containerName);
				droneGO.transform.parent = transform;
				droneGO.transform.localPosition = Vector3.zero;

				var drone = droneGO.AddComponent<DroneAirBasicView>()
					as ILaunchableToDestination;
				drone.Launch(_spawnTransform.position, target);
			}
		}

		internal void FireLaser(Vector3 to) {
			_laserView.Launch(_spawnTransform.position, to);
			_laserIsLasering = true;
		}

		#endregion

		#region Helpers

		private void SetAudio(bool on) {
			if(on) {
				if(_laserBeamAudioSource.isPlaying) { return; }
				_laserBeamAudioSource.Play();
			} else { 
				_laserBeamAudioSource.Pause();
			}
		}

		#endregion
	}
}

