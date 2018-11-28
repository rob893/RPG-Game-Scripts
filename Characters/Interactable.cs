using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour {

	public delegate void OnInteract();
	public event OnInteract OnInteracted;

	public float radius = 3f;
	public Transform interactionTransform;

	[SerializeField] private string objectName = "Placeholder";

	private UIManager uiManager;


	protected virtual void Start()
	{
		uiManager = UIManager.Instance;
	}

	public virtual void Interact(Transform interacter)
	{
		if(OnInteracted != null)
		{
			OnInteracted();
		}
	}

	private void FaceTarget(Transform target)
	{
		if (target == null)
		{
			return;
		}

		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
	}

	protected IEnumerator FaceTargetCoroutine(Transform target)
	{
		float timer = 0;
		while (timer < 2)
		{
			timer += Time.deltaTime;
			FaceTarget(target);
			yield return new WaitForEndOfFrame();
		}
	}

	protected virtual void OnMouseEnter()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			uiManager.ShowFixedToolTip(objectName);
		}
	}

	protected virtual void OnMouseExit()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			uiManager.HideToolTip();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if(interactionTransform == null)
		{
			interactionTransform = transform;
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(interactionTransform.position, radius);
	}
}
