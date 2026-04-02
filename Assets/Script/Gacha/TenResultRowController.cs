using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 10連ガチャ結果画面の1行分を管理するコントローラー。
/// TenResultControllerから初期化されます。
/// </summary>
public class TenResultRowController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _displayName;
    [SerializeField] private TMP_Text _tierText;

    [Header("レアリティ別背景色")]
    [SerializeField] private Image _background;
    [SerializeField] private Color _ssrColor = new Color(1f, 0.85f, 0f);
    [SerializeField] private Color _srColor = new Color(0.8f, 0.5f, 1f);
    [SerializeField] private Color _rColor = new Color(0.7f, 0.7f, 0.7f);

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
        _displayName.text = info.DisplayName;
        _tierText.text = info.Tier.ToString();
        _background.color = GetTierColor(info.Tier);
    }

    /// <summary>
    /// 表示をリセットして非表示にします。
    /// </summary>
    public void Reset()
    {
        gameObject.SetActive(false);
        _icon.sprite = null;
        _displayName.text = string.Empty;
        _tierText.text = string.Empty;
    }

    // -------------------------------------------------------
    // ヘルパー
    // -------------------------------------------------------

    private Color GetTierColor(ItemTier tier)
    {
        return tier switch
        {
            ItemTier.SSR => _ssrColor,
            ItemTier.SR => _srColor,
            _ => _rColor,
        };
    }
}