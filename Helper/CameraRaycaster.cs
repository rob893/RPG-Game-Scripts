using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRaycaster : MonoBehaviour
{
	[SerializeField] Texture2D defaultCursor = null;
	[SerializeField] Texture2D enemyCursor = null;
	[SerializeField] Texture2D interactableCursor = null;
	[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

	const int POTENTIALLY_WALKABLE_LAYER = 9;
	const int ATTACKABLE_LAYER = 10;
	float maxRaycastDepth = 200f; // Hard coded value

	Rect currentScrenRect;

	public delegate void OnMouseOverEnemy(CharacterStats enemy);
	public event OnMouseOverEnemy onMouseOverEnemy;

	public delegate void OnMouseOverTerrain(Vector3 destination);
	public event OnMouseOverTerrain onMouseOverPotentiallyWalkable;

	public delegate void OnMouseOverInteractable(Interactable interactable);
	public event OnMouseOverInteractable onMouseOverInteractable;

	private Transform prevHitAndChanged;
	private Transform prevObjectHit;
	private Transform playerTransform;
	private short currentCursorIndex = 0; //default cursor = 1, attack cursor = 2, interact cursor = 3


	private void Start()
	{
		playerTransform = PlayerManager.Instance.player.transform;
		SetDefaultMouseCursor();
	}

	private void Update()
	{
		currentScrenRect = new Rect(0, 0, Screen.width, Screen.height);

		// Check if pointer is over an interactable UI element
		if (EventSystem.current.IsPointerOverGameObject())
		{
			SetDefaultMouseCursor();
		}
		else
		{
			PerformRaycasts();
		}
	}

	private void PerformRaycasts()
	{
		if (currentScrenRect.Contains(Input.mousePosition))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Specify layer priorities below, order matters
			//MakeTransparent();
			if (RaycastForEnemy(ray)) { return; }
			if (RaycastForInteractable(ray)) { return; }
			if (RaycastForPotentiallyWalkable(ray)) { return; }
		}
	}

	private void MakeTransparent()
	{
		//float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position) - 0.5f;
		
		Ray ray = new Ray(transform.position, (playerTransform.position - transform.position).normalized);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20, 1 << 13))
		{
			Transform objectHit = hit.transform;

			if(objectHit != prevObjectHit)
			{
				prevObjectHit = objectHit;
				
				if (objectHit.GetComponentInParent<MakeTransparent>() != null)
				{
					objectHit = objectHit.GetComponentInParent<MakeTransparent>().transform;

					if (objectHit != prevHitAndChanged)
					{
						
						if (prevHitAndChanged != null)
						{
							prevHitAndChanged.GetComponent<MakeTransparent>().RevertObjectToOriginalMaterial();
						}
						prevHitAndChanged = objectHit;
						prevHitAndChanged.GetComponent<MakeTransparent>().MakeObjectTransparent();
					}
				}
			}
		}
		else if (prevHitAndChanged != null)
		{
			prevHitAndChanged.GetComponent<MakeTransparent>().RevertObjectToOriginalMaterial();
			prevHitAndChanged = null;
			prevObjectHit = null;
		}
	}

	private bool RaycastForEnemy(Ray ray)
	{
		RaycastHit hitInfo;
		LayerMask attackableLayer = 1 << ATTACKABLE_LAYER;
		if (Physics.Raycast(ray, out hitInfo, maxRaycastDepth, attackableLayer))
		{
			var gameObjectHit = hitInfo.collider.gameObject;
			var enemyHit = gameObjectHit.GetComponent<CharacterStats>();
			if (enemyHit)
			{
				SetAttackableMouseCursor();

				onMouseOverEnemy(enemyHit);
				return true;
			}
		}
		
		return false;
	}

	private bool RaycastForInteractable(Ray ray)
	{
		RaycastHit hitInfo;
		if(Physics.Raycast(ray, out hitInfo, maxRaycastDepth))
		{
			var gameObjectHit = hitInfo.collider.gameObject;
			var interactableHit = gameObjectHit.GetComponent<Interactable>();
			if (interactableHit)
			{
				SetInteractableMouseCursor();

				onMouseOverInteractable(interactableHit);
				return true;
			}
		}
		return false;
	}

	private bool RaycastForPotentiallyWalkable(Ray ray)
	{
		SetDefaultMouseCursor();

		if (!Input.GetMouseButton(0))
		{
			return false;
		}

		RaycastHit hitInfo;
		LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
		bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, 1500f, potentiallyWalkableLayer);
		if (potentiallyWalkableHit)
		{
			onMouseOverPotentiallyWalkable(hitInfo.point);
			return true;
		}
		return false;
	}

	public void SetDefaultMouseCursor()
	{
		if (currentCursorIndex != 1)
		{
			currentCursorIndex = 1;
			Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
		}
	}

	public void SetAttackableMouseCursor()
	{
		if (currentCursorIndex != 2)
		{
			currentCursorIndex = 2;
			Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
		}
	}

	public void SetInteractableMouseCursor()
	{
		if (currentCursorIndex != 3)
		{
			currentCursorIndex = 3;
			Cursor.SetCursor(interactableCursor, cursorHotspot, CursorMode.Auto);
		}
	}

	private void OnDisable()
	{
		SetDefaultMouseCursor();
	}
}