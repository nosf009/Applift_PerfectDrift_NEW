using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanManualTranslate2D to be smooth
	public class LeanManualTranslate2DSmooth : LeanManualTranslate2D
	{
		[Tooltip("How quickly the position goes to the target value")]
		public float Dampening = 10.0f;

		[System.NonSerialized]
		private Vector3 remainingDelta;

		public override void Translate(Vector2 delta)
		{
			// Translate and increment by delta
			var oldPosition = transform.localPosition;

			base.Translate(delta);

			remainingDelta += transform.localPosition - oldPosition;

			// Revert
			transform.localPosition = oldPosition;
		}

		protected virtual void Update()
		{
			var factor   = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			transform.localPosition += remainingDelta - newDelta;

			remainingDelta = newDelta;
		}
	}
}