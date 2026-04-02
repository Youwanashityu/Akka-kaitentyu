using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ガチャ画面のUIを管理するコントローラー。
/// 1回・10連ボタンの制御、結果表示の呼び出しを担います。
/// </summary>
public class GachaUIController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("ガチャボタン")]
    [SerializeField] private Button _singleDrawButton;
    [SerializeField] private Button _tenDrawButton;

    [Header("PickUpボタン")]
    [SerializeField] private Button _pickUpButton;

    [Header("結果表示")]
    [SerializeField] private OneResultController _oneResultController;
    [SerializeField] private TenResultController _tenResultController;

    [Header("PickUp選択UI")]
    [SerializeField] private PickUpSelectController _pickUpSelectController;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private CancellationTokenSource _cts;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _singleDrawButton.onClick.AddListener(OnSingleDrawButtonClicked);
        _tenDrawButton.onClick.AddListener(OnTenDrawButtonClicked);
        _pickUpButton.onClick.AddListener(OnPickUpButtonClicked);
    }

    private void OnDestroy()
    {
        _cts = _cts.Clear();
    }

    // -------------------------------------------------------
    // ボタンイベント
    // -------------------------------------------------------

    private void OnSingleDrawButtonClicked()
    {
        DrawSingleAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void OnTenDrawButtonClicked()
    {
        DrawTenAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void OnPickUpButtonClicked()
    {
        _pickUpSelectController.Show();
    }

    // -------------------------------------------------------
    // ガチャ実行
    // -------------------------------------------------------

    private async UniTaskVoid DrawSingleAsync(CancellationToken token)
    {
        SetButtonsInteractable(false);

        var item = GachaManager.Instance.DrawSingle();
        if (item == null)
        {
            Debug.LogWarning("[GachaUIController] 鍵が不足しているため1回ガチャを実行できません。");
            SetButtonsInteractable(true);
            return;
        }

        var displayInfo = new ItemDisplayInfo(item);
        await _oneResultController.ShowResult(displayInfo, token);

        SetButtonsInteractable(true);
    }

    private async UniTaskVoid DrawTenAsync(CancellationToken token)
    {
        SetButtonsInteractable(false);

        var items = GachaManager.Instance.DrawTen();
        if (items == null)
        {
            Debug.LogWarning("[GachaUIController] 鍵が不足しているため10連ガチャを実行できません。");
            SetButtonsInteractable(true);
            return;
        }

        var displayInfos = new ItemDisplayInfo[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            displayInfos[i] = new ItemDisplayInfo(items[i]);
        }

        await _tenResultController.ShowResult(displayInfos, token);

        SetButtonsInteractable(true);
    }

    // -------------------------------------------------------
    // ヘルパー
    // -------------------------------------------------------

    private void SetButtonsInteractable(bool interactable)
    {
        _singleDrawButton.interactable = interactable;
        _tenDrawButton.interactable = interactable;
        _pickUpButton.interactable = interactable;
    }
}