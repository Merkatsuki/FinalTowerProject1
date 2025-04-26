using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Strategies/StrategyBundle")]
public class StrategyBundleSO : ScriptableObject
{
    [Header("Entry Strategies")]
    public List<EntryStrategySO> entryStrategies = new List<EntryStrategySO>();

    [Header("Exit Strategies")]
    public List<ExitStrategySO> exitStrategies = new List<ExitStrategySO>();
}
