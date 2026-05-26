using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettingsController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MasterParam = "MasterVolume";
    private const string MusicParam = "MusicVolume";
    private const string SfxParam = "SfxVolume";

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat(MasterParam, 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat(SfxParam, 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat(MusicParam, 0.5f);

        ApplyVolume(MasterParam, masterSlider.value);
        ApplyVolume(MusicParam, musicSlider.value);
        ApplyVolume(SfxParam, sfxSlider.value);

        masterSlider.onValueChanged.AddListener(v => OnSliderChanged(MasterParam, v));
        musicSlider.onValueChanged.AddListener(v => OnSliderChanged(MusicParam, v));
        sfxSlider.onValueChanged.AddListener(v => OnSliderChanged(SfxParam, v));
    }

    private void OnSliderChanged(string param, float value)
    {
        ApplyVolume(param, value);
        PlayerPrefs.SetFloat(param, value);
        Debug.Log(param + " was changed");
    }

    private void ApplyVolume(string param, float value)
    {
        float dB = value > 0.001f ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat(param, dB);
    }
}
