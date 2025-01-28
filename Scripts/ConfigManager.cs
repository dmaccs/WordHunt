using Godot;

namespace WordHunt;
public class ConfigManager
{
    private const string CONFIG_PATH = "user://config.cfg";
    private const string GAME_SECTION = "Game";
    private const string HIGH_SCORE_KEY = "HighScore";
    private const int DEFAULT_HIGH_SCORE = 0;
    
    public bool LoadFirstTimeStatus()
    {
        var config = new ConfigFile();
        Error err = config.Load(CONFIG_PATH);
        
        if (err != Error.Ok)
            return true; // Default to true if file doesn't exist
            
        return (bool)config.GetValue("Setup", "FirstTime", true);
    }
    
    public void SaveFirstTimeStatus(bool status)
    {
        var config = new ConfigFile();
        config.SetValue("Setup", "FirstTime", status);
        config.Save(CONFIG_PATH);
    }

    public int GetHighScore()
    {
        var config = new ConfigFile();
        Error err = config.Load(CONFIG_PATH);
        
        if (err != Error.Ok)
            return DEFAULT_HIGH_SCORE;
            
        return (int)config.GetValue(GAME_SECTION, HIGH_SCORE_KEY, DEFAULT_HIGH_SCORE);
    }
    
    public void SaveHighScore(int score)
    {
        var config = new ConfigFile();
        config.Load(CONFIG_PATH); // Load existing config if any
        config.SetValue(GAME_SECTION, HIGH_SCORE_KEY, score);
        config.Save(CONFIG_PATH);
    }
}