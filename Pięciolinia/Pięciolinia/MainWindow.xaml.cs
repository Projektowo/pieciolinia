using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        //private void RectangleMouseMove1(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        DragDrop.DoDragDrop(Red1, Red1, DragDropEffects.Move);
        //    }
        //}


        private void CanvasDrop(object sender, DragEventArgs e)
        {

        }

        //private void CanvasDragOver(object sender, DragEventArgs e)
        //{
        //    Point dropPositione = e.GetPosition(Canvas);

        //    Canvas.SetLeft(Red, dropPositione.X - 25);
        //    Canvas.SetTop(Red, dropPositione.Y - 25);
        //}

        private void CanvasDragOver(object sender, DragEventArgs e)
        {
            if (sender is Rectangle draggedRectangle)
            {
                Point dropPosition = e.GetPosition(MainCanvas);

                Canvas.SetLeft(draggedRectangle, dropPosition.X - (draggedRectangle.Width / 2));
                Canvas.SetTop(draggedRectangle, dropPosition.Y - (draggedRectangle.Height / 2));
            }
        }

        private bool isDragging = false;
        private Point offset;

        private void Canvas_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                if (e.Source is Rectangle draggedRectangle)
                {
                    draggedRectangle.DragEnter += Rectangle_DragEnter;
                    draggedRectangle.Stroke = Brushes.Blue;
                    e.Effects = DragDropEffects.Copy;
                }
            }
        }

        private void Rectangle_DragEnter(object sender, DragEventArgs e)
        {
            // Your DragEnter logic for individual rectangles
        }

        private void Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            offset = e.GetPosition((UIElement)sender);
            ((UIElement)sender).CaptureMouse();
        }

        private void Rectangle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(MainCanvas);
                double newX = currentPosition.X - offset.X;
                double newY = currentPosition.Y - offset.Y;

                Canvas.SetLeft((UIElement)sender, newX);
                Canvas.SetTop((UIElement)sender, newY);
            }
        }

        private void Rectangle_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
        }


        public MainWindow()
        {
            InitializeComponent();
            this.MinWidth = 920;
            this.MinHeight = 600;

        }

        private void TactValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9/]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}