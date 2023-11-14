using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pięciolinia
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private void RectangleMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(Red, Red, DragDropEffects.Move);
            }
        }

        private void CanvasDrop(object sender, DragEventArgs e)
        {

        }

        private void CanvasDragOver(object sender, DragEventArgs e)
        {
            Point dropPositione = e.GetPosition(Canvas);

            Canvas.SetLeft(Red, dropPositione.X - 25);
            Canvas.SetTop(Red, dropPositione.Y - 25);
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
