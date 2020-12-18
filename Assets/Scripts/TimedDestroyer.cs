using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroyer : MonoBehaviour
{
    public float duration = 5f;
    public bool scaleDown = true;

    float timer = 0;
    bool destroyed = false;

    void Update()
    {
        if (destroyed)
            return;

        if (timer < duration)
            timer += Time.deltaTime;
        else
        {
            destroyed = true;
            if (scaleDown)
            {
                StartCoroutine(ScaleDownOverTime(duration));
            } else
            {
                Destroy(this.gameObject);
            }
        }

    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator ScaleDownOverTime(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            float interpolation = t / duration;
            float scaleX = Mathf.Lerp(transform.localScale.x, 0.01f, interpolation);
            float scaleY = Mathf.Lerp(transform.localScale.y, 0.01f, interpolation);
            float scaleZ = Mathf.Lerp(transform.localScale.z, 0.01f, interpolation);
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        scaleDown = false;
        destroyed = false;
    }
}
