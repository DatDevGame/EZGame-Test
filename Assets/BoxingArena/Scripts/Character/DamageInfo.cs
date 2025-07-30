using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo : MonoBehaviour
{
    public float Amount;
    public WeakPointType WeakPoint;
    public BaseBoxer Source;
    public bool IsCritical;

    public DamageInfo(float amount, WeakPointType weakPoint, BaseBoxer source = null, bool isCritical = false)
    {
        Amount = amount;
        WeakPoint = weakPoint;
        Source = source;
        IsCritical = isCritical;
    }
}
