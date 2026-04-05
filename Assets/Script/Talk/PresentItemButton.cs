using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレゼントインベントリの1アイテム分のボタン。
/// アイコン・アイテム名・個数を表示します。
/// </summary>
public class PresentItemButton : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _countText;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    public GachaItem Item { get; private set; }

    // -------------------------------------------------------
    // セットアップ
    // -------------------------------------------------------

    /// <summary>
    /// アイテムデータと個数・コールバックをセットします。
    /// </summary>
    public void Setup(GachaItem item, int count, Action<GachaItem> onSelected)
    {
        Item = item;
        _icon.sprite = item.Icon;
        _nameText.text = item.DisplayName;
        _countText.text = $"×{count}";
        _button.onClick.AddListener(() => onSelected(item));
    }

    /// <summary>
    /// 個数表示を更新します。0になったら非表示にします。
    /// </summary>
    public void UpdateCount(int count)
    {
        if (count <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        _countText.text = $"×{count}";
    }
}