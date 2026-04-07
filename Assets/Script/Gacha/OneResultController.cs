using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OneResultController : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform animationTarget;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button skipButton;

    [Header("レアリティ別SE名")]
    [SerializeField] private string _normalSEName = "SE_gacya_single_N";
    [SerializeField] private string _rareSEName = "SE_gacya_single_SR";

    private CancellationTokenSource _cts;

    // -------------------------------------------------------
    // 単発用（スキップボタン非表示）
    // -------------------------------------------------------

    public async UniTask ShowResult(ItemDisplayInfo displayInfo, CancellationToken token)
    {
        _cts = _cts.Reset();
        var linkedToken = _cts.LinkedToken(token);

        skipButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
        SetDisplayInfo(displayInfo);
        PlayAnimation();
        PlaySE(displayInfo.Tier);

        try
        {
            await closeButton.OnClickAsync(linkedToken);
        }
        finally
        {
            gameObject.SetActive(false);
        }
    }

    // -------------------------------------------------------
    // 10連用（スキップボタン表示）
    // -------------------------------------------------------

    public async UniTask<bool> ShowResultWithSkip(ItemDisplayInfo displayInfo, CancellationToken token)
    {
        _cts = _cts.Reset();
        var linkedToken = _cts.LinkedToken(token);

        skipButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
        SetDisplayInfo(displayInfo);
        PlayAnimation();
        PlaySE(displayInfo.Tier);

        bool skipped = false;

        try
        {
            int result = await UniTask.WhenAny(
                closeButton.OnClickAsync(linkedToken),
                skipButton.OnClickAsync(linkedToken)
            );
            skipped = result == 1;
        }
        finally
        {
            gameObject.SetActive(false);
        }

        return skipped;
    }

    // -------------------------------------------------------
    // 内部処理
    // -------------------------------------------------------

    private void PlayAnimation()
    {
        animationTarget.DOKill();
        animationTarget.localScale = Vector3.one * 1.15f;
        animationTarget.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutQuint);
    }

    private void SetDisplayInfo(ItemDisplayInfo info)
    {
        icon.sprite = info.Icon;
        displayName.text = info.DisplayName;
    }

    private void PlaySE(ItemTier tier)
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("[OneResultController] SoundManager が見つかりません。");
            return;
        }

        var seName = tier == ItemTier.R ? _normalSEName : _rareSEName;
        SoundManager.Instance.PlaySE(seName);
    }

    private void OnDestroy()
    {
        _cts = _cts.Clear();
    }
}