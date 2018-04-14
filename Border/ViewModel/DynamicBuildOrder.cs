using Border.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Border.ViewModel
{
    public class DynamicBuildOrder: INotifyPropertyChanged
    {
        public DynamicBuildOrder(BuildOrder buildOrderData)
        {
            Description = buildOrderData.Description;
            Title = buildOrderData.Title;
            for (int i = 0; i < buildOrderData.Queue.Length; i++)
            {
                Queue.Add(new QueueItem(buildOrderData.Queue[i]));
            }
        }

        public BuildOrder ToBuildOrder()
        {
            var res = new BuildOrder
            {
                Description = Description,
                Title = Title
            };
            for (int i = 0; i < Queue.Count; i++)
            {
                res.Queue[i] = Queue[i].ToQueueTask();
            }
            return res;
        }

        public void Reset()
        {
            Current = null;

            for (int i = 0; i < Queue.Count; i++)
            {
                Queue[i].IsSelected = false; // To cover external IsSelected calls
            }
        }

        public QueueItem Next()
        {
            if (Queue.Count == 0)
            {
                Current = null;
                return null;
            }
            int index = Queue.IndexOf(Current); // TODO: Make this more efficient by tracking the current Id
           
            if (index < 0)
            {
                Current = Queue[0];
                return Current;
            }else if(index >= Queue.Count - 2)
            {
                Current = Queue[Queue.Count - 1];
                return Current;
            }
            Current = Queue[index+1];
            return Current;
        }

        public QueueItem Previous()
        {
            if (Queue.Count == 0)
            {
                Current = null;
                return null;
            }
            int index = Queue.IndexOf(Current);

            if (index <= 0)
            {
                Current = Queue[0];
                return Current;
            }
            Current = Queue[index-1];
            return Current;
        }

        public bool SetCurrentQueueItem(QueueItem item)
        {
            if(Queue.IndexOf(item) == -1){
                return false;
            }else
            {
                Current = item;
            }
            return true;
        }

        public QueueItem GetQueueItemAtTime(double time)
        {
            if (Queue.Count == 0 ||  time < Queue[0].Time)
            {
                return null;
            }
            for (int i = 0; i < Queue.Count-1; i++)
            {
                if (Queue[i].Time < time &&
                    Queue[i + 1].Time > time)
                {
                    return Queue[i];
                }
            }
            return Queue[Queue.Count-1];
        }


        public QueueItem GoToQueueItemAtTime(double time)
        {
            if (Queue.Count == 0 || time < Queue[0].Time)
            {
                Current = null;
                return null;
            }
            for (int i = 0; i < Queue.Count - 1; i++)
            {
                if (Queue[i].Time < time &&
                    Queue[i + 1].Time > time)
                {
                    Current = Queue[i];
                }
            }
            return Queue[Queue.Count - 1];
        }

        public QueueItem _Current = null;
        public QueueItem Current
        {
            get {
                return _Current;
            }
            private set
            {
                if (Current != null)
                {
                    Current.IsSelected = false;
                }
                _Current = value;
                if (value != null)
                {
                    value.IsSelected = true;
                }
                OnPropertyChanged("Current");
            }
        }

        public ObservableCollection<QueueItem> Queue
        {
            get
            {
                return _Queue;
            }
            set
            {
                _Queue = value;
                OnPropertyChanged("Queue");
            }
        }
        public ObservableCollection<QueueItem> _Queue = new ObservableCollection<QueueItem>();

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

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public int GetCurrentId()
        {

            if (Current == null)
            {
                return -1;
            }
            return Queue.IndexOf(Current);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
