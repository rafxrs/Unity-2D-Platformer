using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// One repository for all scriptable objects. Create your query methods here to keep your business logic clean.
/// I make this a MonoBehaviour as sometimes I add some debug/development references in the editor.
/// If you don't feel free to make this a standard class
/// </summary>
public class ResourceSystem : StaticInstance<ResourceSystem> {
    public List<ScriptableUnitBase> units { get; private set; }
    private Dictionary<UnitType, ScriptableUnitBase> _unitDict;
//-------------------------------------------------------------------------------------------//
    protected override void Awake() {
        base.Awake();
        AssembleResources();
    }
//-------------------------------------------------------------------------------------------//

    private void AssembleResources() {
        units = Resources.LoadAll<ScriptableUnitBase>("Units").ToList();
        _unitDict = units.ToDictionary(r => r.unitType, r => r);
    }
//-------------------------------------------------------------------------------------------//

    public ScriptableUnitBase GetExampleHero(UnitType t) => _unitDict[t];
    public ScriptableUnitBase GetRandomHero() => units[Random.Range(0, units.Count)];
}   