using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace EmojiFinder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<EmojItem> Emojis { get; set; } = new ObservableCollection<EmojItem>();
        public ICollectionView CollectionView { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                Task.Run(() =>
                {
                    var jsonBytes = Properties.Resources.emoji_en_US;
                    var json = Encoding.UTF8.GetString(jsonBytes);

                    var emojis = fastJSON.JSON.ToObject<Dictionary<string, string[]>>(json);
                    foreach (var e in emojis)
                    {
                        Emojis.Add(new EmojItem
                        {
                            Keywords = e.Value,
                            Emoji = e.Key
                        });
                    }
                }).ContinueWith(t =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        CollectionView = CollectionViewSource.GetDefaultView(Emojis);
                        lc.ItemsSource = CollectionView;
                        borderMask.Visibility = Visibility.Collapsed;
                    });
                });
            };
        }

        private void Copy(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Emoji.Wpf.TextBlock).DataContext is EmojItem item)
            {
                try
                {
                    Clipboard.SetText(item.Emoji);
                    txtMsg.Text = $"{item.Emoji} 已复制";
                    (this.FindResource("StoryMsg") as Storyboard).Begin();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void tbSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var keyword = tbSearch.Text;
            CollectionView.Filter = new Predicate<object>((item) =>
            {
                return ((EmojItem)item).Keyword.Contains(keyword);
            });
        }

        private void CommandBinding_ClearInput_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            tbSearch.Clear();
        }

        private void CommandBinding_ClearInput_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
