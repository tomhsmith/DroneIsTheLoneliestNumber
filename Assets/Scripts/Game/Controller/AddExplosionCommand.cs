using Assets.Scripts.Game.Controller;
using strange.extensions.command.impl;
using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Controller {
	public class AddExplosionCommand  : Command {

		// Parameters
		[Inject] public Vector3 StartVector { get; set; }
		[Inject] public Transform ParentTransform { get; set; }
		[Inject] public float DieTTLS { get; set; }
		[Inject] public Queue<ExplosionQueueItem> ExplosionQueue { get; set; }

		// Local Components
		private Object _lock = new Object();
		private int _cachedExplosionCount = 0;

		public override void Execute() {
			var nextExplode = new ExplosionQueueItem(StartVector, ParentTransform);

			if(_cachedExplosionCount > 0) {
				ExplosionQueue.Enqueue(nextExplode);
				_cachedExplosionCount++;

				ExplodeQueue();
				return;
			}

			nextExplode.Explode(true, DieTTLS);
		}

		private void ExplodeQueue() {
			lock(_lock) {
				ExplosionQueueItem next;
				while((next = ExplosionQueue.Dequeue()) != null) {
					next.Explode(true);
					_cachedExplosionCount--;
				}
			}
		}
	}
}



