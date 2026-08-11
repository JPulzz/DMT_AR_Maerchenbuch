using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ARPlacement : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private Camera arCamera;
    [SerializeField] private TMP_Text lockButtonText;
    [SerializeField] private ARPlaneManager planeManager;

    [SerializeField] private Slider rotationSlider;
    [SerializeField] private Slider scaleSlider;

    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;

    [SerializeField] private float minimumPlaneWidth = 0.6f;
    [SerializeField] private float minimumPlaneDepth = 0.6f;

    private GameObject placedStage;
    private ARAnchor placedAnchor;

    private bool isDragging = false;
    private bool isStageLocked = false;

    private ARPlane currentDragPlane;
    private Pose lastValidDragPose;

    private Vector3 baseStageScale;
    private float baseStageWidth;
    private float baseStageDepth;

    private ARPlane placementPlane;

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

        if (!IsPlaneLargeEnough(hitPlane))
        {
            Debug.Log("Detected plane is too small for the stage.");
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

        placementPlane = hitPlane;

        placedStage = Instantiate(
            stagePrefab,
            placedAnchor.transform
        );

        placedStage.transform.localPosition = Vector3.zero;
        placedStage.transform.localRotation = Quaternion.identity;

        baseStageScale = placedStage.transform.localScale;

        Bounds stageBounds = GetStageBounds(placedStage);

        baseStageWidth = stageBounds.size.x;
        baseStageDepth = stageBounds.size.z;

        Debug.Log(
            $"Base Stage Size - Width: {baseStageWidth}, Depth: {baseStageDepth}"
        );

        float calculatedMaxScale = CalculateMaxScale(hitPlane);

        maxScale = calculatedMaxScale;

        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.SetValueWithoutNotify(1f);
        }
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

        if (hitPlane != placementPlane)
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

    public void ToggleStageLock()
    {
        if (placedStage == null)
        {
            return;
        }

        isStageLocked = !isStageLocked;

        SetPlaneDetectionActive(!isStageLocked);

        if (rotationSlider != null)
        {
            rotationSlider.interactable = !isStageLocked;
        }

        if (scaleSlider != null)
        {
            scaleSlider.interactable = !isStageLocked;
        }

        if (lockButtonText != null)
        {
            lockButtonText.text = isStageLocked
                ? "Unlock Stage"
                : "Lock Stage";
        }
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

        if (lockButtonText != null)
        {
            lockButtonText.text = "Lock Stage";
        }

        if (rotationSlider != null)
        {
            rotationSlider.SetValueWithoutNotify(0f);
        }

        if (scaleSlider != null)
        {
            scaleSlider.SetValueWithoutNotify(1f);
        }

        if (rotationSlider != null)
        {
            rotationSlider.interactable = true;
        }

        if (scaleSlider != null)
        {
            scaleSlider.interactable = true;
        }

        placementPlane = null;

        SetPlaneDetectionActive(true);
    }

    public void SetStageRotation(float yRotation)
    {
        if (placedStage == null || isStageLocked)
        {
            return;
        }

        placedStage.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }


    public void SetStageScale(float scale)
    {
        if (placedStage == null || isStageLocked)
        {
            return;
        }

        float clampedScale = Mathf.Clamp(scale, minScale, maxScale);

        placedStage.transform.localScale = baseStageScale * clampedScale;
    }

    private bool IsPlaneLargeEnough(ARPlane plane)
    {
        return plane.size.x >= minimumPlaneWidth &&
               plane.size.y >= minimumPlaneDepth;
    }

    private Bounds GetStageBounds(GameObject stage)
    {
        Renderer[] renderers = stage.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            return new Bounds(stage.transform.position, Vector3.zero);
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    private float CalculateMaxScale(ARPlane plane)
    {
        float usablePlaneWidth = plane.size.x * 0.85f;
        float usablePlaneDepth = plane.size.y * 0.85f;

        float maxScaleFromWidth = usablePlaneWidth / baseStageWidth;
        float maxScaleFromDepth = usablePlaneDepth / baseStageDepth;

        return Mathf.Min(maxScaleFromWidth, maxScaleFromDepth);
    }

    private void SetPlaneDetectionActive(bool isActive)
    {
        if (planeManager == null)
        {
            return;
        }

        planeManager.enabled = isActive;

        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(isActive);
        }
    }
}