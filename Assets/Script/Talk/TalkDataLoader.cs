using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ResourcesフォルダのCSVを読み込んでTalkDataのリストに変換します。
/// CSVはヘッダー行あり、Resources/CSV/ 以下に配置してください。
/// TalkIDが空の行は直前のTalkIDを引き継ぎます。
/// セル内の改行にも対応しています。
/// </summary>
public static class TalkDataLoader
{
    private const string CsvBasePath = "CSV/";

    public static Dictionary<string, TalkData> Load(string fileName)
    {
        var result = new Dictionary<string, TalkData>();

        var asset = Resources.Load<TextAsset>(CsvBasePath + fileName);
        if (asset == null)
        {
            Debug.LogError($"[TalkDataLoader] CSVファイルが見つかりません: {CsvBasePath}{fileName}");
            return result;
        }

        // テキスト全体をパースしてフィールドのリストに変換
        var allRows = ParseCsv(asset.text);
        if (allRows.Count <= 1)
        {
            Debug.Log($"[TalkDataLoader] {fileName} を読み込みました（0件）");
            return result;
        }

        // ヘッダー行をスキップ
        string currentTalkID = null;
        var pendingLines = new List<string[]>();

        for (int rowIndex = 1; rowIndex < allRows.Count; rowIndex++)
        {
            var cols = allRows[rowIndex];
            if (cols.Length < 10) continue;

            var talkID = cols[0].Trim();

            if (!string.IsNullOrEmpty(talkID))
            {
                if (currentTalkID != null && pendingLines.Count > 0)
                {
                    var talkDataList = BuildTalkData(currentTalkID, pendingLines);
                    foreach (var td in talkDataList)
                        result[td.TalkID] = td;
                    pendingLines.Clear();
                }
                currentTalkID = talkID;
            }

            if (currentTalkID != null)
                pendingLines.Add(cols);
        }

        if (currentTalkID != null && pendingLines.Count > 0)
        {
            var talkDataList = BuildTalkData(currentTalkID, pendingLines);
            foreach (var td in talkDataList)
                result[td.TalkID] = td;
        }

        Debug.Log($"[TalkDataLoader] {fileName} を読み込みました（{result.Count}件）");
        return result;
    }

    /// <summary>
    /// CSV全体をパースして行×列の2次元リストに変換します。
    /// セル内の改行・カンマに対応しています。
    /// </summary>
    private static List<string[]> ParseCsv(string text)
    {
        var rows = new List<string[]>();
        var currentRow = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // エスケープされたダブルクォート
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    // セル内の改行もそのまま取り込む
                    current.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    currentRow.Add(current.ToString());
                    current.Clear();
                }
                else if (c == '\n')
                {
                    // 行末
                    currentRow.Add(current.ToString().TrimEnd('\r'));
                    current.Clear();
                    rows.Add(currentRow.ToArray());
                    currentRow.Clear();
                }
                else if (c == '\r')
                {
                    // \r\nの\rはスキップ
                }
                else
                {
                    current.Append(c);
                }
            }
        }

        // 最後の行
        if (current.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(current.ToString());
            rows.Add(currentRow.ToArray());
        }

        return rows;
    }

    private static List<TalkData> BuildTalkData(string baseTalkID, List<string[]> lines)
    {
        var result = new List<TalkData>();
        string lastImageType = string.Empty;

        for (int i = 0; i < lines.Count; i++)
        {
            var cols = lines[i];
            var talkID = i == 0 ? baseTalkID : $"{baseTalkID}_{i}";
            var imageType = cols[1].Trim();
            var voiceType = cols[2].Trim();
            var text = cols[3].Trim();
            var choiceA = cols.Length > 4 ? cols[4].Trim() : string.Empty;
            var voiceOnA = cols.Length > 5 ? cols[5].Trim() : string.Empty;
            var choiceB = cols.Length > 6 ? cols[6].Trim() : string.Empty;
            var voiceOnB = cols.Length > 7 ? cols[7].Trim() : string.Empty;
            var nextOnA = cols.Length > 8 ? cols[8].Trim() : string.Empty;
            var nextOnB = cols.Length > 9 ? cols[9].Trim() : string.Empty;

            // ImageTypeが空なら前の行を引き継ぐ
            if (!string.IsNullOrEmpty(imageType))
                lastImageType = imageType;
            else
                imageType = lastImageType;

            // 最後の行でなければ次の行へ自動で進む
            if (string.IsNullOrEmpty(nextOnA) && string.IsNullOrEmpty(choiceA) && i < lines.Count - 1)
                nextOnA = $"{baseTalkID}_{i + 1}";

            result.Add(new TalkData(talkID, imageType, voiceType, text,
                choiceA, voiceOnA, choiceB, voiceOnB, nextOnA, nextOnB));
        }

        return result;
    }
}