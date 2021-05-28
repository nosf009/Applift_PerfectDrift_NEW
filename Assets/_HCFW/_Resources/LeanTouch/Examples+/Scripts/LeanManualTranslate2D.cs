using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to translate the current GameObject using events
	public class LeanManualTranslate2D : MonoBehaviour
	{
		public enum ScaleType
		{
			None,
			DeltaTime
		}

		[Tooltip("The translation multiplier")]
		public ScaleType Scale;

		[Tooltip("The translation space")]
		public Space Space = Space.World;
		
		[Tooltip("The first translation direction")]
		public Vector3 DirectionA = Vector3.right;
		
		[Tooltip("The second translation direction")]
		public Vector3 DirectionB = Vector3.up;

		[Tooltip("Fixed multiplier of each translation")]
		public float Multiplier = 1.0f;

		public virtual void Translate(Vector2 delta)
		{
			if (Scale == ScaleType.DeltaTime)
			{
				delta *= Time.deltaTime;
			}

			transform.Translate(DirectionA * delta.x * Multiplier, Space);
			transform.Translate(DirectionB * delta.y * Multiplier, Space);
		}
	}
}