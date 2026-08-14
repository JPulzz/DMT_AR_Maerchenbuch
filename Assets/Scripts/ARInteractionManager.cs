using UnityEngine;
using UnityEngine.EventSystems;

public class ARInteractionManager : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private float ravenTriggerDistance = 0.6f;
    [SerializeField] private StoryManager storyManager;

    private RavenAnimationTest ravenAnimation;
    private bool ravenInRange = false;

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

        CheckRavenDistance();
    }

    private void TryInteract(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            RavenAnimationTest raven =
                hit.collider.GetComponentInParent<RavenAnimationTest>();

            if (raven != null &&
                storyManager.CurrentState == StoryManager.StoryState.PlayerDecision)
            {
                raven.TriggerReaction();
                storyManager.ChooseDropCheese();
            }
        }
    }
    private void CheckRavenDistance()
    {
        if (ravenAnimation == null)
        {
            ravenAnimation = FindFirstObjectByType<RavenAnimationTest>();

            if (ravenAnimation == null)
            {
                return;
            }
        }

        float distance = Vector3.Distance(
            arCamera.transform.position,
            ravenAnimation.transform.position
        );

        if (distance <= ravenTriggerDistance && !ravenInRange)
        {
            if (storyManager.CurrentState == StoryManager.StoryState.PlayerDecision)
            {
                ravenInRange = true;
                ravenAnimation.TriggerReaction();
                storyManager.ChooseKeepCheese();
            }
        }
        else if (distance > ravenTriggerDistance)
        {
            ravenInRange = false;
        }
    }
}