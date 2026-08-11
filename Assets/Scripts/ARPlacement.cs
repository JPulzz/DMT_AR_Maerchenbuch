using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private Camera arCamera;

    private GameObject placedStage;
    private ARAnchor placedAnchor;

    private bool isDragging = false;
    private bool isStageLocked = false;

    private ARPlane currentDragPlane;
    private Pose lastValidDragPose;

    private static readonly List<ARRaycastHit> rayHits = new();

    private void Update()
    {
        if (raycastManager == null ||
            anchorManager == null ||
            arCamera == null)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            if (placedStage == null)
            {
                PlaceStage(touch.position);
            }
            else if (!isStageLocked && IsTouchingStage(touch.position))
            {
                StartDragging();
            }
        }

        if (touch.phase == TouchPhase.Moved && isDragging)
        {
            DragStage(touch.position);
        }

        if ((touch.phase == TouchPhase.Ended ||
             touch.phase == TouchPhase.Canceled) &&
            isDragging)
        {
            FinishDragging();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (placedStage == null)
            {
                PlaceStage(Input.mousePosition);
            }
            else if (!isStageLocked && IsTouchingStage(Input.mousePosition))
            {
                StartDragging();
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            DragStage(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            FinishDragging();
        }
    }

    private void PlaceStage(Vector2 screenPosition)
    {
        if (!TryGetPlaneHit(screenPosition, out ARRaycastHit hit, out ARPlane hitPlane))
        {
            return;
        }

        placedAnchor = anchorManager.AttachAnchor(
            hitPlane,
            hit.pose
        );

        if (placedAnchor == null)
        {
            Debug.LogWarning("Anchor could not be created.");
            return;
        }

        placedStage = Instantiate(
            stagePrefab,
            placedAnchor.transform
        );

        placedStage.transform.localPosition = Vector3.zero;
        placedStage.transform.localRotation = Quaternion.identity;
    }

    private bool TryGetPlaneHit(
        Vector2 screenPosition,
        out ARRaycastHit hit,
        out ARPlane hitPlane)
    {
        rayHits.Clear();

        bool hitDetected = raycastManager.Raycast(
            screenPosition,
            rayHits,
            TrackableType.PlaneWithinPolygon
        );

        if (!hitDetected || rayHits.Count == 0)
        {
            hit = default;
            hitPlane = null;
            return false;
        }

        hit = rayHits[0];
        hitPlane = hit.trackable as ARPlane;

        return hitPlane != null;
    }

    private bool IsTouchingStage(Vector2 screenPosition)
    {
        if (placedStage == null)
        {
            return false;
        }

        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == placedStage.transform ||
                   hit.transform.IsChildOf(placedStage.transform);
        }

        return false;
    }

    private void StartDragging()
    {
        isDragging = true;

        /*
         * Während des Drags soll die Stage nicht länger
         * vom alten Anchor-Transform abhängig sein.
         *
         * true bedeutet:
         * Die aktuelle World-Position/Rotation bleibt erhalten.
         */
        placedStage.transform.SetParent(null, true);
    }

    private void DragStage(Vector2 screenPosition)
    {
        if (!TryGetPlaneHit(
                screenPosition,
                out ARRaycastHit hit,
                out ARPlane hitPlane))
        {
            return;
        }

        placedStage.transform.position = hit.pose.position;

        /*
         * Rotation behalten wir absichtlich bei.
         * Der Nutzer verschiebt hier nur die Bühne.
         * Rotation bauen wir später separat.
         */

        currentDragPlane = hitPlane;
        lastValidDragPose = hit.pose;
    }

    private void FinishDragging()
    {
        isDragging = false;

        if (currentDragPlane == null)
        {
            // Kein gültiger neuer Treffer:
            // Stage wieder an den alten Anchor hängen.
            if (placedAnchor != null)
            {
                placedStage.transform.SetParent(
                    placedAnchor.transform,
                    true
                );
            }

            return;
        }

        ARAnchor newAnchor = anchorManager.AttachAnchor(
            currentDragPlane,
            lastValidDragPose
        );

        if (newAnchor == null)
        {
            Debug.LogWarning("New anchor could not be created.");

            if (placedAnchor != null)
            {
                placedStage.transform.SetParent(
                    placedAnchor.transform,
                    true
                );
            }

            return;
        }

        placedStage.transform.SetParent(
            newAnchor.transform,
            true
        );

        /*
         * Die Position soll exakt auf dem neuen Anchor liegen.
         * Die Rotation der Stage behalten wir dagegen,
         * damit spätere manuelle Rotation nicht verloren geht.
         */
        placedStage.transform.localPosition = Vector3.zero;

        if (placedAnchor != null)
        {
            Destroy(placedAnchor.gameObject);
        }

        placedAnchor = newAnchor;

        currentDragPlane = null;
    }

    public void LockStage()
    {
        isStageLocked = true;
    }

    public void UnlockStage()
    {
        isStageLocked = false;
    }

    public void ResetStage()
    {
        isDragging = false;
        currentDragPlane = null;

        if (placedStage != null)
        {
            Destroy(placedStage);
            placedStage = null;
        }

        if (placedAnchor != null)
        {
            Destroy(placedAnchor.gameObject);
            placedAnchor = null;
        }

        isStageLocked = false;
    }
}