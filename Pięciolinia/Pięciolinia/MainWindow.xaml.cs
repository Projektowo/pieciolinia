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

        //elementy które są nutami
        private UIElement selectedElement;
        private UIElement[] elements;

        public MainWindow()
        {
            InitializeComponent();
            InitializeElements();
            SelectElement(elements[0]);
            SelectElement(elementImage1);

        }


        //walidacja textboxa na takt
        private void TactValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9/]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //inicjalizowanie elementów/nut dodając je do tablicy
        private void InitializeElements()
        {
            // Add all your Image controls to the array
            elements = new UIElement[] { elementImage1, elementImage2 };
        }


        //wybieranie nuty za pomocą klawiszy lewo/prawo
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


        //przycisk do dodawania taktów
        private void AddColumnButton_Click(object sender, RoutedEventArgs e)
        {
            AddColumnToGrid();
        }


        //póki co jest tylko jedna kolumna zamiast całego taktu
        private void AddColumnToGrid()
        {
            ColumnDefinition columnDefinition = new ColumnDefinition();
            mainGrid.ColumnDefinitions.Add(columnDefinition);

            //przycisk do wizualizacji czegokolwiek
            Button newButton = new Button();
            newButton.Content = "New Button";
            Grid.SetColumn(newButton, mainGrid.ColumnDefinitions.Count - 1);
            mainGrid.Children.Add(newButton);
        }


        //logika strzałek/klawiszy
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    // zmiana nuty na nutę po lewej
                    int currentIndex = Array.IndexOf(elements, selectedElement);
                    int newIndex = (currentIndex - 1 + elements.Length) % elements.Length;
                    SelectElement(elements[newIndex]);
                    break;

                case Key.Right:
                    // zmiana nuty na nutę po prawej
                    currentIndex = Array.IndexOf(elements, selectedElement);
                    newIndex = (currentIndex + 1) % elements.Length;
                    SelectElement(elements[newIndex]);
                    break;

                case Key.Up:
                    //przesunięcie nuty wyżej
                    Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) - 1);
                    break;

                case Key.Down:
                    //przesunięcie nuty niżej
                    Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) + 1);
                    break;
            }
        }

    }
}