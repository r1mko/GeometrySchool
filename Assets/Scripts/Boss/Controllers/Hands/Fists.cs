using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fists : MonoBehaviour
{
    [Serializable]
    private struct FistRef
    {
        public GameObject Fist;
        public int Index;
    }

    [SerializeField] private List<FistRef> availableFists = new List<FistRef>();
    [SerializeField] private float slamMaxY = 2.5f;
    [SerializeField] private float slamMinY = -3f;
    [SerializeField] private float raiseDuration = 0.3f;
    [SerializeField] private float slamDuration = 0.2f;

    [SerializeField] private AnimationCurve raiseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve slamCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private FistRef currentFist;
    private Coroutine currentAnimation;
    private bool hasLastFist = false;

    private void Start()
    {
        ActionBus.SpawnedFallingTrap += StartSlamAnimation;
    }

    private void OnDestroy()
    {
        ActionBus.SpawnedFallingTrap -= StartSlamAnimation;
    }

    public void StartSlamAnimation()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(SlamCoroutine());
    }

    private IEnumerator SlamCoroutine()
    {
        if (availableFists == null || availableFists.Count < 2)
        {
            Debug.LogWarning("Need at least two fists in availableFists.");
            yield break;
        }

        FistRef fistToAnimate;
        if (!hasLastFist)
        {
            fistToAnimate = availableFists[UnityEngine.Random.Range(0, availableFists.Count)];
        }
        else
        {
            var candidates = availableFists.FindAll(f => f.Fist != currentFist.Fist);
            if (candidates.Count == 0) candidates = availableFists;
            fistToAnimate = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        if (hasLastFist && currentFist.Fist != null && currentFist.Fist != fistToAnimate.Fist)
        {
            ResetFistToZero(currentFist.Fist);
        }

        if (fistToAnimate.Fist != null)
        {
            ResetFistToZero(fistToAnimate.Fist);
        }

        currentFist = fistToAnimate;
        hasLastFist = true;

        if (fistToAnimate.Fist == null) yield break;

        yield return MoveFistWithCurve(fistToAnimate.Fist, 0f, slamMaxY, raiseDuration, raiseCurve);

        yield return MoveFistWithCurve(fistToAnimate.Fist, slamMaxY, slamMinY, slamDuration, slamCurve);

        yield return MoveFistWithCurve(fistToAnimate.Fist, slamMinY, 0f, raiseDuration * 0.5f, raiseCurve);
    }

    private void ResetFistToZero(GameObject fist)
    {
        if (fist == null) return;
        Vector3 pos = fist.transform.position;
        fist.transform.position = new Vector3(pos.x, 0f, pos.z);
    }

    private IEnumerator MoveFistWithCurve(GameObject fist, float fromY, float toY, float duration, AnimationCurve curve)
    {
        if (fist == null || duration <= 0f) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float y = Mathf.Lerp(fromY, toY, curve.Evaluate(t));

            Vector3 pos = fist.transform.position;
            fist.transform.position = new Vector3(pos.x, y, pos.z);

            yield return null;
        }

        Vector3 finalPos = fist.transform.position;
        fist.transform.position = new Vector3(finalPos.x, toY, finalPos.z);
    }
}