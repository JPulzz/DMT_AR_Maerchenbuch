using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceCube : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject stagePrefab;

    private GameObject placedStage;

    private static readonly List<ARRaycastHit> rayHits = new();

    private void Update()
    {
        if (raycastManager == null)
        {
            return;
        }

        // Es existiert bereits eine Bühne.
        if (placedStage != null)
        {
            return;
        }

        // Touch-Eingabe auf einem Smartphone oder Tablet
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);

            // Verhindert Platzierung beim Drücken eines UI-Buttons.
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            PlaceStage(touch.position);
        }
        // Maus-Eingabe zum Testen im Editor
        else if (Input.GetMouseButtonDown(0))
        {
            // Verhindert Platzierung beim Anklicken eines UI-Buttons.
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            PlaceStage(Input.mousePosition);
        }
    }

    private void PlaceStage(Vector2 screenPosition)
    {
        rayHits.Clear();

        bool hitDetected = raycastManager.Raycast(
            screenPosition,
            rayHits,
            TrackableType.PlaneWithinPolygon
        );

        if (!hitDetected || rayHits.Count == 0)
        {
            return;
        }

        Pose hitPose = rayHits[0].pose;

        placedStage = Instantiate(
            stagePrefab,
            hitPose.position,
            hitPose.rotation
        );
    }

    public void ResetStage()
    {
        if (placedStage == null)
        {
            return;
        }

        Destroy(placedStage);
        placedStage = null;
    }
}