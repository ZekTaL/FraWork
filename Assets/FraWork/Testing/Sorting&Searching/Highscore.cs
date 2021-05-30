using System;

public class Highscore : IComparable
{
    /// <summary>
    /// Compare the highscores
    /// </summary>
    /// <param name="obj">Object to compare</param>
    /// <returns>
    /// < 0 - before
    /// = 0 - equal
    /// > 0 - after
    /// </returns>
    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        Highscore otherHighscore = obj as Highscore;

        if (otherHighscore != null)
        {
            if (this.score > otherHighscore.score)
                return 1;
            else if (this.score < otherHighscore.score)
                return -1;

            return 0;
        }
        else
        {
            // We couldn't cast the object, so is null
            throw new ArgumentException("Object is not a Highscore");
        }
    }

    protected string playerName;
    protected float score;

    public Highscore(float _score, string _playerName = "Player")
    {
        playerName = _playerName;
        score = _score;
    }

    public override string ToString()
    {
        return playerName + " - " + score.ToString();
    }

    public static Comparison<Highscore> playerNameCompare = (x, y) => x.playerName.CompareTo(y.playerName);
    public static Comparison<Highscore> ScoreCompare = (x, y) => x.score.CompareTo(y.score);
}
