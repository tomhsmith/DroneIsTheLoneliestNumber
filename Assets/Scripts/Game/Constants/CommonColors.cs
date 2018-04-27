using System.Collections.Generic;
using UnityEngine;

namespace DroneDefender.Game.Constants {
	public static class CommonColors {
		public static readonly Color DarkRed = new Color(1f, 1 / 2f, 1 / 2f);
		public static readonly Color Red = new Color(1f, 0, 0);
		public static readonly Color RedOrange = new Color(1f, 1 / 3f, 0f);
		public static readonly Color Orange = new Color(1f, 2 / 3f, 0f);
		public static readonly Color Yellow = new Color(1f, 1f, 0f);
		public static readonly Color YellowGreen = new Color(2 / 3f, 1f, 0f);
		public static readonly Color GreenYellow = new Color(1 / 3f, 1f, 0f);
		public static readonly Color Green = new Color(0f, 1f, 0f);

		public static readonly Dictionary<int, Color> HealthColors = 
			new Dictionary<int, Color>() {
				{0, DarkRed},
				{1, Red},
				{2, RedOrange },
				{3, RedOrange },
				{4, Orange },
				{5, Orange },
				{6, YellowGreen },
				{7, YellowGreen },
				{8, GreenYellow },
				{9, GreenYellow },
				{10, Green }
			};

		public static Color GetHealthColor(float percent) {
			Color healthColor;
			var health = (int)(percent * 10f);

			HealthColors.TryGetValue(health, out healthColor);
			return healthColor;
		}
	}
}
