using UnityEngine;
using UnityEngine.EventSystems;

public class ARInteractionManager : MonoBehaviour
{
    [SerializeField] private Camera arCamera;

    private void Update()
    {
        if (arCamera == null)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                TryInteract(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            TryInteract(Input.mousePosition);
        }
    }

    private void TryInteract(Vector2 screenPosition)
    {
        Debug.Log("Interaction input detected");

        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);

            RavenAnimationTest raven =
                hit.collider.GetComponentInParent<RavenAnimationTest>();

            if (raven != null)
            {
                Debug.Log("Raven found - trigger animation");
                raven.TriggerReaction();
            }
        }
        else
        {
            Debug.Log("Physics.Raycast hit nothing");
        }
    }
}