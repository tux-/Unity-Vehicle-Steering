using UnityEngine;

namespace Vehicle
{
	[RequireComponent(typeof(WheelCollider))]

	public class Wheel : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Can this wheel steer.")]
		public bool steer = false;

		private WheelCollider wheelCollider;
		private MeshRenderer meshRenderer;

		void Awake ()
		{
			wheelCollider = GetComponent<WheelCollider>();
			meshRenderer = GetComponentInChildren<MeshRenderer>();
		}

		public void SteerAngle (float angle)
		{
			if (this.steer) {
				wheelCollider.steerAngle = angle;
			}
		}

		void Update ()
		{
			wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
			meshRenderer.transform.position = pos;
			meshRenderer.transform.rotation = rot;
		}
	}
}
