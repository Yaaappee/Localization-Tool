using System.ComponentModel;

namespace Localization_Tool
{
    public class Translation : NotifyProperyChangedBase
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                CheckPropertyChanged("Name", ref _name, ref value);
            }
        }

        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                CheckPropertyChanged("Value", ref _value, ref value);
            }
        }

        public Translation(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public abstract class NotifyProperyChangedBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region methods
        protected bool CheckPropertyChanged<T>
        (string propertyName, ref T oldValue, ref T newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return false;
            }

            if ((oldValue == null && newValue != null) || !oldValue.Equals((T)newValue))
            {
                oldValue = newValue;
                FirePropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}

