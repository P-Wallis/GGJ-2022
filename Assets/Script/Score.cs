using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ScoreData
{
    public List<ScoreDatum> data = new List<ScoreDatum>();
    public int lastPlay = -1;
    public string lastName = "";
}

[System.Serializable]
public class ScoreDatum
{
    public int gemCount;
    public int playIndex;
    public string name;
}

public class ScoreSaver
{
    static string savePath;
    static string SavePath
    {
        get
        {
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = Path.Combine(Application.persistentDataPath, "HighScores.json");
            }
            return savePath;
        }
    }

    public static ScoreData LoadScoreData()
    {
        if(File.Exists(SavePath))
        {
            ScoreData scoreData = JsonUtility.FromJson<ScoreData>(File.ReadAllText(SavePath));
            if (scoreData.data == null)
            {
                scoreData.data = new List<ScoreDatum>();
            }
            if(scoreData.lastName == null)
            {
                scoreData.lastName = "";
            }
            return scoreData;
        }

        return new ScoreData();
    }

    public static void SaveScoreData(ScoreData data)
    {
        File.WriteAllText(SavePath,JsonUtility.ToJson(data));
    }
}
