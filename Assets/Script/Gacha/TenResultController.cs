using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TenResultController : MonoBehaviour
{
    [SerializeField] private TenResultRowController[] rows;
    [SerializeField] private Button closeButton;
    [SerializeField] private OneResultController oneResultController;

    private CancellationTokenSource _cts;

    /// <summary>
    /// 10連ガチャ結果を表示します。
    /// 1件ずつOneResultControllerで表示し、最後にまとめ画面を表示します。
    /// </summary>
    public async UniTask ShowResult(ItemDisplayInfo[] infos, CancellationToken token)
    {
        _cts = _cts.Reset();
        var linkedToken = _cts.LinkedToken(token);

        // 1件ずつ表示（スキップされたら残りを飛ばす）
        for (int i = 0; i < infos.Length; i++)
        {
            bool skipped = await oneResultController.ShowResultWithSkip(infos[i], linkedToken);
            if (skipped) break;
        }

        // まとめ画面を表示
        ShowSummary(infos);
        gameObject.SetActive(true);

        try
        {
            await closeButton.OnClickAsync(linkedToken);
        }
        finally
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// まとめ画面に全結果を表示します。
    /// </summary>
    private void ShowSummary(ItemDisplayInfo[] infos)
    {
        ResetRows();
        for (int i = 0; i < infos.Length && i < rows.Length; i++)
        {
            rows[i].Initialize(infos[i]);
        }
    }

    private void ResetRows()
    {
        foreach (var row in rows)
        {
            row.Reset();
        }
    }

    private void OnDestroy()
    {
        _cts = _cts.Clear();
    }
}