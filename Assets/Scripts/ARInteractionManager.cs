using UnityEngine;
using UnityEngine.EventSystems;

public class ARInteractionManager : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private float ravenTriggerDistance = 0.6f;
    [SerializeField] private StoryManager storyManager;

    private RavenAnimationController ravenAnimation;
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
            RavenAnimationController raven =
                hit.collider.GetComponentInParent<RavenAnimationController>();

            if (raven != null &&
             storyManager.CurrentState == StoryManager.StoryState.PlayerDecision)
            {
                raven.PlayDropCheese();
                storyManager.ChooseDropCheese();
            }
        }
    }
    private void CheckRavenDistance()
    {
        if (ravenAnimation == null)
        {
            ravenAnimation = FindAnyObjectByType<RavenAnimationController>();

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
                ravenAnimation.PlayKeepCheese();
                storyManager.ChooseKeepCheese();
            }
        }
        else if (distance > ravenTriggerDistance)
        {
            ravenInRange = false;
        }
    }
}