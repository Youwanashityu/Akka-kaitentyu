using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレゼントポップアップのUIを管理するコントローラー。
/// インベントリからSR・Rのお菓子を表示してアイテム選択を受け付けます。
/// </summary>
public class PresentPopupController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Transform _itemListParent;
    [SerializeField] private PresentItemButton _itemButtonPrefab;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private Action<GachaItem> _onItemSelected;
    private readonly List<PresentItemButton> _itemButtons = new();

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _closeButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // 表示制御
    // -------------------------------------------------------

    /// <summary>
    /// プレゼントポップアップを表示します。
    /// </summary>
    public void Show(Action<GachaItem> onItemSelected)
    {
        _onItemSelected = onItemSelected;
        RefreshItemList();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // アイテムリスト更新
    // -------------------------------------------------------

    /// <summary>
    /// インベントリからSR・Rのアイテムだけを取り出してリストを更新します。
    /// </summary>
    private void RefreshItemList()
    {
        // 既存のボタンをクリア
        foreach (var btn in _itemButtons)
        {
            Destroy(btn.gameObject);
        }
        _itemButtons.Clear();

        var inventory = GachaManager.Instance.Inventory;

        foreach (var kvp in inventory)
        {
            var item = kvp.Key;
            var count = kvp.Value;

            // SSRキャラクターはプレゼント不可
            if (item.Tier == ItemTier.SSR) continue;
            if (count <= 0) continue;

            var btn = Instantiate(_itemButtonPrefab, _itemListParent);
            btn.Setup(item, count, OnItemSelected);
            _itemButtons.Add(btn);
        }
    }

    // -------------------------------------------------------
    // アイテム選択
    // -------------------------------------------------------

    private void OnItemSelected(GachaItem item)
    {
        Hide();
        _onItemSelected?.Invoke(item);
    }
}