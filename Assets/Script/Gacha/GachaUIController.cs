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
    [SerializeField] private Image _pickUpButtonImage;
    [SerializeField] private Sprite _pickUpDefaultSprite;

    [Header("結果表示")]
    [SerializeField] private OneResultController _oneResultController;
    [SerializeField] private TenResultController _tenResultController;

    [Header("PickUp選択UI")]
    [SerializeField] private PickUpSelectController _pickUpSelectController;

    [Header("鍵表示")]
    [SerializeField] private KeyDisplayController _keyDisplayController;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _singleDrawButton.onClick.AddListener(OnSingleDrawButtonClicked);
        _tenDrawButton.onClick.AddListener(OnTenDrawButtonClicked);
        _pickUpButton.onClick.AddListener(OnPickUpButtonClicked);

        UpdatePickUpButtonImage();
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
        _pickUpSelectController.Show(OnPickUpSelected);
    }

    private void OnPickUpSelected()
    {
        UpdatePickUpButtonImage();
    }

    // -------------------------------------------------------
    // PickUpボタン画像更新
    // -------------------------------------------------------

    public void UpdatePickUpButtonImage()
    {
        var selected = GachaManager.Instance.SelectedPickUp;
        _pickUpButtonImage.sprite = selected != null ? selected.Icon : _pickUpDefaultSprite;
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

        _keyDisplayController.UpdateDisplay();

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

        _keyDisplayController.UpdateDisplay();

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