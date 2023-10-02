using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace org.bidib.Net.Core.Models;

public class ModelBase : INotifyPropertyChanged
{
    protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }

        field = newValue;
        OnPropertyChanged(GetPropertyName(propertyExpression));
        return true;
    }
        
    public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
        var propertyName = GetPropertyName(propertyExpression);

        if (!string.IsNullOrEmpty(propertyName))
        {
            OnPropertyChanged(propertyName);
        }
    }

    protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
    {
        if (propertyExpression == null)
        {
            throw new ArgumentNullException(nameof(propertyExpression));
        }

        if (propertyExpression.Body is not MemberExpression body)
        {
            throw new InvalidCastException("Expression body must be of type " + typeof(MemberExpression));
        }

        if (body.Member is not PropertyInfo property)
        {
            throw new ArgumentException("Body member must be a property");
        }

        return property.Name;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}