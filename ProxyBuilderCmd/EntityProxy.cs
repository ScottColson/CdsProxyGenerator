using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;



[assembly: Microsoft.Xrm.Sdk.Client.ProxyTypesAssemblyAttribute()]
namespace CCLLC.CDS.Sdk
{
    public abstract partial class EntityProxy : Entity, INotifyPropertyChanged, INotifyPropertyChanging
    {
        public static eTextOptions DefaultTextOptions { get; set; } = eTextOptions.Ignore;
        public static eNumberOptions DefaultNumberOptions { get; set; } = eNumberOptions.Ignore;

        Dictionary<string, object> _changedValues = new Dictionary<string, object>();

        AttributeEqualityComparer _equalityComparer = new AttributeEqualityComparer();

        

        protected EntityProxy(string logicalName)
           : this(new Entity(logicalName)) { }

        protected EntityProxy(Entity original)
        {
            if (string.IsNullOrEmpty(original.LogicalName)) { throw new Exception("Please specify the 'logicalName' on the entity when using a proxy class."); }
            this.LogicalName = GetLogicalName(this.GetType());
            if (this.LogicalName != original.LogicalName) { throw new Exception("Please make sure that the entity logical name matches that of the proxy class you are creating."); }

            this.LogicalName = original.LogicalName;
            this.RelatedEntities.Clear();
            this.FormattedValues.Clear();
            this.Attributes.Clear();
            this.RelatedEntities.AddRange(original.RelatedEntities);
            this.FormattedValues.AddRange(original.FormattedValues);
            this.ExtensionData = original.ExtensionData;
            this.Attributes.AddRange(original.Attributes);
            this.EntityState = original.EntityState;
            this.Id = original.Id;
        }

        public static bool ReturnDatesInLocalTime = false;

        public Guid Create(IOrganizationService service)
        {
            this.Id = service.Create(this);
            _changedValues.Clear();
            return this.Id;
        }

        public void Delete(IOrganizationService service)
        {
            service.Delete(this.LogicalName, this.Id);
        }

        public void Update(IOrganizationService service)
        {
            if (_changedValues.Count > 0)
                service.Update(GetChangedEntity());
            _changedValues.Clear();
        }

        
        public Entity GetChangedEntity()
        {
            var entity = new Entity(this.LogicalName);
            entity.Id = this.Id;
            foreach (string attributeName in _changedValues.Keys)
                entity.Attributes[attributeName] = this.Attributes[attributeName];
            return entity;
        }

        public void Save(IOrganizationService service)
        {
            if (this.Id != Guid.Empty) { this.Update(service); }
            else { this.Create(service); }
        }    

        public static string GetLogicalName<T>() where T : EntityProxy
        {
            return GetLogicalName(typeof(T));
        }

        public static string GetLogicalName(Type t)
        {
            var logicalNameAttribute =
                (EntityLogicalNameAttribute)Attribute.GetCustomAttribute(t, typeof(EntityLogicalNameAttribute));

            return logicalNameAttribute?.LogicalName;
        }

        public static implicit operator EntityReference(EntityProxy proxy)
        {
            if (proxy != null) { return proxy.ToEntityReference(); }
            return null;
        }

        public T GetPropertyValue<T>(string name)
        {
            if (this.Contains(name))
            {
                var value = (T)this.Attributes[name];
                if ((typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?)) && ReturnDatesInLocalTime && value != null)
                    value = (T)(Object)((DateTime)(Object)value).ToLocalTime();
                return value;
            }
            return default(T);
        }

        public void SetPropertyValue<T>(string name, T value, string propertyName)
        {
            if (_changedValues.ContainsKey(name))
            {
                var originalValue = _changedValues[name];
                var currentValue = this.Contains(name) ? (object)this.GetPropertyValue<T>(name) : null;
                if (!_equalityComparer.Equals(currentValue, value))
                {
                    OnPropertyChanging(propertyName);
                    if (_equalityComparer.Equals(originalValue, value)) { _changedValues.Remove(name); }
                    this.Attributes[name] = value;
                    OnPropertyChanged(propertyName);
                }
            }
            else
            {
                var currentValue = this.Contains(name) ? (object)this.GetPropertyValue<T>(name) : null;
                if (!_equalityComparer.Equals(currentValue, value))
                {
                    OnPropertyChanging(propertyName);
                    _changedValues.Add(name, currentValue);
                    this.Attributes[name] = value;
                    OnPropertyChanged(propertyName);
                }
            }
        }

        private void OnPropertyChanging(string propertyName)
        {
            this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetPropertyValue(string logicalName, string value, int maxLength, string propertyName)
        {
            var textOptions = GetTextOptions(logicalName);
            if (textOptions != eTextOptions.Ignore && !string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                if (textOptions == eTextOptions.Truncate) { value = value.Substring(0, maxLength); }
                else { throw new Exception(string.Format(GetErrorString(logicalName, eErrorType.Text), logicalName, value, value.Length, maxLength)); }
            }
            SetPropertyValue<string>(logicalName, value, propertyName);
        }

        public void SetPropertyValue(string name, int? value, int minValue, int maxValue, string propertyName)
        {
            var numberOptions = GetNumberOptions(name);
            if (numberOptions != eNumberOptions.Ignore && (value < minValue || value > maxValue))
            {
                bool throwError = false;
                if (numberOptions == eNumberOptions.CorrectMinAndMax) { value = (value < minValue) ? minValue : maxValue; }
                else if (numberOptions == eNumberOptions.CorrectMinIgnoreMax) { value = (value < minValue) ? minValue : value; }
                else if (numberOptions == eNumberOptions.CorrectMinThrowMax && value < minValue) { value = minValue; }
                else if (numberOptions == eNumberOptions.CorrectMaxIgnoreMin) { value = (value > maxValue) ? maxValue : value; }
                else if (numberOptions == eNumberOptions.CorrectMaxThrowMin && value > maxValue) { value = maxValue; }
                else { throwError = true; }
                if (throwError) { throw new Exception(string.Format(GetErrorString(name, eErrorType.Number), name, value, minValue, maxValue)); }
            }
            SetPropertyValue(name, value, propertyName);
        }
        public void SetPropertyValue(string name, decimal? value, decimal minValue, decimal maxValue, string propertyName)
        {
            var numberOptions = GetNumberOptions(name);
            if (numberOptions != eNumberOptions.Ignore && (value < minValue || value > maxValue))
            {
                bool throwError = false;
                if (numberOptions == eNumberOptions.CorrectMinAndMax) { value = (value < minValue) ? minValue : maxValue; }
                else if (numberOptions == eNumberOptions.CorrectMinIgnoreMax) { value = (value < minValue) ? minValue : value; }
                else if (numberOptions == eNumberOptions.CorrectMinThrowMax && value < minValue) { value = minValue; }
                else if (numberOptions == eNumberOptions.CorrectMaxIgnoreMin) { value = (value > maxValue) ? maxValue : value; }
                else if (numberOptions == eNumberOptions.CorrectMaxThrowMin && value > maxValue) { value = maxValue; }
                else { throwError = true; }
                if (throwError) { throw new Exception(string.Format(GetErrorString(name, eErrorType.Number), name, value, minValue, maxValue)); }
            }
            SetPropertyValue(name, value, propertyName);
        }
        public void SetPropertyValue(string name, double? value, double minValue, double maxValue, string propertyName)
        {
            var numberOptions = GetNumberOptions(name);
            if (numberOptions != eNumberOptions.Ignore && (value < minValue || value > maxValue))
            {
                bool throwError = false;
                if (numberOptions == eNumberOptions.CorrectMinAndMax) { value = (value < minValue) ? minValue : maxValue; }
                else if (numberOptions == eNumberOptions.CorrectMinIgnoreMax) { value = (value < minValue) ? minValue : value; }
                else if (numberOptions == eNumberOptions.CorrectMinThrowMax && value < minValue) { value = minValue; }
                else if (numberOptions == eNumberOptions.CorrectMaxIgnoreMin) { value = (value > maxValue) ? maxValue : value; }
                else if (numberOptions == eNumberOptions.CorrectMaxThrowMin && value > maxValue) { value = maxValue; }
                else { throwError = true; }
                if (throwError) { throw new Exception(string.Format(GetErrorString(name, eErrorType.Number), name, value, minValue, maxValue)); }
            }
            SetPropertyValue(name, value, propertyName);
        }
        public void SetPropertyValue(string name, Money value, decimal minValue, decimal maxValue, string propertyName)
        {
            var numberOptions = GetNumberOptions(name);
            if (value != null && numberOptions != eNumberOptions.Ignore && (value.Value < minValue || value.Value > maxValue))
            {
                bool throwError = false;
                if (numberOptions == eNumberOptions.CorrectMinAndMax) { value.Value = (value.Value < minValue) ? minValue : maxValue; }
                else if (numberOptions == eNumberOptions.CorrectMinIgnoreMax) { value.Value = (value.Value < minValue) ? minValue : value.Value; }
                else if (numberOptions == eNumberOptions.CorrectMinThrowMax && value.Value < minValue) { value.Value = minValue; }
                else if (numberOptions == eNumberOptions.CorrectMaxIgnoreMin) { value.Value = (value.Value > maxValue) ? maxValue : value.Value; }
                else if (numberOptions == eNumberOptions.CorrectMaxThrowMin && value.Value > maxValue) { value.Value = maxValue; }
                else { throwError = true; }
                if (throwError) { throw new Exception(string.Format(GetErrorString(name, eErrorType.Number), name, value.Value, minValue, maxValue)); }
            }
            SetPropertyValue(name, value, propertyName);
        }
        //protected abstract eTextOptions GetTextOptions(string logicalName);
        //protected abstract string GetErrorString(string attributeName, eErrorType defaultErrorType);
        //protected abstract eNumberOptions GetNumberOptions(string logicalName);

        public bool IsDirty
        {
            get { return this._changedValues.Count > 0; }
        }



        private class AttributeEqualityComparer : IEqualityComparer
        {
            public new bool Equals(object x, object y)
            {
                if ((x == null || (x.GetType() == typeof(string) && string.IsNullOrEmpty(x as string))) && (y == null || (y.GetType() == typeof(string) && string.IsNullOrEmpty(y as string))))
                    return true;
                else
                {
                    if (x == null && y == null) { return true; }
                    else if (x == null && y != null) { return false; }
                    else if (x != null && y == null) { return false; }
                    else if (x.GetType() == y.GetType())
                    {
                        if (x.GetType() == typeof(OptionSetValue)) { return ((OptionSetValue)x).Value == ((OptionSetValue)y).Value; }
                        else if (x.GetType() == typeof(BooleanManagedProperty)) { return ((BooleanManagedProperty)x).Value == ((BooleanManagedProperty)y).Value; }
                        else if (x.GetType() == typeof(EntityReference))
                        {
                            if (((EntityReference)x).LogicalName == ((EntityReference)y).LogicalName) { return ((EntityReference)x).Id == ((EntityReference)y).Id; }
                            else { return false; }
                        }
                        else if (x.GetType() == typeof(Money)) { return (((Money)x).Value == ((Money)y).Value); }
                        else if (x.GetType() == typeof(DateTime) || x.GetType() == typeof(DateTime?))
                        {                                                                                                        
                            return Math.Abs(((DateTime)x - (DateTime)y).TotalSeconds) < 1;
                        }
                        else { return x.Equals(y); }
                    }
                    else { return false; }
                }
            }
            public int GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }
        }
        public enum eTextOptions
        {
            /// <summary>Ignore and let CRM handle any issues with the value</summary>
            Ignore,
            /// <summary>If the length is greater than the max length, truncate the value to the max length</summary>
            Truncate,
            /// <summary>Throw an error if the length of the value is greater than the max length</summary>
            ThrowError
        }
        
        public enum eNumberOptions
        {
            /// <summary>Ignore and let CRM handle any issues with the value.</summary>
            Ignore,
            /// <summary>If the value is less than the min value set the value as the min value.<para>Let CRM handle any issues with the max value.</para></summary>
            CorrectMinIgnoreMax,
            /// <summary>If the value is less than the min value set the value as the min value.<para>Throw an error if the value is greater than the max value.</para></summary>
            CorrectMinThrowMax,
            /// <summary>If the value is greater than the max value set the value as the max value.<para>Let CRM handle any issues with the min value.</para></summary>
            CorrectMaxIgnoreMin,
            /// <summary>If the value is greater than the max value set the value as the max value.<para>Throw an error if the value is less than the min value.</para></summary>
            CorrectMaxThrowMin,
            /// <summary>If the value is less than the min value set the value as the min value.<para>If the value is greater than the max value set the value as the max value.</para></summary>
            CorrectMinAndMax,
            /// <summary>Throw an error if the value is less than the min or greater than the max</summary>
            ThrowError
        }
       
        public enum eErrorType
        {
            Text,
            Number
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
    }

}