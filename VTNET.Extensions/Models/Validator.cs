using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace VTNET.Extensions.Models;
public struct ValidatorFields<T>
{
    //PropertyInfo[] _properties;
    T _obj;
    public List<ErrorExModel> Errors { get; set; } = new();
    public readonly bool IsValid => Errors.Count == 0;
    public ValidatorFields(T obj)
    {
        _obj = obj;
        //_properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
    public ValidatorsReturn<T> Check<P>(Expression<Func<T, P>> property)
    {
        if (property.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            var func = property.Compile();
            var value = func(_obj);

            return new ValidatorsReturn<T>(_obj, propertyName, value, Errors);
        }
        else
        {
            throw new ArgumentException("The provided expression does not specify a valid property.", nameof(property));
        }
    }
}

public partial struct ValidatorsReturn<T>
{
    public bool IsValid => Errors.Count == 0;
    public List<ErrorExModel> Errors { get; }
    
    readonly string _valueString;
    readonly string _fieldName;
    readonly object? _value;
    bool _isNot;
    bool _isValid;
    bool? _isValidOr;
    readonly T _obj;

    //PropertyInfo[] _properties;
    public ValidatorsReturn(T obj, string fieldName, object? value, List<ErrorExModel> errors)
    {
        _obj = obj;
        //_properties = properties;
        _fieldName = fieldName;
        Errors = errors;
        _value = value;
        _valueString = value?.ToString() ?? "";
    }
    public ValidatorsReturn(T obj, string fieldName, object? value, bool isValid, List<ErrorExModel> errors)
    {
        _obj = obj;
        //_properties = properties;
        _fieldName = fieldName;
        Errors = errors;
        _value = value;
        _valueString = value?.ToString() ?? "";
        _isValid = isValid;
    }
    public ValidatorsReturn<T> Return(string message)
    {
        if (_isNot)
            _isValid = !_isValid;
        _isNot = false;
        if (_isValidOr.HasValue)
        {
            if (_isValid && !_isValidOr.Value)
            {
                Errors.RemoveAt(Errors.Count-1);
            }
            _isValid = _isValid || _isValidOr.Value;
            if (_isValid)
            {
                return this;
            }
        }
        if (!_isValid && !string.IsNullOrEmpty(message))
        {
            Errors.Add(new()
            {
                Field = _fieldName,
                Message = message
            });
        }
        return this;
    }
    public ValidatorsReturn<T> Check<P>(Expression<Func<T, P>> property)
    {
        if (property.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            var func = property.Compile();
            var value = func(_obj);

            return new ValidatorsReturn<T>(_obj, propertyName, value, Errors);
        }
        else
        {
            throw new ArgumentException("The provided expression does not specify a valid property.", nameof(property));
        }
    }
    //public ValidatorsReturn<T> Check(string fieldName)
    //{
    //    var field = _properties.FirstOrDefault(f => f.Name == fieldName);
    //    if (field != null)
    //    {
    //        var value = field.GetValue(_obj);
    //        return new ValidatorsReturn<T>(_obj, fieldName, value, _properties, Errors);
    //    }
    //    return new ValidatorsReturn<T>(_obj, "", null, false, _properties, Errors);
    //}
    public ValidatorsReturn<T> Not { get
        {
            _isNot = !_isNot;
            return this;
        }
    }
    public ValidatorsReturn<T> Or { get
        {
            _isValidOr = _isValid;
            return this;
        }
    }

    public ValidatorsReturn<T> Is(object obj, string message)
    {
        _isValid = obj.Equals(_value);
        return Return(message);
    }

    public ValidatorsReturn<T> IsEmpty(string message)
    {
        _isValid = string.IsNullOrEmpty(_valueString);
        return Return(message);
    }
    public ValidatorsReturn<T> Matches(string pattern, string message)
    {
        _isValid = new Regex(pattern).IsMatch(_valueString);
        return Return(message);
    }
    public ValidatorsReturn<T> IsNumeric(string message)
    {
        return Matches("^[0-9]*$", message);
    }
    public ValidatorsReturn<T> IsTextOnly(string message)
    {
        _isValid = !RegTextOnly().IsMatch(_valueString);
        return Return(message);
    }
    public ValidatorsReturn<T> Contains(string text, string message, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
    {
        _isValid = _valueString.Contains(text, stringComparison);
        return Return(message);
    }
    public ValidatorsReturn<T> StartsWith(string text, string message)
    {
        _isValid = _valueString.StartsWith(text);
        return Return(message);
    }
    public ValidatorsReturn<T> EndsWith(string text, string message)
    {
        _isValid = _valueString.EndsWith(text);
        return Return(message);
    }
    public ValidatorsReturn<T> IsNumberType(string message)
    {
        _isValid = NumberEx.IsNumberType(_value);
        return Return(message);
    }
    public ValidatorsReturn<T> Length(int min, int max, string message)
    {
        _isValid = _valueString.Length >= min && _valueString.Length <= max;
        return Return(message);
    }
    public ValidatorsReturn<T> Length(int max, string message) => Length(0, max, message);

    public ValidatorsReturn<T> Number(decimal? min = null, decimal? max = null, string message = "")
    {
        if (!NumberEx.IsNumberType(_value))
        {
            _isValid = false;
            return Return(message);
        }
        var num = Convert.ToDecimal(_value);
        if(min.HasValue)
            _isValid = num >= min;
        if(max.HasValue)
            _isValid = num <= max;
        return Return(message);
    }

    [GeneratedRegex("^[0-9]*$")]
    private static partial Regex RegTextOnly();
}
