using DroneDefender.Game.Ai.Controller;
using System.Collections.Generic;

namespace DroneDefender.Game.Model {
	public interface IPlayer {
		int Id { get; }
		int TeamId { get; }
		List<IAiController> Spawns { get; }
	}
}
