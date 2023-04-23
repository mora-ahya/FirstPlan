using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMathf
{
    public enum CubicEasingType
    {
        None,
        In,
        Out,
        InOut,
        OutIn,
    }

    public static float CubicEasing(float t, CubicEasingType type = CubicEasingType.In)
    {
        t = Mathf.Clamp01(t);

        switch (type)
        {
            case CubicEasingType.In:
                t = Mathf.Pow(t, 3.0f);
                break;

            case CubicEasingType.Out:
                t = 1.0f - Mathf.Pow(1.0f - t, 3.0f);
                break;

            case CubicEasingType.InOut:
                if (t < 0.5f)
                {
                    t = CubicEasing(2 * t);
                }
                else
                {
                    t = CubicEasing(2 * t - 1.0f, CubicEasingType.Out) + 1.0f;
                }
                t /= 2.0f;
                break;

            case CubicEasingType.OutIn:
                if (t < 0.5f)
                {
                    t = CubicEasing(2 * t, CubicEasingType.Out);
                }
                else
                {
                    t = CubicEasing(2 * t - 1.0f) + 1.0f;
                }
                t /= 2.0f;
                break;
        }
        return t;
    }
}
