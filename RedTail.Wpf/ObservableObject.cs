using System.ComponentModel;

namespace RedTail.Wpf
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            var func = PropertyChanged;
            if (func != null)
            {
                func(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
