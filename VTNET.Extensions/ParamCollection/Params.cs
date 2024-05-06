using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VTNET.Extensions;

public class Params<T> : IEnumerable<ParamValue<T>>
{
    protected Dictionary<string, T?> Parameters { get; set; } = new();
    public int Count { get => Parameters.Count; }
    public ICollection<string> Keys => Parameters.Keys;

    public ICollection<T?> Values => Parameters.Values;
    protected int Index { get; set; } = 0;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    public Params(params (string key, T? value)[] parameters)
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
    public Params(params T?[] parameters)
    {
        foreach (var item in parameters)
        {
            Parameters.Add(Guid.NewGuid().ToString(), item);
        }
    }
    public Params(IEnumerable<KeyValuePair<string, T?>> parameters)
    {
        foreach (var item in parameters)
        {
            Add(item);
        }
    }
    public Params(IEnumerable<ParamValue<T>> parameters)
    {
        foreach (var item in parameters)
        {
            Add(item);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public Params() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public T? this[string key]
    {
        set
        {
            Parameters[key] = value;
        }
        get { return Parameters[key]; }
    }

    public T? this[int index]
    {
        set
        {
            if (index >= 0 && index < Parameters.Count)
            {
                var element = Parameters.ElementAt(index);
                Parameters[element.Key] = value;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
        get
        {
            if (index >= 0 && index < Parameters.Count)
            {
                return Parameters.ElementAt(index).Value;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
    public T? Get(string key)
    {
        return Parameters[key];
    }
    public void Set(string key, T? value)
    {
        Parameters[key] = value;
    }
    public T? Get(int index)
    {
        return this[index];
    }
    public void Add(string name, T? value)
    {
        Parameters.Add(name, value);
    }
    public void Add(T? value)
    {
        var prefix = "";
        while(!Parameters.TryAdd($"{prefix}{Index++}", value))
        {
            Index--;
            prefix += "#";
        }
    }
    public void Add((string, T?) item)
    {
        Parameters.Add(item.Item1, item.Item2);
    }
    public void Add(KeyValuePair<string, T?> item)
    {
        Parameters.Add(item.Key, item.Value);
    }
    public bool ContainsKey(string key)
    {
        return Parameters.ContainsKey(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T? value)
    {
        return Parameters.TryGetValue(key, out value);
    }

    public void Clear()
    {
        Parameters.Clear();
    }

    public bool Contains(KeyValuePair<string, T?> item)
    {
        return Parameters.Contains(item);
    }

    public bool Remove(string key)
    {
        return Parameters.Remove(key);
    }

    public bool Remove(KeyValuePair<string, T?> item)
    {
        return Parameters.Remove(item.Key);
    }

    public void ForEach(Action<T?> action)
    {
        foreach (var item in Parameters)
        {
            action(item.Value);
        }
    }
    public IEnumerable<T?> ForEach(Func<T?, T?> action)
    {
        foreach (var item in Parameters)
        {
            yield return action(item.Value);
        }
    }

    public T? Pop(string key)
    {
        if (Parameters.TryGetValue(key, out var data))
        {
            Parameters.Remove(key);
            return data;
        }
        return default;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Parameters.Select(x => new ParamValue<T>(x.Key, x.Value)).GetEnumerator();
    }

    IEnumerator<ParamValue<T>> IEnumerable<ParamValue<T>>.GetEnumerator()
    {
        return Parameters.Select(x => new ParamValue<T>(x.Key, x.Value)).GetEnumerator();
    }

    public Params ToParams()
    {
        var @params = new Params();
        foreach (var item in Parameters)
        {
            @params.Add(item.Key, item.Value);
        }
        return @params;
    }

    public static implicit operator Params(Params<T> data)
    {
        var @params = new Params();
        foreach (var item in data)
        {
            @params.Add(item);
        }
        return @params;
    }

    public static implicit operator Params<T>(T[] data)
    {
        var @params = new Params<T>();
        foreach (var item in data)
        {
            @params.Add(item);
        }
        return @params;
    }

    public static implicit operator Params<T>((string key, T? value)[] data)
    {
        var @params = new Params<T>();
        foreach (var item in data)
        {
            @params.Add(item.key, item.value);
        }
        return @params;
    }

    public static implicit operator Params<T>(Dictionary<string, T> data)
    {
        var @params = new Params<T>();
        foreach (var item in data)
        {
            @params.Add(item.Key, item.Value);
        }
        return @params;
    }
}

/// <summary>
/// 
/// </summary>
public class Params : Params<object>
{ 
    //Dictionary<string, object?> Parameters { get; set; } = new();
    //readonly bool _IsParameterAuto = false;
    //public bool IsParameterAuto { get => _IsParameterAuto; }
    //public int Count { get => Parameters.Count; }
    //public ICollection<string> Keys => Parameters.Keys;

    //public ICollection<object?> Values => Parameters.Values;

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
        foreach (var item in parameters)
        {
            Parameters.Add(Guid.NewGuid().ToString(), item);
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
    public Params(){}

    public virtual T? Get<T>(string key)
    {
        return (T?)Parameters[key];
    }
    public virtual T? Get<T>(int index)
    {
        return (T?)Parameters.ElementAt(index).Value;
    }

    public void ForEach<T>(Action<T?> action)
    {
        foreach (var item in Parameters)
        {
            action((T?)item.Value);
        }
    }
    public IEnumerable<T?> ForEach<T>(Func<T?, T?> action)
    {
        foreach (var item in Parameters)
        {
            yield return action((T?)item.Value);
        }
    }

    public T? Pop<T>(string key)
    {
        if (Parameters.TryGetValue(key, out var data))
        {
            Parameters.Remove(key);
            return (T?)data;
        }
        return default;
    }

    public static implicit operator Params(object[] data)
    {
        var p = new Params();
        foreach (var item in data)
        {
            p.Add(item);
        }
        return p;
    }
    public static implicit operator Params((string key, object? value)[] data)
    {
        var p = new Params();
        foreach (var item in data)
        {
            p.Add(item.key, item.value);
        }
        return p;
    }

}

[Serializable]
public readonly struct ParamValue<T>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _key;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly T? _value;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly bool _autokey;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Type _valueType;

    public ParamValue(T? value)
    {
        _autokey = true;
        _key = Guid.NewGuid().ToString();
        _value = value;
        _valueType = value?.GetType()??typeof(T);
    }
    public ParamValue(string key, T? value)
    {
        _key = key;
        _value = value;
        _valueType = value?.GetType() ?? typeof(T);
    }
    public ParamValue(string key, T? value, string tag)
    {
        _key = key;
        _value = value;
        _valueType = value?.GetType() ?? typeof(T);
    }
    public string Key => _key;
    public T? Value => _value;
    public bool IsAutoKey => _autokey;
    public Type ValueType => _valueType;

    public override string ToString()
    {
        return $"{_key}: {_value}";
    }

    public static implicit operator ParamValue<T>((string key, T? value) data)
    {
        return new ParamValue<T>(data.key, data.value);
    }
    public static implicit operator ParamValue<T>(T value)
    {
        return new ParamValue<T>(value);
    }    
    public static implicit operator ParamValue<T>(KeyValuePair<string, T?> value)
    {
        return new ParamValue<T>(value.Key, value.Value);
    }
    public static implicit operator KeyValuePair<string, T?>(ParamValue<T> value)
    {
        return KeyValuePair.Create(value.Key, value.Value);
    }
}