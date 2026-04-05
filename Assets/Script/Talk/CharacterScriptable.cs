using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターごとの画像・ボイスを管理するScriptableObject。
/// Assets を右クリック → Data/Character で作成できます。
/// </summary>
[CreateAssetMenu(fileName = "NewCharacter", menuName = "Data/Character")]
public class CharacterScriptable : ScriptableObject
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("基本情報")]
    [SerializeField] private string _characterName;
    [SerializeField] private GachaItem _gachaItem; // SSRキャラクターのGachaItem（チュートリアルキャラはnull）

    [Header("デフォルト画像")]
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _miniDefaultSprite;

    [Header("画像一覧（ImageType名 → Sprite）")]
    [SerializeField] private ImageEntry[] _images;

    [Header("ボイス一覧（VoiceType名 → AudioClip）")]
    [SerializeField] private VoiceEntry[] _voices;

    // -------------------------------------------------------
    // プロパティ
    // -------------------------------------------------------

    public string CharacterName => _characterName;
    public GachaItem GachaItem => _gachaItem;
    public Sprite DefaultSprite => _defaultSprite;
    public Sprite MiniDefaultSprite => _miniDefaultSprite;

    // -------------------------------------------------------
    // メソッド
    // -------------------------------------------------------

    /// <summary>
    /// 画像の辞書を返します。キーはImageType名の文字列です。
    /// </summary>
    public Dictionary<string, Sprite> GetImageDict()
    {
        var dict = new Dictionary<string, Sprite>();
        foreach (var entry in _images)
        {
            if (!string.IsNullOrEmpty(entry.ImageTypeName) && entry.Sprite != null)
                dict[entry.ImageTypeName] = entry.Sprite;
        }
        return dict;
    }

    /// <summary>
    /// ボイスの辞書を返します。キーはVoiceType名の文字列です。
    /// </summary>
    public Dictionary<string, AudioClip> GetVoiceDict()
    {
        var dict = new Dictionary<string, AudioClip>();
        foreach (var entry in _voices)
        {
            if (!string.IsNullOrEmpty(entry.VoiceTypeName) && entry.Clip != null)
                dict[entry.VoiceTypeName] = entry.Clip;
        }
        return dict;
    }
}

/// <summary>
/// ImageType名とSpriteのペア。
/// </summary>
[System.Serializable]
public class ImageEntry
{
    [Tooltip("LuxImageTypeのenum名をそのまま書く（例：R_UP_EXCITING）")]
    public string ImageTypeName;
    public Sprite Sprite;
}

/// <summary>
/// VoiceType名とAudioClipのペア。
/// </summary>
[System.Serializable]
public class VoiceEntry
{
    [Tooltip("LuxVoiceTypeのenum名をそのまま書く（例：Hello）")]
    public string VoiceTypeName;
    public AudioClip Clip;
}