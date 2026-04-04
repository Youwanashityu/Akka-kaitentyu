using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ResourcesフォルダのCSVを読み込んでTalkDataのリストに変換します。
/// CSVはヘッダー行あり、Resources/CSV/ 以下に配置してください。
/// </summary>
public static class TalkDataLoader
{
    private const string CsvBasePath = "CSV/";

    /// <summary>
    /// 指定したCSVファイルを読み込んでTalkDataの辞書を返します。
    /// キーはTalkIDです。
    /// </summary>
    public static Dictionary<string, TalkData> Load(string fileName)
    {
        var result = new Dictionary<string, TalkData>();

        var asset = Resources.Load<TextAsset>(CsvBasePath + fileName);
        if (asset == null)
        {
            Debug.LogError($"[TalkDataLoader] CSVファイルが見つかりません: {CsvBasePath}{fileName}");
            return result;
        }

        using var reader = new StringReader(asset.text);

        // ヘッダー行をスキップ
        reader.ReadLine();

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = SplitCsvLine(line);
            if (cols.Length < 10) continue;

            var data = new TalkData(
                talkID: cols[0].Trim(),
                imageType: cols[1].Trim(),
                voiceType: cols[2].Trim(),
                text: cols[3].Trim(),
                choiceA: cols[4].Trim(),
                voiceOnA: cols[5].Trim(),
                choiceB: cols[6].Trim(),
                voiceOnB: cols[7].Trim(),
                nextOnA: cols[8].Trim(),
                nextOnB: cols[9].Trim()
            );

            if (!string.IsNullOrEmpty(data.TalkID))
            {
                result[data.TalkID] = data;
            }
        }

        Debug.Log($"[TalkDataLoader] {fileName} を読み込みました（{result.Count}件）");
        return result;
    }

    /// <summary>
    /// CSV行をカンマで分割します。ダブルクォートで囲まれたセル内の改行・カンマに対応しています。
    /// </summary>
    private static string[] SplitCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // エスケープされたダブルクォート（""）
                    if (i + 1 < line.Length && line[i + 1] == '"')
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
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
        }

        fields.Add(current.ToString());
        return fields.ToArray();
    }
}