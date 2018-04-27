using DroneDefender.Common;
using DroneDefender.Common.Extension;
using DroneDefender.Resource.Tag;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System;
using UnityEngine;

namespace DroneDefender.Game.Views.Projectile {
	public class GunView : View {
		// Settings
		public string ContainerName = "Gun Top";
		public float ReloadTTLS = 4f;

		// Cross context
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject ContextView { get; set; }

		public Signal FireSignal { get; private set; } = new Signal();

		// Local Components
		private GameObject _currentGO;
		private NullableResultDictionary<GameObject> _children;

		private GameObject _bulletGO;
		private Type _bulletType;
		private GameObject _spawnPoint;

		private float _reloadTTLS;

		public void Init() {
			_currentGO = new GameObject(ContainerName);
			_currentGO.transform.parent = transform;
			_currentGO.transform.localPosition = Vector3.zero;
		}

		public GunView ConfigureGun(string prefabPath) {
			var gunGO = Instantiate(Resources.Load<GameObject>(prefabPath), _currentGO.transform);
			gunGO.transform.localPosition = Vector3.zero;

			_children = this.ChildrenByTagDictionary();
			_spawnPoint = _children.GetNullableFirst(TagResources.ProjectileSpawn);

			if(_spawnPoint == null) {
				Debug.LogError($"Spawn point not found for:{Environment.NewLine}{prefabPath}");
			}

			FireSignal.AddListener(FireHandler);

			return this;
		}

		public GunView ConfigureBullet<T>(string prefabPath) 
			where T: BulletView {
			_bulletGO = Instantiate(Resources.Load<GameObject>(prefabPath));

			_bulletGO.AddComponent<T>();
			_bulletType = typeof(T);

			return this;
		}

		private void FireHandler() {
			var bullet = Instantiate(_bulletGO, ContextView.transform)
				.GetComponent(_bulletType) as ILaunchableFromPositionRotation;

			if(bullet == null) {
				Debug.LogError($"{_bulletGO.name} is not of type {bullet.GetType().Name}");
			}

			bullet.Launch(_spawnPoint.transform.position, _spawnPoint.transform.rotation);
		}
	}
}
