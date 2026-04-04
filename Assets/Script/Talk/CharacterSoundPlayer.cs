using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラクターの画像切り替えとボイス再生を担うコンポーネント。
/// ImageType・VoiceTypeは文字列で受け取り、辞書から対応するアセットを引きます。
/// </summary>
public class CharacterSoundPlayer : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [SerializeField] private TalkController _talkController;
    [SerializeField] private AudioSource _voiceAudioSource;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private Dictionary<string, Sprite> _images;
    private Dictionary<string, AudioClip> _voices;

    // -------------------------------------------------------
    // 初期化
    // -------------------------------------------------------

    /// <summary>
    /// 画像とボイスの辞書をセットします。
    /// キーはenumの名前文字列（例："R_UP_EXCITING"）です。
    /// </summary>
    public void Setup(Dictionary<string, Sprite> images, Dictionary<string, AudioClip> voices)
    {
        _images = images;
        _voices = voices;
    }

    // -------------------------------------------------------
    // 画像切り替え
    // -------------------------------------------------------

    /// <summary>
    /// ImageType名に対応するスプライトをキャラクター画像にセットします。
    /// </summary>
    public void SetImage(string imageType)
    {
        if (_images == null) return;

        if (_images.TryGetValue(imageType, out var sprite))
        {
            _talkController.CharacterImage.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"[CharacterSoundPlayer] ImageTypeが見つかりません: {imageType}");
        }
    }

    // -------------------------------------------------------
    // ボイス再生
    // -------------------------------------------------------

    /// <summary>
    /// VoiceType名に対応するAudioClipを再生します。
    /// </summary>
    public void PlayVoice(string voiceType)
    {
        if (_voices == null) return;

        if (_voices.TryGetValue(voiceType, out var clip))
        {
            _voiceAudioSource.Stop();
            _voiceAudioSource.clip = clip;
            _voiceAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"[CharacterSoundPlayer] VoiceTypeが見つかりません: {voiceType}");
        }
    }
}