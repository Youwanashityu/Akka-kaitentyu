using UnityEngine;

/// <summary>
/// ガチャ結果をUIに渡すための表示用データ。
/// GachaItemから生成して使います。
/// </summary>
public class ItemDisplayInfo
{
    public string DisplayName { get; }
    public Sprite Icon { get; }
    public ItemTier Tier { get; }

    public ItemDisplayInfo(GachaItem item)
    {
        DisplayName = item.DisplayName;
        Icon = item.Icon;
        Tier = item.Tier;
    }
}