using Border.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Border.ViewModel
{
    public class BuildOrderGroup : INotifyPropertyChanged
    {
        public BuildOrderGroup(BuildOrderData buildOrderData)
        {
            Author = buildOrderData.Author;
            Date = buildOrderData.Date;
            Revision = buildOrderData.Revision;

            for (int i = 0; i < buildOrderData.GameVersions.Length; i++)
            {
                GameVersions.Add(buildOrderData.GameVersions[i]);
            }
            
            for (int i = 0; i < buildOrderData.BuildOrders.Length; i++)
            {
                BuildOrders.Add(new DynamicBuildOrder(buildOrderData.BuildOrders[i]));
            }
        }

        public readonly ObservableCollection<DynamicBuildOrder> BuildOrders = new ObservableCollection<DynamicBuildOrder>();
        public readonly ObservableCollection<string> GameVersions = new ObservableCollection<string>();
        public BuildOrderData ToBuildOrderData()
        {
            var res = new BuildOrderData
            {
                Author = Author,
                Date = Date,
                GameVersions = new List<string>(GameVersions).ToArray(),
                Revision = Revision,
                BuildOrders = new BuildOrder[BuildOrders.Count]
            };
            for (int i = 0; i < BuildOrders.Count; i++)
            {
                res.BuildOrders[i] = BuildOrders[i].ToBuildOrder();
            }
            return res;
        }

        private string _Author;
        public string Author
        {
            get { return _Author; }
            set { _Author = value; OnPropertyChanged("Author"); }
        }

        private DateTimeOffset _Date;
        public DateTimeOffset Date
        {
            get { return _Date; }
            set { _Date = value; OnPropertyChanged("Date"); }
        }

        private long _Revision;
        public long Revision
        {
            get { return _Revision; }
            set { _Revision = value;  OnPropertyChanged("Revision"); }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
