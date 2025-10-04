
using System;

[Serializable]
public class RangedGameValueFloat : RangedGameValue<float>
{
    public override void ToRange()
    {
        this.value -= this.maxValue;
    }
}