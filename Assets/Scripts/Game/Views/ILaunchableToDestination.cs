using UnityEngine;

namespace DroneDefender.Game.Views.Drone {
	public interface ILaunchableToDestination {
		GameObject Launch(Vector3? from, Vector3 target);
	}
}

