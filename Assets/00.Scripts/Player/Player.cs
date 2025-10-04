using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Player : SingletonOnlyScene<Player>
{
    [SerializeField] private RangedGameValueInt exp =new RangedGameValueInt();
    [SerializeField] GameValue<int> level = new GameValue<int>();
    public GameValue<int> Level => level;

    [SerializeField] private float expMaxIncreaseRate = 0.3f;
    [FormerlySerializedAs("dropRadius")] public float ObtainRadius = 1f;

    protected override void InitializeSingleton()
    {
    }

    private void Start()
    {
        exp.OnOverflow.AddListener(() => { OnOverflowExp();});
    }

    private void OnOverflowExp()
    {
        level.Value += 1;
        exp.MaxValue = exp.MaxValue + (int)(exp.MaxValue * expMaxIncreaseRate);
    }

    public void OnObtain(int point)
    {
        exp.Value += point;
    }

}
