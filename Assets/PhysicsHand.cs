using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PhysicsHand : MonoBehaviour
{
	public InputDeviceCharacteristics controllerCharacteristics;

	private InputDevice targetDevice;
	private Animator handAnimator;
	public ActionBasedController followObject;
	[SerializeField] float followSpeed = 30f;
	[SerializeField] float rotationSpeed = 100f;
	Transform followTarget;
	[SerializeField] Vector3 positionOffset;
	[SerializeField] Vector3 rotationOffset;
	private Rigidbody rb;
	SkinnedMeshRenderer mesh;
	Collider[] colliders;


	// Start is called before the first frame update
	void Start()
	{
		TryInitialize();
		mesh = GetComponentInChildren<SkinnedMeshRenderer>();
		colliders = GetComponentsInChildren<Collider>();

		followTarget = followObject.transform;
		rb = GetComponent<Rigidbody>();
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		rb.maxAngularVelocity = 20f;

		rb.position = followTarget.position;
		rb.rotation = followTarget.rotation;
		
	}

	void TryInitialize()
	{
		List<InputDevice> devices = new List<InputDevice>();

		InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);


		if (devices.Count > 0)
		{
			targetDevice = devices[0];
			handAnimator = GetComponent<Animator>();
		}
	}

	void PhysicsMove()
	{
		Vector3 positionWithOffset = followTarget.TransformPoint(positionOffset);
		float distance = Vector3.Distance(positionWithOffset, transform.position);
		rb.velocity = Vector3.Normalize(positionWithOffset - transform.position) * followSpeed * Time.deltaTime * distance;

		Quaternion rotationWithOffset = followTarget.rotation * Quaternion.Euler(rotationOffset);
		Quaternion rotation = rotationWithOffset * Quaternion.Inverse(rb.rotation);
		rotation.ToAngleAxis(out float angle, out Vector3 axis);
		if (Mathf.Abs(axis.magnitude) != Mathf.Infinity)
        {
            if (angle > 180.0f) { angle -= 360.0f; }
            rb.angularVelocity = axis * angle * Mathf.Deg2Rad * rotationSpeed * Time.deltaTime;
        }

	}

	void UpdateHandAnimation()
	{
		if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
		{
			handAnimator.SetFloat("Trigger", triggerValue);
			
		}
		else
		{
			handAnimator.SetFloat("Trigger", 0);
		}
		if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
		{
			handAnimator.SetFloat("Grip", gripValue);
		}
		else
		{
			handAnimator.SetFloat("Grip", 0);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!targetDevice.isValid)
		{
			TryInitialize();
		}
		else
		{
			UpdateHandAnimation();
			
			
		}
		PhysicsMove();
	}

	public void StartHolding()
	{
		mesh.enabled = false;
		foreach(Collider collider in colliders)
		{
			collider.enabled = false;
		}
	}

	public void StopHolding()
	{
		mesh.enabled = true;
		foreach(Collider collider in colliders)
		{
			collider.enabled = true;
		}
	}
}
