using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TenResultController : MonoBehaviour
{
    [SerializeField]
    private TenResultRowController[] rows;
    [SerializeField]
    private Button closeButton;

    private CancellationTokenSource _cts;

    public async UniTask ShowResult(ItemDisplayInfo[] infos, CancellationToken token)
    {
        _cts = _cts.Reset();
        var linkedToken = _cts.LinkedToken(token);

        HideRows();
        gameObject.SetActive(true);

        try
        {
            ShowRows(infos, linkedToken).Forget();
            await closeButton.OnClickAsync(linkedToken);
        }
        finally
        {
            gameObject.SetActive(false);
        }
    }


    private void HideRows()
    {
        foreach (var t in rows)
        {
            t.Reset();
        }
    }

    private async UniTask ShowRows(ItemDisplayInfo[] infos, CancellationToken token)
    {
        for (int i = 0; i < infos.Length && i < rows.Length; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
            rows[i].Initialize(infos[i]);
        }
    }

    private void OnDestroy()
    {
        _cts = _cts.Clear();
    }
}
