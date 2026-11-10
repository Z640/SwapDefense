using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip mainBgmClip;
    [SerializeField] private AudioClip fireSfxClip;

    [Header("Volume Settings")]
    [SerializeField] private float bgmVolume = 0.5f;
    [SerializeField] private float fireSfxVolume = 0.7f;

    public AudioSource BgmAudioSource => bgmAudioSource;
    public AudioSource SfxAudioSource => sfxAudioSource;
    public AudioClip MainBgmClip => mainBgmClip;
    public AudioClip FireSfxClip => fireSfxClip;

    public void SetupReferences(
        AudioSource bgmSource,
        AudioSource sfxSource,
        AudioClip bgmClip,
        AudioClip fireClip,
        float defaultBgmVolume = 0.5f,
        float defaultFireSfxVolume = 0.7f)
    {
        bgmAudioSource = bgmSource;
        sfxAudioSource = sfxSource;
        mainBgmClip = bgmClip;
        fireSfxClip = fireClip;
        bgmVolume = defaultBgmVolume;
        fireSfxVolume = defaultFireSfxVolume;

        if (bgmAudioSource != null)
        {
            bgmAudioSource.clip = mainBgmClip;
            bgmAudioSource.loop = true;
            bgmAudioSource.playOnAwake = true;
            bgmAudioSource.spatialBlend = 0f;
            bgmAudioSource.volume = bgmVolume;
        }

        if (sfxAudioSource != null)
        {
            sfxAudioSource.loop = false;
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.spatialBlend = 0f;
            sfxAudioSource.volume = 1f;
        }
    }

    private void Start()
    {
        PlayMainBgm();
    }

    public void PlayMainBgm()
    {
        if (bgmAudioSource == null || mainBgmClip == null)
        {
            return;
        }

        bgmAudioSource.clip = mainBgmClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = bgmVolume;

        if (!bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }
    }

    public void StopMainBgm()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
    }

    public void PlayFireSfx()
    {
        if (sfxAudioSource == null || fireSfxClip == null)
        {
            return;
        }

        sfxAudioSource.PlayOneShot(fireSfxClip, fireSfxVolume);
    }
}
