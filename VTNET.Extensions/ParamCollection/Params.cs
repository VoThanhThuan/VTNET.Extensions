using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VTNET.Extensions;
/// <summary>
/// 
/// </summary>
public class Params : IEnumerable<KeyValuePair<string, object?>>
{ 
    Dictionary<string, object?> Parameters { get; set; } = new();
    readonly bool _IsParameterAuto = false;
    int _index = 0;
    public bool IsParameterAuto { get => _IsParameterAuto || _index > 0; }
    public int Count { get => Parameters.Count; }

    public ICollection<string> Keys => Parameters.Keys;

    public ICollection<object?> Values => Parameters.Values;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    public Params(params (string key, object? value)[] parameters)
    {
        foreach (var (key, value) in parameters)
        {
            Parameters.Add(key, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    public Params(params object?[] parameters)
    {
        _IsParameterAuto = true;
        foreach (var item in parameters)
        {
            Parameters.Add(_index++.ToString(), item);
        }
    }
    public Params(IEnumerable<KeyValuePair<string, object?>> parameters)
    {
        foreach (var item in parameters)
        {
            Add(item);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public Params()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object? this[string key]
    {
        set
        {
            Parameters[key] = value;
        }
        get { return Parameters[key]; }
    }    
    
    public object? this[int index]
    {
        set
        {
            Parameters[index.ToString()] = value;
        }
        get { return Parameters[index.ToString()]; }
    }    
    public T? Get<T>(string name)
    {
        return (T?)Parameters[name];
    }
    public void Add(string name, object? value)
    {
        Parameters.Add(name, value);
    }
    public void Add(object? value)
    {
        Parameters.Add(_index++.ToString(), value);
    }
    public void Add(KeyValuePair<string, object?> item)
    {
        Parameters.Add(item.Key, item.Value);
    }
    public bool ContainsKey(string key)
    {
        return Parameters.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return Parameters.Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
    {
        return Parameters.TryGetValue(key, out value);
    }

    public void Clear()
    {
        Parameters.Clear();
    }

    public bool Contains(KeyValuePair<string, object?> item)
    {
        return Parameters.Contains(item);
    }

    public bool Remove(KeyValuePair<string, object?> item)
    {
        return Parameters.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return Parameters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Parameters.GetEnumerator();
    }
}