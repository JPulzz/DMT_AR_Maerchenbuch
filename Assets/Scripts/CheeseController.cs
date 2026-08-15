using System.Collections;
using UnityEngine;

public class CheeseController : MonoBehaviour
{
    [SerializeField] private Rigidbody cheeseRigidbody;
    [SerializeField] private float dropDelay = 0.7f;
    [SerializeField] private Vector3 dropVelocity = new Vector3(0f, 0.22f, -0.5f);

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        if (cheeseRigidbody == null)
            cheeseRigidbody = GetComponent<Rigidbody>();

        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        cheeseRigidbody.isKinematic = true;
    }

    public void DropCheese()
    {
        StartCoroutine(DropAfterDelay());
    }

    private IEnumerator DropAfterDelay()
    {
        yield return new WaitForSeconds(dropDelay);

        transform.SetParent(null);
        cheeseRigidbody.isKinematic = false;
        cheeseRigidbody.linearVelocity = dropVelocity;
    }

    public void ResetCheese(Transform parent)
    {
        cheeseRigidbody.isKinematic = true;

        transform.SetParent(parent);
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;

        cheeseRigidbody.linearVelocity = Vector3.zero;
        cheeseRigidbody.angularVelocity = Vector3.zero;
    }
}