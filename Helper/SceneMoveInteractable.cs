using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveInteractable : Interactable  {

	[SerializeField] private int sceneToLoadIndex;
	[SerializeField] private bool manuallySetNextSceneLocation = false;
	[SerializeField] private Vector3 nextSceneSpawnLocation;
	[SerializeField] private AudioClip moveSceneSound;

	public override void Interact(Transform interacter)
	{
		base.Interact(interacter);
		AudioManager.Instance.PlaySoundEffect(moveSceneSound, true);
		StartCoroutine(MoveScene());
	}

	private IEnumerator MoveScene()
	{
		yield return new WaitForSeconds(0.5f);

		GameManager.Instance.SaveDataBetweenScenes();

		if (manuallySetNextSceneLocation)
		{
			GameManager.Instance.SetNextSceneLocation(sceneToLoadIndex, nextSceneSpawnLocation);
		}

		UIManager.Instance.LoadLevel(sceneToLoadIndex);
	}
}
