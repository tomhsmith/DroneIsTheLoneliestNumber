using strange.extensions.mediation.impl;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneDefender.Common.Extension {
	public static class UnityExtensions {
		#region Nullable Result Dictionary

		/// <summary>
		/// Assembles all children GOs -> gc, ggc, gggc, into sorted Dictionary,.
		/// </summary>
		/// <param name="view">View to find children.</param>
		/// <returns>All children up to gggc sorted by tag.</returns>
		public static NullableResultDictionary<GameObject> ChildrenByTagDictionary(this View view) {
			var childrenSorted = new NullableResultDictionary<GameObject>();

			Insert(childrenSorted, view.transform);

			return childrenSorted;
		}

		private static void Insert(NullableResultDictionary<GameObject> children, Transform transform) {
			foreach(Transform child in transform) {
				Insert(children, child.gameObject);
				Insert(children, child);
			}
		}

		private static void Insert(NullableResultDictionary<GameObject> children, GameObject go) {
			children.Add(go);
		}

		#endregion

		#region Transform

		public static NullableResultDictionary<GameObject> ChildrenByTagDictionary(this Transform transform) {
			var childrenSorted = new NullableResultDictionary<GameObject>();

			Insert(childrenSorted, transform);

			return childrenSorted;
		}

		public static Transform SetGlobaleScale(this Transform transform, Vector3 globalScale) {
			transform.localScale = Vector3.one;
			transform.localScale = new Vector3(
					globalScale.x / transform.lossyScale.x,
					globalScale.y / transform.lossyScale.y,
					globalScale.z / transform.lossyScale.z
				);

			return transform;
		}

		public static Transform DestroyChildren(this Transform transform, List<string> excludeTags = null) {
			foreach (Transform trans in transform) {
				if(excludeTags?.Any() ?? false
					&& excludeTags.Contains(trans.gameObject.tag)) {
					continue;
				}
				GameObject.Destroy(trans.gameObject);
			}
			return transform;
		}

		public static Vector3 Masking(this Vector3 vector, VectorMask mask) {
			return new Vector3(
				(mask.HasFlag(VectorMask.x)) ? 0 : vector.x,
				(mask.HasFlag(VectorMask.y)) ? 0 : vector.x,
				(mask.HasFlag(VectorMask.z)) ? 0 : vector.z
			);
		}

		public static Vector4 Masking(this Vector4 vector, VectorMask mask) {
			return new Vector4(
				(mask.HasFlag(VectorMask.x)) ? 0 : vector.x,
				(mask.HasFlag(VectorMask.y)) ? 0 : vector.x,
				(mask.HasFlag(VectorMask.z)) ? 0 : vector.z,
				(mask.HasFlag(VectorMask.w)) ? 0 : vector.w
			);
		}

		[System.Flags]
		public enum VectorMask {
			x = 0,
			y = 1,
			z = 2,
			w = 4
		}

		#endregion

		#region GameObject

		public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
			var component = go.GetComponent<T>();
			
			if(component == null) {
				component = go.AddComponent<T>();
			}

			return component;
		}

		#endregion

		#region Material

		public static Material SetTransparent(this Material material, float alpha, Color? backgroundColor = null) {
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			material.SetInt("_ZWrite", 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;

			var newColor = backgroundColor ?? material.color;
			newColor.a = alpha;
			material.color = newColor;

			return material;
		}

		#endregion

		#region Camera

		private static Dictionary<Camera, Vector2> CameraDimensionCache 
			= new Dictionary<Camera, Vector2>();

		public static Vector2 GetCameraDimensions(this Camera camera) {
			Vector2 dimensions;

			CameraDimensionCache.TryGetValue(camera, out dimensions);

			// Found it.
			if(dimensions != default(Vector2)) { return dimensions; }

			// Calculate and add.
			var height = 2f * camera.orthographicSize;
			var newDimensions = new Vector2(height, height * camera.aspect);
			CameraDimensionCache.Add(camera, newDimensions);

			return newDimensions;
		}

		#endregion
	}
}
