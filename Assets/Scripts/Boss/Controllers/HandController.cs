using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour
{
    [Header("Static Trap Movement Settings")]
    [SerializeField] private float targetSpeed;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float returnDelay;

    private Vector3 startLocalPosition;
    private Coroutine currentRoutine;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
        ActionBus.SpawnStaticTrap += OnSpawnStaticTrap;
    }

    private void OnDestroy()
    {
        ActionBus.SpawnStaticTrap -= OnSpawnStaticTrap;
    }

    private void OnSpawnStaticTrap(Vector3 targetWorldPosition)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(MoveToAndReturn(targetWorldPosition));
    }

    private IEnumerator MoveToAndReturn(Vector3 targetWorldPosition)
    {
        yield return MoveToWorldTarget(targetWorldPosition);

        yield return new WaitForSeconds(returnDelay);

        yield return MoveToLocalBack(startLocalPosition);

        currentRoutine = null;
    }

    private IEnumerator MoveToWorldTarget(Vector3 worldTarget)
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, worldTarget);
        float duration = distance / targetSpeed;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(startPos, worldTarget, t);
            yield return null;
        }

        transform.position = worldTarget;
    }

    private IEnumerator MoveToLocalBack(Vector3 localTarget)
    {
        Vector3 startLocal = transform.localPosition;
        float distance = Vector3.Distance(transform.TransformPoint(startLocal), transform.parent.TransformPoint(localTarget));
        float duration = distance / returnSpeed;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(startLocal, localTarget, t);
            yield return null;
        }

        transform.localPosition = localTarget;
    }
}