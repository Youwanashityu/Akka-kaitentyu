using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// プレイヤーがBGM・SE・VCの音量をスライダーで調整するUIコントローラー。
/// スライダーを最低にすると音量が0（無音）になります。
/// AudioMixerを直接操作します。
/// </summary>
public class VolumeController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("BGM")]
    [SerializeField] private Slider bgmSlider;

    [Header("SE")]
    [SerializeField] private Slider seSlider;

    [Header("VC")]
    [SerializeField] private Slider vcSlider;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        // スライダーの範囲を設定（min=0で最低音量が無音）
        bgmSlider.minValue = 0f;
        bgmSlider.maxValue = 1f;
        bgmSlider.value = 1f;

        seSlider.minValue = 0f;
        seSlider.maxValue = 1f;
        seSlider.value = 1f;

        vcSlider.minValue = 0f;
        vcSlider.maxValue = 1f;
        vcSlider.value = 1f;

        // リスナー登録
        bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        seSlider.onValueChanged.AddListener(OnSESliderChanged);
        vcSlider.onValueChanged.AddListener(OnVCSliderChanged);
    }

    // -------------------------------------------------------
    // スライダー
    // -------------------------------------------------------

    private void OnBGMSliderChanged(float value)
    {
        ApplyBGMVolume(value);
    }

    private void OnSESliderChanged(float value)
    {
        ApplySEVolume(value);
    }

    private void OnVCSliderChanged(float value)
    {
        ApplyVCVolume(value);
    }

    // -------------------------------------------------------
    // AudioMixer操作
    // -------------------------------------------------------

    /// <param name="volume">音量（0〜1）</param>
    private void ApplyBGMVolume(float volume)
    {
        float dB = volume <= 0.0001f ? -80f : Mathf.Clamp(Mathf.Log10(volume) * 20f, -80f, 0f);
        audioMixer.SetFloat("BGMVolume", dB);
    }

    /// <param name="volume">音量（0〜1）</param>
    private void ApplySEVolume(float volume)
    {
        float dB = volume <= 0.0001f ? -80f : Mathf.Clamp(Mathf.Log10(volume) * 20f, -80f, 0f);
        audioMixer.SetFloat("SEVolume", dB);
    }

    /// <param name="volume">音量（0〜1）</param>
    private void ApplyVCVolume(float volume)
    {
        float dB = volume <= 0.0001f ? -80f : Mathf.Clamp(Mathf.Log10(volume) * 20f, -80f, 0f);
        audioMixer.SetFloat("VCVolume", dB);
    }
}