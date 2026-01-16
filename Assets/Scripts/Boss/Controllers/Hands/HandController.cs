using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HandController : MonoBehaviour
{
    enum Hands
    {
        Palm,
        Snap
    }

    [Header("Common")]
    private Hands currentHand; // to do

    [Header("HandTypes")]
    [SerializeField] private GameObject palm;
    [SerializeField] private GameObject snap;

    [Header("Palm")]
    [SerializeField] private float targetSpeed;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float returnDelay;
    private Vector3 palmStartLocalPosition;
    private Coroutine palmCurrentRoutine;

    [Header("Snap")]
    [SerializeField] private Sprite fistSprite;
    [SerializeField] private Sprite fingerSprite;
    [SerializeField] private float snapSpeed;
    private SpriteRenderer snapView;
    private Coroutine snapCurrentRoutine;

    private void Start()
    {
        palmStartLocalPosition = palm.transform.localPosition;
        snapView = snap.GetComponent<SpriteRenderer>();
        ActionBus.SpawnedStaticTrap += OnSpawnStaticTrap;
        ActionBus.SpawnedColumnTrap += OnSpawnColumnTrap;
    }

    private void OnDestroy()
    {
        ActionBus.SpawnedStaticTrap -= OnSpawnStaticTrap;
        ActionBus.SpawnedColumnTrap -= OnSpawnColumnTrap;
    }

    // Static Trap
    private void OnSpawnStaticTrap(Vector3 targetWorldPosition)
    {
        if (palmCurrentRoutine != null)
        {
            StopCoroutine(palmCurrentRoutine);
        }

        palmCurrentRoutine = StartCoroutine(PalmMoveRoutine(targetWorldPosition));
    }

    private IEnumerator PalmMoveRoutine(Vector3 targetWorldPosition)
    {
        yield return MoveToWorldTarget(targetWorldPosition);

        yield return new WaitForSeconds(returnDelay);

        yield return MoveToLocalBack(palmStartLocalPosition);

        palmCurrentRoutine = null;
    }

    private IEnumerator MoveToWorldTarget(Vector3 worldTarget)
    {
        palm.SetActive(true);
        Vector3 startPos = palm.transform.position;
        float distance = Vector3.Distance(startPos, worldTarget);
        float duration = distance / targetSpeed;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            palm.transform.position = Vector3.Lerp(startPos, worldTarget, t);
            yield return null;
        }

        palm.transform.position = worldTarget;
    }

    private IEnumerator MoveToLocalBack(Vector3 localTarget)
    {
        Vector3 startLocal = palm.transform.localPosition;
        float distance = Vector3.Distance(palm.transform.TransformPoint(startLocal), palm.transform.parent.TransformPoint(localTarget));
        float duration = distance / returnSpeed;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            palm.transform.localPosition = Vector3.Lerp(startLocal, localTarget, t);
            yield return null;
        }

        palm.transform.localPosition = localTarget;
        palm.SetActive(true);
    }

    // Column Trap
    private void OnSpawnColumnTrap()
    {
        if (snapCurrentRoutine != null)
        {
            StopCoroutine(snapCurrentRoutine);
        }

        snapCurrentRoutine = StartCoroutine(SnapAnimationRoutine());
    }

    private IEnumerator SnapAnimationRoutine()
    {
        snap.SetActive(true);
        snapView.sprite = fistSprite;
        yield return new WaitForSeconds(snapSpeed);

        snapView.sprite = fingerSprite;
        yield return new WaitForSeconds(snapSpeed);

        snap.SetActive(false);
    }
}