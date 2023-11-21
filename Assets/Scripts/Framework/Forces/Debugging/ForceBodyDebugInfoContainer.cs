using System.Collections.Generic;
using System.Linq;

public class ForceBodyDebugInfoContainer
{
    private Dictionary<List<IForceable>, Dictionary<IForceable, ForceDebugInfo>> _dictionary = new Dictionary<List<IForceable>, Dictionary<IForceable, ForceDebugInfo>>();

    public bool Add(List<IForceable> targetCollection, IForceable force, ForceDebugInfo debugInfo)
    {
        if (_dictionary.TryGetValue(targetCollection, out var mapInfo))
            return AddToExistingDictionary(mapInfo, force, debugInfo);

        var newMap = new Dictionary<IForceable, ForceDebugInfo>();
        _dictionary.Add(targetCollection, newMap);
        return AddToExistingDictionary(newMap, force, debugInfo);
    }

    public bool Remove(List<IForceable> targetCollection, IForceable force)
    {
        if (!_dictionary.TryGetValue(targetCollection, out var mapInfo))
            return false;

        var debugInfo = mapInfo[force];
        debugInfo.Dispose();
        mapInfo.Remove(force);

        if (mapInfo.Count == 0)
            _dictionary.Remove(targetCollection);

        return true;
    }

    public bool Move(IForceable force, List<IForceable> from, List<IForceable> to)
    {
        if (ReferenceEquals(from, to))
            return false;
        if (!_dictionary.TryGetValue(from, out var firstMap))
            return false;
        if (!_dictionary.TryGetValue(to, out var secondMap))
            return false;

        var movingForce = firstMap[force];
        firstMap.Remove(force);
        secondMap.Add(force, movingForce);
        
        return true;
    }

    public bool Concat(List<IForceable> @base, List<IForceable> toAppend)
    {
        if (ReferenceEquals(@base, toAppend))
            return false;
        if (!_dictionary.TryGetValue(@base, out var baseMap))
            return false;
        if (!_dictionary.TryGetValue(toAppend, out var toAppendMap))
            return false;

        foreach (var pair in toAppendMap)
        {
            baseMap.Add(pair.Key, pair.Value);
        }
        
        toAppendMap.Clear();
        _dictionary.Remove(toAppend);

        return true;
    }

    public void Clear(List<IForceable>[] targetCollections)
    {
        var l = targetCollections.Length;
        for (var i = 0; i < l; i++)
        {
            var currentCollection = targetCollections[i];
            if (!_dictionary.TryGetValue(currentCollection, out var infoMap))
                continue;

            foreach (var pair in infoMap)
                pair.Value.Dispose();
            
            infoMap.Clear();
            _dictionary.Remove(currentCollection);
        }
    }

    public void Empty()
    {
        foreach (var grandPair in _dictionary)
        {
            var currentInfoMap = grandPair.Value;
            var l = currentInfoMap.Count;
            for (var i = 0; i < l; i++)
            {
                var pair = currentInfoMap.ElementAt(0);
                pair.Value.Dispose();
                currentInfoMap.Remove(pair.Key);
            }
        }
    }

    public Dictionary<IForceable, ForceDebugInfo> GetMapOf(List<IForceable> targetCollection) =>
        _dictionary[targetCollection];

    public bool TryGetMapOf(List<IForceable> targetCollection, out Dictionary<IForceable, ForceDebugInfo> map) =>
        _dictionary.TryGetValue(targetCollection, out map);

    public (List<IForceable> targetCollection, ForceDebugInfo debugInfo)? GetDebugInfo(IForceable force)
    {
        foreach (var grandPair in _dictionary)
        {
            if(!grandPair.Value.TryGetValue(force, out var debugInfo))
                continue;

            return (grandPair.Key, debugInfo);
        }

        return null;
    }

    private static bool AddToExistingDictionary(Dictionary<IForceable, ForceDebugInfo> mappedInfo, IForceable force, ForceDebugInfo debugInfo)
    {
        if (mappedInfo.ContainsKey(force))
            return false;
        
        mappedInfo.Add(force, debugInfo);
        return true;
    }
}