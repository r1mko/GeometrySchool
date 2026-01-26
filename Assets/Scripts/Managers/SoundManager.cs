using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundAudio;

    public void PlayBackgroundCurrentMusic()
    {
        backgroundAudio.Play();
    }
}
