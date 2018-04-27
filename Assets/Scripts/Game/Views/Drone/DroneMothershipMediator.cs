using DroneDefender.Common.Extension;
using DroneDefender.Game.Constants.Prefab;
using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace DroneDefender.Game.Views.Drone {
	public class DroneMothershipMediator : Mediator {
		// Cross Context
		[Inject] public DroneMothershipView View { get; set; }
		[Inject] public DroneMothershipModel Mothership { get; set; }

		[Inject(PlayerContexts.LocalPlayer)]
		public ILocalPlayerModel LocalPlayer { get; set; }

		[Inject(AudioContexts.IntroMustProtect)]
		public AudioClip IntroMustProtectAudioClip { get; set; }

		// Local Components
		private AudioSource _audioSource;

		public override void OnRegister() {
			base.OnRegister();

			// Handlers
			//	Mothership
			Mothership.MoveSignal.AddListener(MoveHandler);
			//	Local Player
			LocalPlayer.LaunchDroneSignal.AddListener(LaunchDronesHandler);
			LocalPlayer.FireLaserSignal.AddListener(FireLaserHandler);

			_audioSource = gameObject.GetOrAddComponent<AudioSource>();
			_audioSource.PlayOneShot(IntroMustProtectAudioClip);

			View.Init();
		}

		private void MoveHandler(Vector3 vector) {
			View.Move(vector);
		}

		private void LaunchDronesHandler(bool updateTargets) {
			if (LocalPlayer.LastRaycastHitPoint == null) { return; }

			View.LaunchDrones(LocalPlayer.GetTargetVectors());
		}

		private void FireLaserHandler(bool updateRaycast) {
			if(LocalPlayer.LastRaycastHitPoint == null) { return; }

			View.FireLaser(LocalPlayer.LastRaycastHitPoint.Value);
		}
	}
}
