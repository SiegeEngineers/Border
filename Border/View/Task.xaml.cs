using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Border.View
{
    /// <summary>
    /// Interaction logic for Task.xaml
    /// </summary>
    public partial class Task : ContentControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(Task), new PropertyMetadata(false));

        public bool IsCompact
        {
            get { return (bool)GetValue(IsCompactProperty); }
            set { SetValue(IsCompactProperty, value); }
        }

        public static readonly DependencyProperty IsCompactProperty =
            DependencyProperty.Register("IsCompact", typeof(bool), typeof(Task), new PropertyMetadata(false));

        public bool IsVertical
        {
            get { return (bool)GetValue(IsVerticalProperty); }
            set { SetValue(IsVerticalProperty, value); }
        }

        public static readonly DependencyProperty IsVerticalProperty =
            DependencyProperty.Register("IsVertical", typeof(bool), typeof(Task), new PropertyMetadata(false));

        public string Title
        {
            get { return base.GetValue(TitleProperty) as string; }
            set { base.SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
          DependencyProperty.Register("Title", typeof(string), typeof(Task), null);

        public string Description
        {
            get { return base.GetValue(DescriptionProperty) as string; }
            set { base.SetValue(DescriptionProperty, value); }
        }
        public static readonly DependencyProperty DescriptionProperty =
          DependencyProperty.Register("Description", typeof(string), typeof(Task), null);

        public string Time
        {
            get { return base.GetValue(TimeProperty) as string; }
            set { base.SetValue(TimeProperty, value); }
        }
        public static readonly DependencyProperty TimeProperty =
          DependencyProperty.Register("Time", typeof(string), typeof(Task), null);
    }
}