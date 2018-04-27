using DroneDefender.Game.Ai.Controller;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Model {
	public class PlayerModel : IPlayer {
		public int Id { get { return _id; } }
		private int _id = 0;

		public int TeamId { get { return _teamId; } }
		private int _teamId = 0;

		public List<IAiController> Spawns { get; private set; }

		public PlayerModel(int teamId = 1) {
			Spawns = new List<IAiController>();

			_id = Random.Range(1, 100);
			_teamId = teamId;
		}
	}
}
