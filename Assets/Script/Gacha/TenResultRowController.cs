using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 10連ガチャ結果まとめ画面の1枠分を管理するコントローラー。
/// TenResultControllerから初期化されます。
/// </summary>
public class TenResultRowController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private Image _icon;
    [SerializeField] private Image _background;

    [Header("レアリティ別背景画像")]
    [SerializeField] private Sprite _ssrBackground;
    [SerializeField] private Sprite _srBackground;
    [SerializeField] private Sprite _rBackground;

    // -------------------------------------------------------
    // 初期化・リセット
    // -------------------------------------------------------

    /// <summary>
    /// アイテム情報を表示します。TenResultControllerから呼ばれます。
    /// </summary>
    public void Initialize(ItemDisplayInfo info)
    {
        gameObject.SetActive(true);
        _icon.sprite = info.Icon;
        _background.sprite = GetBackgroundSprite(info.Tier);
    }

    /// <summary>
    /// 表示をリセットして非表示にします。
    /// </summary>
    public void Reset()
    {
        gameObject.SetActive(false);
        _icon.sprite = null;
        _background.sprite = null;
    }

    // -------------------------------------------------------
    // ヘルパー
    // -------------------------------------------------------

    private Sprite GetBackgroundSprite(ItemTier tier)
    {
        return tier switch
        {
            ItemTier.SSR => _ssrBackground,
            ItemTier.SR => _srBackground,
            _ => _rBackground,
        };
    }
}