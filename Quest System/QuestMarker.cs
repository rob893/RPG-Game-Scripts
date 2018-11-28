using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMarker : MonoBehaviour {

	[SerializeField] private GameObject questAvailableMarker;
	[SerializeField] private GameObject questCompletedMarker;
	[SerializeField] private Material yellowMat;
	[SerializeField] private Material greyMat;

	private bool active = false;
	private Transform cam;


	private void Awake()
	{
		SetNoQuestMarker();
	}

	private void Start()
	{
		cam = Camera.main.transform;
	}

	private void LateUpdate()
	{
		if (active)
		{
			Vector3 targetPostition = new Vector3(cam.position.x, transform.position.y, cam.position.z);
			transform.LookAt(targetPostition);
		}
	}

	public void ToggleLootAtCamera(bool enabled)
	{
		active = enabled;
	}

	public void SetAvailableQuestMarker()
	{
		questCompletedMarker.SetActive(false);
		questAvailableMarker.SetActive(true);
		active = true;
		questAvailableMarker.GetComponent<Renderer>().material = yellowMat;
	}

	public void SetCompletedQuestMarker()
	{
		questAvailableMarker.SetActive(false);
		questCompletedMarker.SetActive(true);
		active = true;
		questCompletedMarker.GetComponent<Renderer>().material = yellowMat;
	}

	public void SetRunningQuestMarker()
	{
		questCompletedMarker.SetActive(true);
		questAvailableMarker.SetActive(false);
		active = true;
		questCompletedMarker.GetComponent<Renderer>().material = greyMat;
	}

	public void SetNoQuestMarker()
	{
		active = false;
		questCompletedMarker.SetActive(false);
		questAvailableMarker.SetActive(false);
	}
}
