using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to translate the current GameObject using events
	public class LeanManualTranslate : MonoBehaviour
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
		public Vector3 Direction = Vector3.forward;

		[Tooltip("Fixed multiplier of each translation")]
		public float Multiplier = 1.0f;

		public virtual void Translate(float delta)
		{
			if (Scale == ScaleType.DeltaTime)
			{
				delta *= Time.deltaTime;
			}

			transform.Translate(Direction * delta * Multiplier, Space);
		}
	}
}