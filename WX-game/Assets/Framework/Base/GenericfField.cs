using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IGenericField
{
    public Type GetValueType();
}
public class GenericfField<T> : IGenericField
{
    public T Value { get; set; }
    public Type GetValueType()
    {
        return typeof(T);
    }
}
