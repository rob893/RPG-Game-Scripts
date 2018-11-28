using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

	public Transform target;
	public Vector3 offset;
	public float pitch = 2f;
	public float zoomSpeed = 4f;
	public float minZoom = 5f;
	public float maxZoom = 15f;
	public float yawSpeed = 100f;

	private float currentZoom = 8f;
	private float currentYaw = -90f;

	private void Start()
	{
		if(target == null)
		{
			target = PlayerManager.Instance.player.transform;
		}
	}

	private void Update()
	{
		if(Input.GetAxis("Mouse ScrollWheel") != 0 && !EventSystem.current.IsPointerOverGameObject())
		{
			currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
			currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
		}
		
		currentYaw -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
	}

	private void LateUpdate()
	{
		transform.position = target.position - offset * currentZoom;
		transform.LookAt(target.position + Vector3.up * pitch);

		transform.RotateAround(target.position, Vector3.up, currentYaw);
	}
}
