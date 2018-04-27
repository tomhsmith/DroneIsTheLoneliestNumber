using UnityEngine;

namespace DroneDefender.Game.Ai.Controller {
	public interface IControllable {
		GameObject MovementGO { get; }
		GameObject ProjectileContainerGO { get; }
		GameObject ProjectileSpawnGO { get;  }
		TextMesh HealthTextMesh { get; }
		IControllable SetTextMesh(string text, Color color);
	}
}
