using UnityEngine;

/// <summary>
/// ガチャから排出されるアイテム1種のデータ。
/// ScriptableObjectとして各アイテムをアセット化して管理します。
/// </summary>
[CreateAssetMenu(fileName = "GachaItem", menuName = "Gacha/GachaItem")]
public class GachaItem : ScriptableObject
{
    [Header("基本情報")]
    [SerializeField] private string _displayName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private ItemTier _tier;

    [Header("キャラクター設定")]
    [Tooltip("SSRキャラクターの場合はtrueにする")]
    [SerializeField] private bool _isCharacter;

    [Header("プレゼント設定")]
    [Tooltip("このアイテムをプレゼントしたときに再生する会話のTalkID")]
    [SerializeField] private string _presentTalkID;

    [Tooltip("2回目以降にプレゼントしたときに再生する会話のTalkID（nullなら毎回同じ会話）")]
    [SerializeField] private string _presentTalkIDAgain;

    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public ItemTier Tier => _tier;
    public bool IsCharacter => _isCharacter;
    public string PresentTalkID => _presentTalkID;
    public string PresentTalkIDAgain => _presentTalkIDAgain;
}