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
    public partial class MainWindow : Window
    {

        private UIElement selectedElement;
        private UIElement[] elements;

        public MainWindow()
        {
            InitializeComponent();
            InitializeElements();
            SelectElement(elements[0]);
            SelectElement(elementImage1);
            this.MinWidth = 920;
            this.MinHeight = 600;

        }

        private void TactValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9/]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void InitializeElements()
        {
            // Add all your Image controls to the array
            elements = new UIElement[] { elementImage1, elementImage2 };
        }

        private void SelectElement(UIElement element)
        {
            // Deselect the currently selected element
            if (selectedElement != null)
            {
                // Adjust any visual changes as needed
            }

            // Select the new element
            selectedElement = element;
            // Highlight the selected element, adjust any visual changes as needed
        }

        private void AddColumnButton_Click(object sender, RoutedEventArgs e)
        {
            AddColumnToGrid();
        }

        private void AddColumnToGrid()
        {
            ColumnDefinition columnDefinition = new ColumnDefinition();
            mainGrid.ColumnDefinitions.Add(columnDefinition);

            // Add content to the new column if needed
            // For example, add a button in the new column
            Button newButton = new Button();
            newButton.Content = "New Button";
            Grid.SetColumn(newButton, mainGrid.ColumnDefinitions.Count - 1); // Set the button to the last column
            mainGrid.Children.Add(newButton);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            double step = 5; // Set the step size for movement

            switch (e.Key)
            {
                case Key.Left:
                    // Move the selection to the previous element
                    int currentIndex = Array.IndexOf(elements, selectedElement);
                    int newIndex = (currentIndex - 1 + elements.Length) % elements.Length;
                    SelectElement(elements[newIndex]);
                    break;

                case Key.Right:
                    // Move the selection to the next element
                    currentIndex = Array.IndexOf(elements, selectedElement);
                    newIndex = (currentIndex + 1) % elements.Length;
                    SelectElement(elements[newIndex]);
                    break;

                case Key.Up:
                    Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) - 1);
                    break;

                case Key.Down:
                    Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) + 1);
                    break;
            }
        }

    }
}