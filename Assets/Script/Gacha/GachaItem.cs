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

    /// <summary>キャラクターかどうか（SSRのみtrue）</summary>
    [Header("キャラクター設定")]
    [SerializeField] private bool _isCharacter;

    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public ItemTier Tier => _tier;
    public bool IsCharacter => _isCharacter;
}