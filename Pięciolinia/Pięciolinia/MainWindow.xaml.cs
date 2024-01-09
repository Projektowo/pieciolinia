using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Pięciolinia
{
    public partial class MainWindow : Window
    {

        //elementy które są nutami
        private UIElement selectedElement;

        // lista z danymi dla każdej nuty
        private List<ElementInfo> elementInfoList;

        // zmienna blokujaca mozliwosc poruszania nutami, kiedy żaden takt nie istnieje
        private bool tactControl = false;

        //tablica przechowująca nuty/pauzy (a dokladnie ich grafiki)
        private string[] sharedImagePaths = { "/Images/calanuta.png", "/Images/polnuta.png", "/Images/cwiercnuta.png", "/Images/osemkapojedynczo.png", "/Images/szesnastka.png" };

        public MainWindow()
        {
            //inizjalizacja
            InitializeComponent();
            InitializeElements();


            //wybranie pierwszego elementu
            //SelectElement(elementInfoList[0].Element);

            // auto focus textboxa
            inputTextBox.Focus();
        }

        //inicjalizowanie elementów/nut dodając je do tablicy
        private void InitializeElements()
        {
            // tablica do której dodaje się nuty
            elementInfoList = new List<ElementInfo>
            {
                //new ElementInfo { Element = elementImage1 },
                //new ElementInfo { Element = elementImage2 },
                // dodatkowe elementy >> here <<
            };


            ////indexy obrazów poszczególnych elementów
            //foreach (var elementInfo in elementInfoList)
            //{
            //    elementInfo.ImagePaths = sharedImagePaths;
            //    elementInfo.CurrentImageIndex = 0;
            //    elementInfo.UpDownValue = 6; // 6 to wartość początkowa >> DO UZGODNIENIA <<
            //}
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
            // Todo: dowiedziec sie w jaki sposob implementuje sie rzeczy typu Effects, bo internet nie wie
        }


        //przycisk do dodawania taktów
        private void AddColumnButton_Click(object sender, RoutedEventArgs e)
        {
            //AddColumnsToGrid(4); // 4 jak tempo to np 4/X

            // pobranie danych z textboxa
            string userInput = inputTextBox.Text;
            ProcessUserInput(userInput);


        }


        private void ProcessUserInput(string userInput)
        {
            // podział cyfr
            string[] inputParts = userInput.Split('/');

            if (inputParts.Length == 2)
            {
                // pierwsza cyfra
                if (int.TryParse(inputParts[0], out int columnCount))
                {
                    if (columnCount <= 0)
                    {
                        MessageBox.Show("Number of columns must be greater than zero.");
                        return;
                    }

                    // druga cyfra
                    if (int.TryParse(inputParts[1], out int imageIndex))
                    {
                        if (imageIndex <= 0)
                        {
                            MessageBox.Show("Image index must be greater than zero.");
                            return;
                        }

                        // sprawdzenie czy nie wykracza poza ilość nut
                        if (imageIndex > 5)
                        {
                            MessageBox.Show($"Nie ma tylu nut bruh.");
                            return;
                        }

                        // jak wszystko buja, to zwraca zmienne
                        tactControl = true;
                        AddColumnsToGrid(columnCount, imageIndex - 1);

                        // wyłączenie textboxa
                        inputTextBox.IsEnabled = false;
                        inputTextBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#212121"));
                    }
                    else
                    {
                        MessageBox.Show("Nieprawidłowy rodzaj nuty. Proszę wprowadź poprawny numer.");
                    }
                }
                else
                {
                    MessageBox.Show("Nieprawidłowy tempo. Proszę wprowadź poprawny numer.");
                }
            }
            else
            {
                MessageBox.Show("nieprawidłowy format. proszę wprowadź poprawne dane w postaci 'X/Y'.");
            }
        }

        // mięso dodawania taktu
        private void AddColumnsToGrid(int columnCount, int imageIndex)
        {
            for (int i = 0; i < columnCount; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                mainGrid.ColumnDefinitions.Add(columnDefinition);

                // tworzenie elementu ze zdjęciem
                Image newImage = new Image();
                newImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(sharedImagePaths[0], UriKind.RelativeOrAbsolute)); // Use the first image path as default
                Grid.SetColumn(newImage, mainGrid.ColumnDefinitions.Count - 1);

                // tworzenie tego w określonym wierszu grida
                Grid.SetRow(newImage, 6); // jako że to indexy, to 5 tak naprawdę jest szóstką
                mainGrid.Children.Add(newImage);

                // przypisanie wartości do elementu
                ElementInfo newElementInfo = new ElementInfo { Element = newImage, ImagePaths = sharedImagePaths, CurrentImageIndex = 0, UpDownValue = 6 };
                elementInfoList.Add(newElementInfo);

                // zwiększanie indexu pozycji
                newElementInfo.CurrentImageIndex = (newElementInfo.CurrentImageIndex + 1) % newElementInfo.ImagePaths.Length;
                newElementInfo.CurrentType = (char)(newElementInfo.CurrentImageIndex - 1 + 97);
                newImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(newElementInfo.ImagePaths[newElementInfo.CurrentImageIndex], UriKind.RelativeOrAbsolute));
            }

            // ustawienie indexu grafiki
            foreach (var info in elementInfoList.Skip(elementInfoList.Count - columnCount))
            {
                info.CurrentImageIndex = imageIndex % info.ImagePaths.Length;

                // sprawdzenie czy element ma sourca
                if (info.Element is Image imageElement)
                {
                    imageElement.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(info.ImagePaths[info.CurrentImageIndex], UriKind.RelativeOrAbsolute));
                }
            }


            // WEIRD STUFF - wybranie każdego elementu i przesunięcie go o 1 + i -, żeby zaktualizować typ nuty
            int x = 0;
            foreach (var ele in elementInfoList)
            {
                SelectElement(elementInfoList[x].Element);
                x++;
                ChangeImage(1);
                ChangeImage(-1);
            }

            // wybranie pierwszego elementu z dodanego taktu
            if (elementInfoList.Count >= columnCount)
            {
                SelectElement(elementInfoList[elementInfoList.Count - columnCount].Element);
            }
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
                selectedElementInfo.CurrentType = (char)(selectedElementInfo.CurrentImageIndex + 97);
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

                int selectedElementRow = selectedElementInfo.UpDownValue;
                char selectedElementType = selectedElementInfo.CurrentType;
                //Console.WriteLine($"{selectedElementType}{selectedElementRow}");
            }
        }

        private int GetNoteIntValue(char note)
        {
            int value = 0;

            switch (note)
            {
                case 'a':
                    value = 1;
                    break;

                case 'b':
                    value = 2;
                    break;

                case 'c':
                    value = 3;
                    break;

                case 'd':
                    value = 4;
                    break;

                case 'e':
                    value = 5;
                    break;
            }

            return value;
        }

        private void SaveToTxt()
        {
            //Pobieranie wszystkich danych
            string data = $"{inputTextBox.Text}\n";

            foreach (var ele in elementInfoList)
            {
                data += $"{ele.CurrentType}{ele.UpDownValue}\n";
            }

            //Console.WriteLine(data);

            //Zapisanie danych przy pomocy SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, data);
        }

        private void LoadFromTxt()
        {
            //Odczytanie danych przy pomocy OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text file (*.txt)|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Clear();

                var fileStream = openFileDialog.OpenFile();
                string loadedTact = "";
                string loadedNotes = "";


                bool catchTact = true;
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    if (catchTact)
                    {
                        loadedTact = reader.ReadLine();
                        catchTact = false;
                    }
                    loadedNotes = reader.ReadToEnd();
                }

                //Console.WriteLine(loadedTact);
                //Console.WriteLine(loadedNotes);

                //Emuluje reczne dodanie taktu przez uzytkownika
                inputTextBox.Text = loadedTact;
                ProcessUserInput(loadedTact);

                //Zapamietac - co trzeci element loadedNotes to \n

                var numberOfNotes = loadedNotes.Length / 3;
                //Console.WriteLine(loadedNotes);
                //Console.WriteLine(numberOfNotes);

                //6 - startowa lokacja nuty
                for (int i = 0; i < numberOfNotes; i++)
                {
                    SelectElement(elementInfoList[i].Element);
                    int tempNoteVal = GetNoteIntValue(loadedNotes[3 * i]);
                    int tempUpDown = loadedNotes[3 * i + 1];
                    int trueTact = loadedTact[2]; 

                    /* nie dziala :(
                    while (tempNoteVal > trueTact)
                    {
                        ChangeImage(-1);
                        tempNoteVal--;
                    }
                    while (tempNoteVal < trueTact)
                    {
                        ChangeImage(1);
                        tempNoteVal++;
                    }

                    while (tempUpDown > trueTact)
                    {
                        ChangeImage(-1);
                        ChangeUpDownValue(-1);
                    }
                    while (tempUpDown < trueTact)
                    {
                        ChangeImage(1);
                        ChangeUpDownValue(1);
                    }
                    */
                }
            }

            if (!tactControl) tactControl = true;
        }


        // mówi samo za siebie
        private class ElementInfo
        {
            public UIElement Element { get; set; }
            public string[] ImagePaths { get; set; }
            public int CurrentImageIndex { get; set; }
            public int UpDownValue { get; set; }
            public char CurrentType { get; set; }
        }


        //logika strzałek/klawiszy
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (tactControl)
            {
                switch (e.Key)
                {
                    case Key.A:
                        // wybierz element do lewej
                        int currentIndex = elementInfoList.FindIndex(info => info.Element == selectedElement);
                        int newIndex = (currentIndex - 1 + elementInfoList.Count) % elementInfoList.Count;
                        SelectElement(elementInfoList[newIndex].Element);
                        break;

                    case Key.D:
                        // wybierz element do prawej
                        currentIndex = elementInfoList.FindIndex(info => info.Element == selectedElement);
                        newIndex = (currentIndex + 1) % elementInfoList.Count;
                        SelectElement(elementInfoList[newIndex].Element);
                        break;

                    case Key.W:
                        //przesunięcie nuty wyżej
                        if (Grid.GetRow(selectedElement) - 1 != -1)
                        {
                            Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) - 1);
                            ChangeUpDownValue(1);
                        }
                        break;

                    case Key.S:
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
        //⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⠟⠀⠀⠀⠸⠇
        //
        //
        // https://www.youtube.com/watch?v=Zttt_rv87no⠀⠀⠀⠀⠀⠀
        // bruh
        // https://www.youtube.com/watch?v=Z3J_MCbwaJ0
        // bruh
        // https://www.youtube.com/watch?v=AINfHRXx1kQ
        private void TactValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9/]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Clear()
        {
            inputTextBox.Text = "";
            inputTextBox.IsEnabled = true;
            mainGrid.Children.Clear();
            mainGrid.ColumnDefinitions.Clear();

            ColumnDefinition columnDefinition = new ColumnDefinition();
            mainGrid.ColumnDefinitions.Add(columnDefinition);

            tactControl = false;
        }
        private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (tactControl)
            {
                SaveToTxt();
            }
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadFromTxt();
        }
    }
}