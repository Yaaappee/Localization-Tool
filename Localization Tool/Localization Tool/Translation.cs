using System.ComponentModel;

namespace Localization_Tool
{
    public class Translation : NotifyProperyChangedBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { CheckPropertyChanged("Name", ref _name, ref value); }
        }

        private string _uk;
        public string UK
        {
            get { return _uk; }
            set { CheckPropertyChanged("UK", ref _uk, ref value); }
        }

        private string _us;
        public string US
        {
            get { return _us; }
            set { CheckPropertyChanged("US", ref _us, ref value); }
        }

        private string _ru;
        public string RU
        {
            get { return _ru; }
            set { CheckPropertyChanged("RU", ref _ru, ref value); }
        }

        public string this[Lang index]
        {
            get
            {
                switch (index)
                {
                    case Lang.UK:
                        return UK;
                    case Lang.US:
                        return US;
                    case Lang.RU:
                        return RU;
                    default:
                        return "";
                }
            }
            set
            {
                switch (index)
                {
                    case Lang.UK:
                        UK = value;
                        break;
                    case Lang.US:
                        US = value;
                        break;
                    case Lang.RU:
                        RU = value;
                        break;
                }
            }
        }

        public Translation()
        {
        }

        public Translation(string name)
        {
            Name = name;
        }
    }

    public abstract class NotifyProperyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool CheckPropertyChanged<T>(string propertyName, ref T oldValue, ref T newValue)
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
    }
}

