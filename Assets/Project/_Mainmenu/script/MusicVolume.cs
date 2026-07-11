using UnityEngine;

public class MusicVolume : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSource.volume = volume;
    }

    public void SetVolume(float value)
    {
        // إذا السلايدر من 0 إلى 100
        float volume = value / 100f;

        musicSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}