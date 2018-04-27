using DroneDefender.Common.Extension;
using DroneDefender.Game.Constants;
using DroneDefender.Game.Views.Explosion;
using UnityEngine;

namespace Assets.Scripts.Game.Controller {
	/// <summary>
	/// Buffer item to reduce lag from explosions.
	/// </summary>
	public class ExplosionQueueItem {
		public const string ContainerNamne = "Explosion Top";
		// Settings
		public int IgnoreRaycastLayer = 2;

		internal Vector3 StartVector;
		internal Transform ParentTransform;

		internal ExplosionQueueItem(Vector3 start, Transform parent) {
			StartVector = start;
			ParentTransform = parent;
		}

		internal GameObject Explode(bool destroy, float ttls = 0) {
			var _rigidbody = ParentTransform.gameObject
				.GetOrAddComponent<Rigidbody>();

			if (_rigidbody != null) {
				_rigidbody.isKinematic = true;
			}

			var explosion = new GameObject(ContainerNamne) {
				layer = IgnoreRaycastLayer
			};
			explosion.transform.parent = ParentTransform.parent;
			explosion.transform.position = ParentTransform.position;
			explosion.transform.SetGlobaleScale(Vector3.one);

			explosion.AddComponent<SimpleExplosionView>();

			if (destroy) {
				ParentTransform.gameObject
					.SendMessageUpwards(
						MethodNames.Die,
						SendMessageOptions.DontRequireReceiver
					);
				UnityEngine.Object.Destroy(ParentTransform.gameObject, ttls);
			}

			return explosion;
		}
	}
}
