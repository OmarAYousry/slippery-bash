using System.Collections;
using UnityEngine;

public class TileDestruction: MonoBehaviour
{
    [SerializeField] private AudioSource audioSource = null;

    Coroutine coroutine;

    public void SetupCollider()
    {
        coroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<TileController>())
        {
            if(coroutine == null)
                coroutine = StartCoroutine(PlaySound());
            other.GetComponentInParent<TileController>().DamageTile(0, true, 1, false);
        }
    }

    IEnumerator PlaySound()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        AudioController.PlaySoundEffect(SoundEffectType.TITANIC_CRASH, audioSource);
        coroutine = null;
    }
}
