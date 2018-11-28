using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	public delegate void MapOpen();
	public event MapOpen OnMapOpen;

	public delegate void MapClose();
	public event MapClose OnMapClose;

	public float zoomSpeed = 75f;
	public float minZoom = 100f;
	public float maxZoom = 400f;
	public float camMovementSpeed = 100;
	public GameObject mapCloseButton;

	private GameObject mainCam;
	private Transform playerTransform;
	private Vector3 offset = new Vector3(0, 1, 0);
	private float currentZoom = 300;
	private UIManager uiManager;
	private float leftRightMovement;
	private float upDownMovement;

	private void Start()
	{
		mainCam = Camera.main.gameObject;
		playerTransform = PlayerManager.Instance.transform;
		uiManager = UIManager.Instance;

		mapCloseButton.SetActive(false);
		gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
			currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
		}

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			leftRightMovement -= camMovementSpeed * Time.unscaledDeltaTime;
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			leftRightMovement += camMovementSpeed * Time.unscaledDeltaTime;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			upDownMovement -= camMovementSpeed * Time.unscaledDeltaTime;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			upDownMovement += camMovementSpeed * Time.unscaledDeltaTime;
		}
		if (Input.GetKey(KeyCode.H))
		{
			leftRightMovement = 0;
			upDownMovement = 0;
			currentZoom = 300;
		}
	}

	private void LateUpdate()
	{
		offset = new Vector3(leftRightMovement, currentZoom, upDownMovement);
		transform.position = playerTransform.position + offset;
	}

	public void ToggleMap()
	{
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
			uiManager.PlayOpenMenuSound();
			uiManager.ToggleUIElements(false);
			leftRightMovement = 0;
			upDownMovement = 0;
			currentZoom = 300;

			if (Terrain.activeTerrain != null)
			{
				Terrain.activeTerrain.drawTreesAndFoliage = false;
			}
			
			RenderSettings.fog = false;
			mainCam.SetActive(false);
			Time.timeScale = 0;
			mapCloseButton.SetActive(true);
			transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + currentZoom, playerTransform.position.z);

			if(OnMapOpen != null)
			{
				OnMapOpen.Invoke();
			}
		}
		else
		{
			uiManager.ToggleUIElements(true);
			Time.timeScale = 1;

			if (Terrain.activeTerrain != null)
			{
				Terrain.activeTerrain.drawTreesAndFoliage = true;
			}

			RenderSettings.fog = true;
			mainCam.SetActive(true);
			uiManager.PlayCloseMenuSound();
			gameObject.SetActive(false);
			mapCloseButton.SetActive(false);

			if (OnMapClose != null)
			{
				OnMapClose.Invoke();
			}
		}
	}
}
