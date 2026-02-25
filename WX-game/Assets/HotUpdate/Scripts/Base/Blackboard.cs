using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard 
{
    Dictionary<string, IGenericField> dataStore = new Dictionary<string, IGenericField>();

    public void SetValue<T>(string key, T value)
    {
        dataStore[key] = new GenericfField<T> { Value = value };
    }

    public T GetValue<T>(string key)
    {
        if (dataStore.TryGetValue(key, out IGenericField value) && value.GetValueType() == typeof(T))
        {
            return ((GenericfField<T>)value).Value;        
        }
        return default;
    }
}
