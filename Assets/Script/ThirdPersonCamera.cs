
using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
	[SerializeField]
	private Transform target = null;
	[SerializeField]
	private float distance = 10.0f;
	[SerializeField]
	private float height = 5.0f;
	[SerializeField]
	private float Rotation = 0;
	[SerializeField]
	private float rotationDamping = 0;
	[SerializeField]
//	private float heightDamping = 10;
//	private Rigidbody rigid;

	public float rotate = 10;

	int mFrameRate = 60;
	int cameraNO = 0;

	void Start ()
	{ 
		QualitySettings.vSyncCount = 0;
		mFrameRate = 60;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
//		rigid = target.GetComponent<Rigidbody> ();
		reset ();
	}

	public void reset ()
	{
		M.isParked = false;
		cameraNO = -1;
		setCamera ();
		Rotation = 0;
	}

	void LateUpdate ()
	{
		if (!target)
			return;
		var wantedHeight = target.position.y + height;
		rotationDamping = 10;   

		var currentRotationAngle = Mathf.LerpAngle (transform.eulerAngles.y, Rotation + target.eulerAngles.y, (rotationDamping) * Time.deltaTime);
		var currentHeight = Mathf.Lerp (transform.position.y, wantedHeight, 10 * Time.deltaTime);
		var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		transform.LookAt (target);
		Vector3 local = transform.localPosition;   
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
		transform.rotation = Quaternion.Euler (rotate, currentRotationAngle, 0);
	}

	void Update ()
	{
		if (Application.targetFrameRate != mFrameRate) {
			Application.targetFrameRate = mFrameRate;
		}
	}

	public void setCamera ()
	{
		cameraNO++;
		if (!M.isParked) {
			Rotation = 0;
			switch (cameraNO % 3) {
			case 0:
				distance = 10.0f;
				height = 15.0f;

				rotate = 40;
				break;
			case 1:
				distance = 3.0f;
				height = 6.0f;

				rotate = 10;
				break;
			case 2:
				distance = 3.0f;
				height = 10.0f;
				rotate = 10;
				break;
			}
		} else {
			height = 10.0f;
			distance = 13.0f;
			Rotation += 30; 
			rotate = 10;
		}
	}

}
