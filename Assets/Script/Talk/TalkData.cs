/// <summary>
/// CSV1行分の会話データ。
/// </summary>
public class TalkData
{
    public string TalkID { get; }
    public string ImageType { get; }
    public string VoiceType { get; }
    public string Text { get; }
    public string ChoiceA { get; }
    public string VoiceOnA { get; }
    public string ChoiceB { get; }
    public string VoiceOnB { get; }
    public string NextOnA { get; }
    public string NextOnB { get; }

    public bool HasChoice => !string.IsNullOrEmpty(ChoiceA);
    public bool HasBranch => !string.IsNullOrEmpty(ChoiceB);

    public TalkData(string talkID, string imageType, string voiceType, string text,
                    string choiceA, string voiceOnA, string choiceB, string voiceOnB,
                    string nextOnA, string nextOnB)
    {
        TalkID = talkID;
        ImageType = imageType;
        VoiceType = voiceType;
        Text = text;
        ChoiceA = choiceA;
        VoiceOnA = voiceOnA;
        ChoiceB = choiceB;
        VoiceOnB = voiceOnB;
        NextOnA = nextOnA;
        NextOnB = nextOnB;
    }
}