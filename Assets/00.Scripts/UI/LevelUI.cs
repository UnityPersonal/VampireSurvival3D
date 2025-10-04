using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Slider expBar;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text levelText;
    public void OnLevelChanged(int oldLevel, int newLevel)
    {
        levelText.text = newLevel.ToString();
    }

    public void OnExpChanged(int oldExp, int newExp, int maxExp)
    {
        this.expBar.value = newExp;
        this.expBar.maxValue = maxExp;
        this.expText.text = $"{newExp}/{maxExp}";
    }
        
}