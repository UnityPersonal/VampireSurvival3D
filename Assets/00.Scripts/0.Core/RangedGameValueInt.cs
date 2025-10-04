using System;

[Serializable]
public class RangedGameValueInt : RangedGameValue<int>
{
    public override void ToRange()
    {
        this.value -= maxValue;
    }
}