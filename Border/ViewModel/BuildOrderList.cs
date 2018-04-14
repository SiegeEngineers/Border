using Border.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Border.ViewModel
{
    public class BuildOrderList : INotifyPropertyChanged
    {
        private ObservableCollection<DynamicBuildOrder> _List = new ObservableCollection<DynamicBuildOrder>();
        public ObservableCollection<DynamicBuildOrder> List
        {
            get
            {
                return _List;
            }
            private set
            {
                _List = value;
                OnPropertyChanged("List");
            }
        }

        public void AddBuildOrder(BuildOrder buildOrder)
        {
            List.Add(new DynamicBuildOrder(buildOrder));
            //OnPropertyChanged("BuildOrders");
            List = new ObservableCollection<DynamicBuildOrder>(List.OrderBy(x => x.Title));
        }

        public void AddBuildOrders(BuildOrderData buildOrderData)
        {
            var bos = new BuildOrderGroup(buildOrderData).BuildOrders;
            for (int i = 0; i < bos.Count; i++)
            {
                List.Add(bos[i]);
            }

            List = new ObservableCollection<DynamicBuildOrder>(List.OrderBy(x => x.Title));
            //OnPropertyChanged("BuildOrders");
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private DynamicBuildOrder _Current;
        public DynamicBuildOrder Current
        {
            get { return _Current; }

            private set {
                if (Current != null)
                {
                    _Current.IsSelected = false;
                }
                _Current = value;
                if (value != null)
                {
                    value.IsSelected = true;
                }
                OnPropertyChanged("Current");
            }
        }

        public DynamicBuildOrder Next()
        {
            int index = List.IndexOf(Current);
            if (List.Count == 0 || index + 1 >= List.Count || index == -1)
            {
                if (List.Count > 0)
                {
                    Current = List[0];
                }
                else
                {
                    Current = null;
                }
            }
            else
            {
                Current = List[index + 1];
            }
            return Current;
        }

        public DynamicBuildOrder Previous()
        {
            int index = List.IndexOf(Current);
            if (List.Count == 0 || index <= 0)
            {
                if (List.Count > 0)
                {
                    Current = List[List.Count - 1];
                }
                else
                {
                    Current = null;
                }
            }
            else {
                Current = List[index - 1];
            }
            return Current;
        }

        public DynamicBuildOrder SetCurrentBuildOrder(DynamicBuildOrder dynamicBuildOrder)
        {
            if (List.Contains(dynamicBuildOrder))
            {
                Current = dynamicBuildOrder;
            }
            else
            {
                Current = null;
            }
            return Current;
        }

        public int GetCurrentId()
        {

            if(Current == null)
            {
                return -1;
            }
            return List.IndexOf(Current);
        }
    }
}
