using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 換金所ポップアップのUIを管理するコントローラー。
/// コインを消費して鍵を購入できます。
/// </summary>
public class ShopPopupController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("コイン表示")]
    [SerializeField] private TMP_Text _coinCountText;

    [Header("購入ボタン")]
    [SerializeField] private Button _buyOneButton;    // 1回分  250コイン
    [SerializeField] private Button _buyTenButton;    // 10連分 2500コイン
    [SerializeField] private Button _buyFiftyButton;  // 50連分 10000コイン

    [Header("閉じるボタン")]
    [SerializeField] private Button _closeButton;

    // -------------------------------------------------------
    // 購入設定
    // -------------------------------------------------------

    private const int BuyOneKeyCost = 250;
    private const int BuyOnekeyAmount = 1;

    private const int BuyTenKeyCost = 2500;
    private const int BuyTenKeyAmount = 10;

    private const int BuyFiftyKeyCost = 10000;
    private const int BuyFiftyKeyAmount = 50;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private Action _onClosed;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _buyOneButton.onClick.AddListener(() => OnBuyButtonClicked(BuyOneKeyCost, BuyOnekeyAmount));
        _buyTenButton.onClick.AddListener(() => OnBuyButtonClicked(BuyTenKeyCost, BuyTenKeyAmount));
        _buyFiftyButton.onClick.AddListener(() => OnBuyButtonClicked(BuyFiftyKeyCost, BuyFiftyKeyAmount));
        _closeButton.onClick.AddListener(OnCloseButtonClicked);

        gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // 表示制御
    // -------------------------------------------------------

    /// <summary>
    /// ショップポップアップを表示します。
    /// </summary>
    /// <param name="onClosed">閉じたときに呼ばれるコールバック</param>
    public void Show(Action onClosed = null)
    {
        _onClosed = onClosed;
        UpdateCoinDisplay();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        _onClosed?.Invoke();
        _onClosed = null;
    }

    // -------------------------------------------------------
    // 購入処理
    // -------------------------------------------------------

    private void OnBuyButtonClicked(int cost, int keyAmount)
    {
        if (!CoinManager.Instance.TrySpendCoin(cost))
        {
            Debug.LogWarning($"[ShopPopupController] コインが不足しています。必要: {cost}, 所持: {CoinManager.Instance.CoinCount}");
            return;
        }

        GachaManager.Instance.AddKey(keyAmount);
        UpdateCoinDisplay();
        Debug.Log($"[ShopPopupController] 鍵{keyAmount}個購入。残コイン: {CoinManager.Instance.CoinCount}");
    }

    // -------------------------------------------------------
    // 表示更新
    // -------------------------------------------------------

    private void UpdateCoinDisplay()
    {
        _coinCountText.text = CoinManager.Instance.CoinCount.ToString("N0");
    }

    // -------------------------------------------------------
    // ボタンイベント
    // -------------------------------------------------------

    private void OnCloseButtonClicked()
    {
        Hide();
    }
}