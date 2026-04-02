using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 鍵の残数表示と＋ボタンによるショップポップアップ呼び出しを管理するコントローラー。
/// </summary>
public class KeyDisplayController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private TMP_Text _keyCountText;
    [SerializeField] private Button _addKeyButton;

    [Header("ショップポップアップ")]
    [SerializeField] private ShopPopupController _shopPopupController;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _addKeyButton.onClick.AddListener(OnAddKeyButtonClicked);
        UpdateDisplay();
    }

    // -------------------------------------------------------
    // 表示更新
    // -------------------------------------------------------

    /// <summary>
    /// 鍵残数の表示を最新の状態に更新します。
    /// ガチャを引いた後など、外部から呼び出せます。
    /// </summary>
    public void UpdateDisplay()
    {
        _keyCountText.text = GachaManager.Instance.KeyCount.ToString();
    }

    // -------------------------------------------------------
    // ボタンイベント
    // -------------------------------------------------------

    private void OnAddKeyButtonClicked()
    {
        _shopPopupController.Show(OnShopClosed);
    }

    private void OnShopClosed()
    {
        UpdateDisplay();
    }
}
