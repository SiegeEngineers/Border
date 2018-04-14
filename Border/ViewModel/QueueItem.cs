using Border.Model;
using System.ComponentModel;
using System.Windows;

namespace Border.ViewModel
{
    public class QueueItem : INotifyPropertyChanged
    {
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; OnPropertyChanged("Title"); }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; OnPropertyChanged("Description"); }
        }

        public string _ReadableTime;
        public string ReadableTime
        {
            get { return _ReadableTime; }
            protected set { _ReadableTime = value; OnPropertyChanged("ReadableTime"); }
        }
        
        private double _Time;
        public double Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                OnPropertyChanged("Time");
                UpdateReadableTime();
            }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public QueueItem(QueueTask item, bool isSelected = false)
        {
            Title = item.Title;
            Description = item.Description;
            Time = item.Time;
            IsSelected = isSelected;
            UpdateReadableTime();
        }

        protected void UpdateReadableTime()
        {
            long totalSeconds = (long)Time;
            long seconds = totalSeconds % 60;

            long hours = totalSeconds / 3600;
            long minutes = (totalSeconds - (hours * 3600)) / 60;
            if (hours > 0)
            {
                ReadableTime = string.Format("{0}:{1}:{2}", hours, minutes.ToString("00"), seconds.ToString("00"));
            }
            else
            {
                ReadableTime = string.Format("{0}:{1}", minutes, seconds.ToString("00"));
            }
        }

        public QueueTask ToQueueTask()
        {
            var res = new QueueTask();
            res.Description = Description;
            res.Time = (long) Time;
            res.Title = Title;
            return res;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
