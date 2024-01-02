using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pięciolinia
{
    public partial class MainWindow : Window
    {

        //elementy które są nutami
        private UIElement selectedElement; 

        // lista z danymi dla każdej nuty
        private List<ElementInfo> elementInfoList; 

        //tablica przechowująca nuty/pauzy (a dokladnie ich grafiki)
        private string[] sharedImagePaths = { "/Images/calanuta.png", "/Images/polnuta.png", "/Images/cwiercnuta.png", "/Images/osemkapojedynczo.png", "/Images/szesnastka.png" };

        public MainWindow()
        {
            //inizjalizacja
            InitializeComponent();
            InitializeElements();

            //wybranie pierwszego elementu
            SelectElement(elementInfoList[0].Element);
        }

        //inicjalizowanie elementów/nut dodając je do tablicy
        private void InitializeElements()
        {
            // tablica do której dodaje się nuty
            elementInfoList = new List<ElementInfo>
            {
                new ElementInfo { Element = elementImage1 },
                new ElementInfo { Element = elementImage2 },
                // dodatkowe elementy >> here <<
            };


            //indexy obrazów poszczególnych elementów
            foreach (var elementInfo in elementInfoList)
            {
                elementInfo.ImagePaths = sharedImagePaths;
                elementInfo.CurrentImageIndex = 0;
                elementInfo.UpDownValue = 6; // 6 to wartość początkowa >> DO UZGODNIENIA <<
            }
        }


        //wybieranie nuty za pomocą klawiszy lewo/prawo
        private void SelectElement(UIElement element)
        {
            // odznacza aktywny element
            if (selectedElement != null)
            {
                // jakieś wizualne pierdoły można tu dodać
            }

            // wybranie nowego elementu
            selectedElement = element;
            // znowu wizualne bzdety
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

        // podmiana grafiki danego elementu
        private void ChangeImage(int direction)
        {
            // pobranie danych aktualnego elementu
            ElementInfo selectedElementInfo = elementInfoList.Find(info => info.Element == selectedElement);

            if (selectedElementInfo != null)
            {
                // wyciągnięcie indexu aktualnej grafiki elementu
                selectedElementInfo.CurrentImageIndex = (selectedElementInfo.CurrentImageIndex + direction + selectedElementInfo.ImagePaths.Length) % selectedElementInfo.ImagePaths.Length;

                if (selectedElement is Image image)
                {
                    // podmiana grafiki na adekwatną
                    image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(selectedElementInfo.ImagePaths[selectedElementInfo.CurrentImageIndex], UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void ChangeUpDownValue(int direction)
        {
            // pobranie danych aktualnego elementu
            ElementInfo selectedElementInfo = elementInfoList.Find(info => info.Element == selectedElement);

            if (selectedElementInfo != null)
            {
                // zmiana wartości zmiennej mówiącej o pozycji na pięciolini (góra/dół)
                selectedElementInfo.UpDownValue += direction;

                // potencjalne dostanie się do pozycji danej nuty na pięciolini (work in progress)
                MessageBox.Show($"Up/Down value for the selected element: {selectedElementInfo.UpDownValue}");
            }
        }


        // mówi samo za siebie
        private class ElementInfo
        {
            public UIElement Element { get; set; }
            public string[] ImagePaths { get; set; }
            public int CurrentImageIndex { get; set; }
            public int UpDownValue { get; set; }
        }


        //logika strzałek/klawiszy
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                case Key.Left:
                    // wybierz element do lewej
                    int currentIndex = elementInfoList.FindIndex(info => info.Element == selectedElement);
                    int newIndex = (currentIndex - 1 + elementInfoList.Count) % elementInfoList.Count;
                    SelectElement(elementInfoList[newIndex].Element);
                    break;

                case Key.D:
                case Key.Right:
                    // wybierz element do prawej
                    currentIndex = elementInfoList.FindIndex(info => info.Element == selectedElement);
                    newIndex = (currentIndex + 1) % elementInfoList.Count;
                    SelectElement(elementInfoList[newIndex].Element);
                    break;

                case Key.W:
                case Key.Up:
                    //przesunięcie nuty wyżej
                    if (Grid.GetRow(selectedElement) - 1 != -1)
                    {
                        Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) - 1);
                        ChangeUpDownValue(1);
                    }
                    break;

                case Key.S:
                case Key.Down:
                    //przesunięcie nuty niżej
                    if (Grid.GetRow(selectedElement) - 1 != 10)
                    {
                        Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) + 1);
                        ChangeUpDownValue(-1);
                    }
                    break;

                case Key.E:
                case Key.OemPlus:
                    // zwiększenie indexu obrazu o jeden
                    ChangeImage(1);
                    break;

                case Key.Q:
                case Key.OemMinus:
                    // zmniejszenie indexu obrazu o jeden
                    ChangeImage(-1);
                    break;
            }
        }

        //walidacja textboxa na takt za pomocą regexa

            //        ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⣴⣶⣿⣿⣿⠳⣦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⣻⣿⣿⠠⢼⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⡿⢿⣭⠽⠿⣿⠀⢳⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⡋⣷⠿⠀⢀⣿⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⣀⣠⠤⠶⢺⢦⣄⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⠁⠂⢀⢠⡌⢹⠀⢸⠀⠀⣀⣤⡴⠖⠒⠋⠉⠀⠀⠀⢸⡀⠙⣧⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⡖⢺⣌⠉⢸⣿⣶⢾⡛⣿⣅⣿⣷⣶⠠⢤⡀⠀⠀⠀⢸⡇⠈⣏⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣤⣿⡟⢛⣿⣶⣾⣿⣿⡀⠸⣿⠟⠟⢿⡏⣦⣼⡆⠀⠀⢀⣸⡇⠀⣿⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⣀⣠⣴⣶⣾⢿⢹⡏⣉⠉⣻⣿⠈⣿⡶⠰⡷⢻⠏⠀⡇⣼⣴⣿⣿⣷⣶⣿⣿⣿⣿⣦⣿⠀⠀⠀⠀⠀⠀
            //⢀⣠⣤⡴⣶⡿⣏⣿⣿⣿⣿⣿⣿⡇⠁⠘⣻⡟⠘⣿⣧⣰⣷⣾⣼⣷⣿⣿⣿⣿⣿⣿⣿⡿⠿⠛⠛⠋⠁⠀⠀⠀⠀⠀⠀
            //⢸⡟⣾⣷⣥⣠⣋⠻⢿⣿⡋⣿⢸⣇⣴⣾⡿⢄⢠⡿⣿⣿⣿⣿⣿⣿⠿⡟⠛⠉⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⢸⡷⡏⠃⣄⣽⣿⣿⣿⣿⣿⣿⣿⣿⣿⠹⣿⣼⡾⣟⣿⣿⢿⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⢸⣇⣧⣦⣿⣿⣿⣿⣿⣿⣿⣿⠿⠿⣿⠀⠀⣸⣇⣹⣿⠁⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠙⠻⣿⣿⣿⠿⠛⠛⠋⠉⠁⠀⠀⠀⣿⠀⠀⣾⣏⠉⢸⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⠿⡷⠾⠛⠙⠓⠾⢦⣼⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣿⣿⣧⣶⣾⣿⣤⣤⠄⠀⠉⠳⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣏⣙⣿⣿⣿⣿⣶⡖⠀⠀⠈⠲⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣾⣿⣭⠟⠛⠿⣯⡈⢻⣿⣿⣧⡀⠀⠀⠀⠹⣆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⣿⣿⣦⣀⣏⣦⢹⣿⣿⣿⣿⡯⠀⠀⠀⠀⢸⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⣿⣿⣏⠀⢉⣽⢻⣿⣿⣿⣿⣷⡀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⣿⣿⡿⡿⠿⣿⣿⣿⣿⢿⣿⣯⣳⡀⠀⠀⠀⢹⡄⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⣿⣦⣿⣿⠿⣯⣽⣿⠘⣿⣿⣿⣿⣗⠄⠀⠈⠛⠻⣿⡷⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⡏⠀⣿⡿⣿⠀⢿⣿⣿⣿⢿⠃⠀⠀⠀⠀⣇⣷⡸⡟⠶⣤⡀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⠈⣾⠏⠀⣿⠀⢸⣿⠏⠙⢾⣄⠀⠀⠀⠀⡿⣿⣇⣧⠀⠀⠉⠳⢤⡄⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⣿⢨⠘⠀⠀⣾⠀⢸⣿⣷⣄⣀⠙⢷⣤⣦⣠⡇⣿⡟⢻⠀⠀⠀⠀⠈⢻⠶⣤⣀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣻⠀⠀⠀⣛⠀⢸⠉⠻⠿⣿⣶⣀⠙⢾⣿⠁⣿⣿⠘⣇⠀⠀⠀⠀⠈⣷⠀⠁
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⡆⠀⠀⠀⢹⡄⢸⠀⠀⠀⠨⣿⣿⣿⣾⣿⠘⣿⣿⠀⠋⠀⠀⠀⠀⠀⠛⠃⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⠿⠇⠀⠀⠀⢸⡇⠸⠄⠀⠀⣸⣿⣿⣿⣿⢃⣈⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠾⠇⠀⠀⠀⠀⢼⣿⣿⣿⣷⣾⣿⣿⣿⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⡏⢻⣧⣩⣽⣿⣿⣿⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣬⣿⣿⠃⠈⢿⣿⠿⠀⠀⠀⠀⠀⠀⠀⠀⠀
            //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⠟⠀⠀⠀⠸⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
        private void TactValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9/]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}