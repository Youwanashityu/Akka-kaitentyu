using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OneResultController : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform animation;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button skipButton;

    private CancellationTokenSource _cts;

    // -------------------------------------------------------
    // 単発用（スキップボタン非表示）
    // -------------------------------------------------------

    /// <summary>
    /// 単発ガチャ結果を表示します。スキップボタンは非表示になります。
    /// </summary>
    public async UniTask ShowResult(ItemDisplayInfo displayInfo, CancellationToken token)
    {
        _cts = _cts.Reset();
        var linkedToken = _cts.LinkedToken(token);

        skipButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
        SetDisplayInfo(displayInfo);
        PlayAnimation();

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

    /// <summary>
    /// 10連ガチャの1件分を表示します。スキップボタンが押されたらtrueを返します。
    /// </summary>
    public async UniTask<bool> ShowResultWithSkip(ItemDisplayInfo displayInfo, CancellationToken token)
    {
        _cts = _cts.Reset();
        var linkedToken = _cts.LinkedToken(token);

        skipButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
        SetDisplayInfo(displayInfo);
        PlayAnimation();

        bool skipped = false;

        try
        {
            // 閉じるボタンかスキップボタンのどちらかを待つ
            int result = await UniTask.WhenAny(
                closeButton.OnClickAsync(linkedToken),
                skipButton.OnClickAsync(linkedToken)
            );
            skipped = result == 1; // 1ならスキップボタン
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
        animation.DOKill();
        animation.localScale = Vector3.one * 1.15f;
        animation.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutQuint);
    }

    private void SetDisplayInfo(ItemDisplayInfo info)
    {
        icon.sprite = info.Icon;
        displayName.text = info.DisplayName;
    }

    private void OnDestroy()
    {
        _cts = _cts.Clear();
    }
}