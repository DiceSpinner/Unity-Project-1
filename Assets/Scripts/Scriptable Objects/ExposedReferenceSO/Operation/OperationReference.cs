using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OperationReference
{
    [SerializeField] private string description;
    [SerializeField] private OperationSO operation;
    [SerializeField] private CoroutineOperationSO coroutine;
    [SerializeField] private bool useCoroutine;

    public void Operate() {
        if (useCoroutine)
        {
           coroutine.Operate();
        }
        else {
            operation.Operate();
        }
    }
}