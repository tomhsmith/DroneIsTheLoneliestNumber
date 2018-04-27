using DroneDefender.Game.Model;
using DroneDefender.Game.Signals;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Game.Views.UI {
	public class IntroCanvasView : View {
		// Settings
		public float FPSVectorMultiplier = 12f;

		#region Intro Canvas

		//	Vectors
		public Vector3 TextScale = new Vector3(.001f, 0.001f, .001f);
		public Vector3 ScreenBackwardsScale = new Vector3(0f, 1f, 2f);
		public Vector3 PlayerMoveScale = new Vector3(0f, .1f, .01f);
		public Vector3 BodyTextMoveScale = (Vector3.back * 50f);
		//	Prefab Component Names
		public const string IntroTextLeadName = "Intro Text Lead";
		public const string IntroTextOneName = "Intro Text One";
		public const string IntroTextBodyName = "Intro Text Body";
		public const string IntroTextCreditName = "Intro Text Credit";
		public const string IntroCanvasPlane = "Intro Canvas Plane";
		public const string IntroSkyscraper = "Skyscraper Prefab";
		public const string IntroAudioSource = "Intro Audio Source";
		public const string DroneText = "DRONE";
		//	Intro Action Timings
		private static class Timings {
			internal const float First = 2f;
			internal const float Second = 4f;
			internal const float Third = 6.5f;
			internal const float Fourth = 8f;
			internal const float Fifth = 11f;
			internal const float Sixth = 18f;
			internal const float Seventh = 40f;
		}

		#endregion

		#region Cross Context

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		[Inject(PlayerContexts.LocalPlayer)]
		public ILocalPlayerModel LocalPlayer { get; set; }

		[Inject] public DroneMothershipModel Mothership { get; set; }
		[Inject] public SettingsModel Settings { get; set; }

		#endregion

		#region Local Components

		// Canvas
		private Transform _leadText;
		private Transform _oneText;
		private Transform _bodyText;
		private Transform _backgroundPlane;
		private Transform _building;
		private Transform _credit;
		// Audio
		private Transform _audioSourceTransform;
		private AudioSource _mainAudioSource;
		private AudioSource _loopAudioSource;
		// Timing
		private float _lastDroneFired = 0f;
		private float _elapsed = 0f;
		private bool _backgroundDestroyed = false;
		private bool _oneTextDestroyed = false;
		private bool _hasStarted = false;

		#endregion

		#region Base View

		internal void Init() {
			// Intro Canvas Components
			_leadText = gameObject.transform.Find(IntroTextLeadName);
			_oneText = gameObject.transform.Find(IntroTextOneName);
			_bodyText = gameObject.transform.Find(IntroTextBodyName);
			_credit = gameObject.transform.Find(IntroTextCreditName);
			_backgroundPlane = gameObject.transform.Find(IntroCanvasPlane);
			_building = gameObject.transform.Find(IntroSkyscraper);
			
			//Audio
			_audioSourceTransform = gameObject.transform.Find(IntroAudioSource);
			var audioSources = _audioSourceTransform.GetComponents<AudioSource>();
			_mainAudioSource = audioSources.FirstOrDefault();
			_loopAudioSource = audioSources.Skip(1).FirstOrDefault();
			_loopAudioSource?.PlayScheduled(AudioSettings.dspTime + _mainAudioSource.clip.length);

			// Mode Specific
			if(Settings.GameMode == GameModes.FPS) {
				PlayerMoveScale *= FPSVectorMultiplier;
			}
		}

		private void FixedUpdate() {
			if(!_hasStarted) { return; }

			TutorialHandler();
		}

		#endregion

		#region Handlers

		private void TutorialHandler() {
			_lastDroneFired += Time.fixedDeltaTime;
			_elapsed += Time.fixedDeltaTime;

			// Randomize drone fire.
			var randomHeight = Random.Range(50f, 500f);
			LocalPlayer.LastRaycastHitPoint = _building.position + new Vector3(75f, randomHeight, 50f);

			// Fire and sometimes move mother.
			if (_lastDroneFired > Timings.First) {
				_lastDroneFired = 0f;
				LocalPlayer.LaunchDroneSignal.Dispatch(false);
			} else if (_lastDroneFired < .3f) {
				Mothership.MoveSignal.Dispatch(Vector3.forward);
			}
			// Timed Events
			if (!_oneTextDestroyed && _elapsed > Timings.Second) {
				_oneText.GetComponent<MeshRenderer>().enabled = true;
			}
			if (_elapsed > Timings.Third) {
				_bodyText.GetComponent<MeshRenderer>().enabled = true;
			}
			if (_elapsed > Timings.Fourth) {
				_leadText.GetComponent<MeshRenderer>().enabled = true;
				_leadText.localScale += TextScale;
			}
			if (_elapsed > Timings.Fifth) {
				_leadText.GetComponent<TextMesh>().text = DroneText;

				if (!_oneTextDestroyed) {
					_oneTextDestroyed = true;
					Destroy(_oneText.gameObject);
				}

				if (!_backgroundDestroyed && _elapsed > Timings.Fifth + 3f) {
					_backgroundDestroyed = true;
					_bodyText.localPosition += BodyTextMoveScale;
					Destroy(_backgroundPlane.gameObject);
				}
			}
			if (_elapsed > Timings.Sixth) {
				// Move screen backwards.
				transform.localPosition += ScreenBackwardsScale;
				// Increase body text.
				_bodyText.localScale += TextScale;
				_credit.localScale += TextScale;
				// Move player up
				LocalPlayer.MoveSignal.Dispatch(PlayerMoveScale);
			}
			if (_elapsed > Timings.Seventh) {
				enabled = false;
			}
		}

		internal IntroCanvasView Play() {
			_hasStarted = true;
			return this;
		}

		#endregion
	}
}
