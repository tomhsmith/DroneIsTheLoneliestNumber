using DroneDefender.Game.Model;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Signals.Ai {
	public class AddSpawnSignal : Signal<IPlayer, SpawnTypes, Vector3, List<GameObject>> { }
}