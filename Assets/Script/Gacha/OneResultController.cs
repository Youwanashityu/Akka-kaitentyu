using System;
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
    // [SerializeField] private SerializedDictionary<ItemTier, Sprite> tierItemBacks;

    private CancellationTokenSource _cts;

    public Button SkipButton => skipButton;

    public async UniTask ShowResult(ItemDisplayInfo displayInfo, CancellationToken token)
    {
        _cts = _cts.Reset();
        var linkedToken = _cts.LinkedToken(token);

        gameObject.SetActive(true);
        SetDisplayInfo(displayInfo);
        Animation();

        try
        {
            await closeButton.OnClickAsync(linkedToken);
        }
        finally
        {
            gameObject.SetActive(false);
        }
    }

    private void Animation()
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
