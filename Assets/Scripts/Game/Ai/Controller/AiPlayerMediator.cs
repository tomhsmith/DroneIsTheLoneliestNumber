using DroneDefender.Common;
using DroneDefender.Game.Ai;
using DroneDefender.Game.Model;
using DroneDefender.Game.Signals.Ai;
using DroneDefender.Game.Views;
using DroneDefender.Resource.Tag;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiPlayerMediator : Mediator {
	// Settings
	public int MaxSpawn = 70;
	public int SquadSize = 5;
	public int HelicoptersPerSquad = 2;
	public float SpawnRefresh = 16f;

	// Cross Context
	[Inject(InjectableTypes.TeamContainer)]
	public NullableResultDictionary<GameObject> TeamContainer { get; set; }

	[Inject] public AiPlayerView View { get; set; }
	[Inject] public AddSpawnSignal AddSpawnSignal { get; set; }

	//	Local Components
	private IPlayer _player;
	private List<GameObject> _spawnPoints;
	private List<GameObject> _unusedSpawns = new List<GameObject>();
	private List<GameObject> _waypoints;
	private float _spawnCountdown;

	public override void OnRegister() {
		base.OnRegister();

		// Setup Model
		_player = new PlayerModel();

		UpdateSpawnpoints();
		UpdateWaypoints();

		View.Init();
	}

	private void FixedUpdate() {
		_spawnCountdown -= Time.fixedDeltaTime;

		SpawnHandler();
	}

	private void SpawnHandler() {
		if (_spawnCountdown > 0 || _player.Spawns.Count > MaxSpawn) { return; }

		// Spawn a squad.
		for (var i = 0; i < SquadSize; i++) {
			var spawnTransform = NextSpawnPoint().transform;
			var spawnScaleVector = spawnTransform.localScale;

			var randomSpawnVector = spawnTransform.position;
			randomSpawnVector.x += Random.Range(-spawnScaleVector.x / 2f, spawnScaleVector.x / 2f);
			randomSpawnVector.y += Random.Range(-spawnScaleVector.y / 2f, spawnScaleVector.y / 2f);
			randomSpawnVector.z += Random.Range(-spawnScaleVector.z / 2f, spawnScaleVector.z / 2f);

			AddSpawnSignal.Dispatch(_player, SpawnTypes.TANK, randomSpawnVector, _waypoints);

			// Don't forget the heli.
			if (i < HelicoptersPerSquad) {
				AddSpawnSignal.Dispatch(_player, SpawnTypes.HELICOPTER, randomSpawnVector, _waypoints);
			}
		}

		_spawnCountdown = SpawnRefresh;
	}

	private GameObject NextSpawnPoint() {
		// Exhausted spawns, refresh.
		if(_unusedSpawns.Count < 1) { _unusedSpawns = _spawnPoints.ToList(); }

		// Pick one out of unused bucket.
		var nextSpawn = _unusedSpawns[Random.Range(0, _unusedSpawns.Count - 1)];
		// It's now used.
		_unusedSpawns.Remove(nextSpawn);

		return nextSpawn;
	}

	private void UpdateSpawnpoints() {
		_spawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag(TagResources.Team01Spawn));

		if (!_spawnPoints.Any()) {
			Debug.LogError($"No spawn points found for {GetType().Name}.");
		}
	}

	private void UpdateWaypoints() {
		// Waypoints are optional.
		_waypoints = (new List<GameObject>(GameObject.FindGameObjectsWithTag(TagResources.Waypoint)))
			.OrderBy(x => x.GetComponent<WaypointBehaviour>().Number).ToList();
	}
}
