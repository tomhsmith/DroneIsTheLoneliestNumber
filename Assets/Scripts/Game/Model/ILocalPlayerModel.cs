using DroneDefender.Game.Signals.Ai;
using DroneDefender.Game.Signals.Generic;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Model {
	public interface ILocalPlayerModel {
		List<Vector3> GetTargetVectors();
		float PlayerTargetTTLS { get; }

		// Exposed here for raycast.
		Camera MainCamera { get; set; }

		// Last success
		Vector3? LastRaycastHitPoint { get; set; }

		// Signals
		MoveSignal MoveSignal { get; }
		RotateSignal RotateSignal { get; }
		SecondaryFireSignal LaunchDroneSignal { get; }
		PrimaryFireSignal FireLaserSignal { get; }
		//	Command Signal
		AddPlayerTargetSignal AddTargetSignal { get; }
	}
}

