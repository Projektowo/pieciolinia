﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

        private bool isPlaying = false;

        //tablica przechowująca nuty/pauzy (a dokladnie ich grafiki)
        private string[] sharedImagePaths = { "/Resources/calanuta.png", "/Resources/polnuta.png", "/Resources/cwiercnuta.png", "/Resources/osemka.png", "/Resources/szesnastka.png" };

        int currentIndex;

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

            //// Unhighlight the previously selected element
            //if (selectedElement != null)
            //{
            //    if (selectedElement is Image selectedImage)
            //    {
            //        selectedImage.Effect = null; // Remove the effect on the previously selected element
            //    }
            //}

            // Select the new element
            selectedElement = element;

            // Highlight the selected element with a drop shadow effect
            if (selectedElement is Image selectedImage)
            {
                DropShadowEffect dropShadowEffect = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = 15,
                    Color = Colors.Yellow // You can set any color you prefer
                };

                selectedImage.Effect = dropShadowEffect;
            }
            else
            {
                // If the selected element is not an Image, you may need to adjust the handling
                // based on the actual type of the element.
                // For example, you could check for other types like Button, Rectangle, etc.
            }
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
                
                highlightElement();

            }

                int numberOfElements = elementInfoList.Count;
                currentIndex = numberOfElements - columnCount;
            
        }


        private void highlightElement()
        {
            foreach (var elementInfo in elementInfoList)
            {
                if (elementInfo.Element is Image image)
                {
                    // Remove the effect by setting it to null
                    image.Effect = null;
                }
            }

            ElementInfo selectedElementInfo = elementInfoList.Find(info => info.Element == selectedElement);
            // Assuming selectedElementInfo is the selected element
            if (selectedElementInfo != null && selectedElementInfo.Element is Image selectedImage)
            {
                // Create a DropShadowEffect
                DropShadowEffect shadowEffect = new DropShadowEffect
                {
                    ShadowDepth = 0.5,
                    BlurRadius = 15,
                    Color = Colors.DarkBlue,
                    Opacity = 1,
                    Direction = 315 // Change the direction based on your preference
                };

                // Apply the effect to the selected Image
                selectedImage.Effect = shadowEffect;
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
                if(ele.UpDownValue < 9)
                {
                    data += $"{ele.CurrentType}0{ele.UpDownValue+1}\n";
                }
                if(ele.UpDownValue >= 9)
                {
                    data += $"{ele.CurrentType}{ele.UpDownValue + 1}\n";
                }
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
                int numberOfNotes = 0;

                bool catchTact = true;

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    if (catchTact)
                    {
                        loadedTact = reader.ReadLine();
                        catchTact = false;
                    }
                    while (!reader.EndOfStream) {
                        numberOfNotes++;
                        loadedNotes += reader.ReadLine() + "\n";
                    }
                }

                //Console.WriteLine(loadedTact);
                //Console.WriteLine(loadedNotes);
                //Console.WriteLine(numberOfNotes);

                //Emuluje reczne dodanie taktu przez uzytkownika
                inputTextBox.Text = loadedTact;
                ProcessUserInput(loadedTact);

                //Zapamietac - co trzeci element loadedNotes to \n

                string[] getTact = loadedTact.Split('/');
                //Console.WriteLine(leftTact[0]);// <- ilosc nut na jeden takt

                //Dodawanie kolejnych taktow jesli jest wiecej niz 1
                int tempNoN = numberOfNotes;
                int NotesOnTact = Int32.Parse(getTact[0]);
                int fullTacts = 0;
                int forcedTacts = 0;

                while (tempNoN > NotesOnTact)
                {
                    ProcessUserInput(loadedTact);
                    tempNoN -= NotesOnTact;
                    fullTacts++;
                }

                if(tempNoN> 0)
                {
                    forcedTacts = fullTacts + 1;
                }
                else
                {
                    forcedTacts = fullTacts;
                }



                //Console.WriteLine(fullTacts); // <- ilosc pelnych taktow
                //Console.WriteLine(forcedTacts); // <- ilosc wymuszonych taktow

                //6 - startowa lokacja nuty

                for (int i = 0; i < numberOfNotes; i++)
                {
                    SelectElement(elementInfoList[i].Element);
                    int tempNoteVal = GetNoteIntValue(loadedNotes[4 * i]);

                    int tempFirstUD = (int)char.GetNumericValue(loadedNotes[4 * i + 1]);
                    int tempSecondUD = (int)char.GetNumericValue(loadedNotes[4 * i + 2]);


                    int tempUpDown = 0;

                    if (tempFirstUD == 0)
                    {
                        tempUpDown = tempSecondUD - 1;
                    }
                    else
                    {
                        tempUpDown = tempSecondUD + 10 - 1;
                    }

                    int trueTact = Int32.Parse(getTact[1]);
                    //Console.WriteLine($"{loadedNotes[4 * i]} {loadedNotes[4 * i + 1]} {getTact[1]}");
                    //Console.WriteLine($"{tempNoteVal} {tempUpDown} {trueTact}");

                    //Zmiana typu nuty
                    while (tempNoteVal > trueTact)
                    {
                        ChangeImage(1);
                        tempNoteVal--;
                    }
                    while (tempNoteVal < trueTact)
                    {
                        ChangeImage(-1);
                        tempNoteVal++;
                    }

                    //Zmiana pozycji nuty
                    while (tempUpDown > 6)
                    {
                        if (Grid.GetRow(selectedElement) - 1 != -1)
                        {
                            Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) - 1);
                            ChangeUpDownValue(1);
                        }
                        tempUpDown--;
                    }

                    while (tempUpDown < 6)
                    {
                        if (Grid.GetRow(selectedElement) - 1 != 11)
                        {
                            Grid.SetRow(selectedElement, Grid.GetRow(selectedElement) + 1);
                            ChangeUpDownValue(-1);

                        }
                        tempUpDown++;
                    }

                }
                //fullTacts; // <- ilosc pelnych taktow
                //forcedTacts; // <- ilosc wymuszonych taktow

                if (fullTacts != 0 & fullTacts < forcedTacts)
                {
                    int forcedNumberOfNotes = forcedTacts * NotesOnTact;
                    //Console.WriteLine(forcedNumberOfNotes);
                    //Console.WriteLine(numberOfNotes);

                    int tempdel = forcedNumberOfNotes - numberOfNotes;
                    int tempdelnon = numberOfNotes;
                    while(tempdel > 0)
                    {
                        currentIndex = tempdelnon;
                        Delete();
                        tempdelnon--;
                        tempdel--;
                    }
                }

            }

            if (!tactControl) tactControl = true;


            


            foreach (var elementInfo in elementInfoList)
            {
                if (elementInfo.Element is Image image)
                {
                    // Remove the effect by setting it to null
                    image.Effect = null;
                }
            }

            currentIndex = 0;
            SelectElement(elementInfoList[currentIndex].Element);
            highlightElement();
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
                        if (currentIndex == 0)
                        {
                            currentIndex = elementInfoList.Count - 1;
                            SelectElement(elementInfoList[currentIndex].Element);
                            highlightElement();
                        }
                        else
                        {
                            currentIndex--;
                            SelectElement(elementInfoList[currentIndex].Element);
                            highlightElement();
                        }
                            
                        break;

                    case Key.D:
                        // wybierz element do prawej
                        if (currentIndex == elementInfoList.Count-1)
                        {
                            currentIndex = 0;
                            SelectElement(elementInfoList[currentIndex].Element);
                            highlightElement();
                        }
                        else
                        {
                            currentIndex++;
                            SelectElement(elementInfoList[currentIndex].Element);
                            highlightElement();
                        }
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
                        if (Grid.GetRow(selectedElement) - 1 != 11)
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
            elementInfoList.Clear();
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

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            start_Btn_Click();
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                isPlaying = false;
                Console.WriteLine(elementInfoList);
            }
        }

        private void Delete()
        {
            if (elementInfoList.Count > 0)
            {
                int columnIndexToRemove = currentIndex;

                // Remove elements associated with the column
                foreach (var elementInfo in elementInfoList.ToArray())
                {
                    if (Grid.GetColumn(elementInfo.Element) == columnIndexToRemove)
                    {
                        // Remove any effects or highlights if applied
                        // ...

                        // Remove the element from the grid
                        mainGrid.Children.Remove(elementInfo.Element);

                        // Remove the element from the list
                        elementInfoList.Remove(elementInfo);
                    }
                }

                // Remove the column definition
                mainGrid.ColumnDefinitions.RemoveAt(columnIndexToRemove);

                // Update column indices for remaining elements
                foreach (var elementInfo in elementInfoList)
                {
                    int currentColumn = Grid.GetColumn(elementInfo.Element);

                    // Update the column index if it was after the removed column
                    if (currentColumn > columnIndexToRemove)
                    {
                        Grid.SetColumn(elementInfo.Element, currentColumn - 1);
                    }
                }

                if (currentIndex == 0)
                {
                    currentIndex = 0;
                }
                else
                {
                    currentIndex--;
                }

                if (elementInfoList.Count > 0)
                {
                    SelectElement(elementInfoList[currentIndex].Element);
                    highlightElement();
                }
            }
        }

        private void deleteNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private async void start_Btn_Click()
        {
            if (!isPlaying)
            {  
                isPlaying = true;
                Console.WriteLine(elementInfoList.Count);
                foreach (var note in elementInfoList)
                {
                    if(isPlaying == false)
                    {
                        break;
                    }
                    PlayAudio($"{note.UpDownValue+1}");

                    await Task.Delay(getDelayForNoteType(note.CurrentType));
                    //System.Threading.Thread.Sleep(getDelayForNoteType(note.CurrentType));

                }
                isPlaying = false;
                Console.WriteLine(elementInfoList);

            }
        }

        private int getDelayForNoteType(char currentType)
        {
            if (currentType == 'a')
            {
                return 64;
            }
            else if (currentType == 'b')
            {
                return 32;
            }
            else if (currentType == 'c')
            {
                return 16;
            }
            else if (currentType == 'd')
            {
                return 8;
            }
            else
            {
                return 4;
            }
        }

        static void PlayAudio(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            var stream = assembly.GetManifestResourceStream($"Pięciolinia.Resources.n{fileName}.wav");

            using (SoundPlayer player = new SoundPlayer(stream))
            {
                player.PlaySync();
            }
        }

        
    }
}