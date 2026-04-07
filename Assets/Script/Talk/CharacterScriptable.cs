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
    [SerializeField] private GachaItem _gachaItem;

    [Header("チュートリアルキャラクター設定")]
    [Tooltip("最初からいるチュートリアルキャラクターの場合はtrueにする")]
    [SerializeField] private bool _isTutorialCharacter;

    [Header("画像")]
    [Tooltip("ホーム画面に表示する立ち絵")]
    [SerializeField] private Sprite _defaultSprite;
    [Tooltip("ミニキャラ画像")]
    [SerializeField] private Sprite _miniDefaultSprite;
    [Tooltip("キャラ切替ポップアップに表示するアイコン")]
    [SerializeField] private Sprite _selectIconSprite;

    [Header("画像一覧（ImageType名 → Sprite）")]
    [SerializeField] private ImageEntry[] _images;

    [Header("ボイス一覧（VoiceType名 → AudioClip）")]
    [SerializeField] private VoiceEntry[] _voices;

    // -------------------------------------------------------
    // プロパティ
    // -------------------------------------------------------

    public string CharacterName => _characterName;
    public GachaItem GachaItem => _gachaItem;
    public bool IsTutorialCharacter => _isTutorialCharacter;
    public Sprite DefaultSprite => _defaultSprite;
    public Sprite MiniDefaultSprite => _miniDefaultSprite;
    /// <summary>キャラ切替ポップアップ用アイコン。未設定の場合はDefaultSpriteを返します。</summary>
    public Sprite SelectIconSprite => _selectIconSprite != null ? _selectIconSprite : _defaultSprite;

    // -------------------------------------------------------
    // メソッド
    // -------------------------------------------------------

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

[System.Serializable]
public class ImageEntry
{
    [Tooltip("ImageTypeのenum名をそのまま書く（例：R_UP_EXCITING）")]
    public string ImageTypeName;
    public Sprite Sprite;
}

[System.Serializable]
public class VoiceEntry
{
    [Tooltip("VoiceTypeのenum名をそのまま書く（例：Hello）")]
    public string VoiceTypeName;
    public AudioClip Clip;
}