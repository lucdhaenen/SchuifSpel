using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchuifSpel
{
    /// <summary>
    /// Interaction logic for SchuifSpelWindow.xaml
    /// </summary>
    public partial class SchuifSpelWindow : Window
    {

        public SchuifSpelWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Shuffle();
        }

        private void Check()
        {
            int irij, ikolom, grij, gkolom;
            int aantalfout = 0;
            foreach (Image stukje in puzzelGrid.Children)
            {
                irij = Convert.ToInt16(stukje.Name.Substring(4, 1));
                ikolom = Convert.ToInt16(stukje.Name.Substring(5, 1));
                grij = Grid.GetRow(stukje);
                gkolom = Grid.GetColumn(stukje);
                if ((irij != grij) || (ikolom != gkolom))
                {
                    aantalfout++;
                }
            }
            if (aantalfout == 0)
                Oplossing();
        }

        private void OplossingButton_Click(object sender, RoutedEventArgs e)
        {
            Oplossing();
        }

        private void Oplossing()
        {
            puzzelGrid.Children.Clear();
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    Image stuk = new Image();
                    BitmapImage bi = new BitmapImage(new Uri(@"images/vdab" + r + k + ".jpg", UriKind.Relative));
                    stuk.Name = "stuk" + r + k;
                    stuk.Source = bi;
                    zetImage(r, k, stuk);
                }
            }
        }

        private void zetImage(int rij, int kolom, Image zetstuk)
        {
            Image stuk = new Image();
            stuk = zetstuk;
            Grid.SetColumn(stuk, kolom);
            Grid.SetRow(stuk, rij);
            if (stuk.Name == "stuk33")
            {
                stuk.Drop += puzzelGrid_Drop;
                stuk.AllowDrop = true;
            }
            else
            {
                stuk.MouseMove += stuk_MouseMove;
                AllowDrop = false;
            }
            puzzelGrid.Children.Add(stuk);
        }

        private void Shuffle()
        {
            puzzelGrid.Children.Clear();
            int[,] checken = new int[4, 4];
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    checken[r, k] = 0;
                }
            }
            checken[3, 3] = 1;

            Random rnd = new Random();
            int rij, kolom;
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    if (k < 3 || r < 3)
                    {
                        do
                        {
                            rij = rnd.Next(0, 4);
                            kolom = rnd.Next(0, 4);
                        } while (checken[rij, kolom] == 1);

                        checken[rij, kolom] = 1;
                        Image stuk = new Image();
                        BitmapImage bi = new BitmapImage(new Uri(@"images/vdab" + r + k + ".jpg", UriKind.Relative));
                        stuk.Name = "stuk" + r + k;
                        stuk.Source = bi;
                        zetImage(rij, kolom, stuk);
                    }
                }
            }

            Image leegstuk = new Image();
            BitmapImage bl = new BitmapImage(new Uri(@"images/leeg33.jpg", UriKind.Relative));
            leegstuk.Name = "stuk33";
            leegstuk.Source = bl;
            zetImage(3, 3, leegstuk);
            droprij = 3;
            dropkolom = 3;
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Shuffle();
        }

        private int sleeprij, sleepkolom, droprij, dropkolom;

        private void stuk_MouseMove(object sender, MouseEventArgs e)
        {
            Image stuk = (Image)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                sleeprij = Grid.GetRow(stuk);
                sleepkolom = Grid.GetColumn(stuk);
                DataObject sleepStuk = new DataObject("hetStuk", stuk);
                DragDrop.DoDragDrop(stuk, sleepStuk, DragDropEffects.Move);
            }
        }

        private void puzzelGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("hetStuk"))
            {
                if (geldig())
                {
                    Image gesleepteStuk = (Image)e.Data.GetData("hetStuk"); // het te verplaatsen stuk
                    Image dropStuk = (Image)sender; // de lege plaats
                    droprij = Grid.GetRow(dropStuk); // rij die leeg is
                    dropkolom = Grid.GetColumn(dropStuk); // kolom die leeg is            
                    puzzelGrid.Children.Remove(gesleepteStuk); // verwijder te verplaatsen stuk 
                    puzzelGrid.Children.Remove(dropStuk); // verwijder blanco stuk
                    zetImage(droprij, dropkolom, gesleepteStuk); // zet het stuk op de juiste plaats
                    zetImage(sleeprij, sleepkolom, dropStuk); // zet het blanco stuk op de nieuwe plaats
                    droprij = sleeprij; // nieuwe beschikbare droprij
                    dropkolom = sleepkolom; // nieuwe beschikbare kolom
                }                
            }            
        }

        private Boolean geldig()
        {
            if (((sleeprij + 1 == droprij) || (sleeprij - 1 == droprij)) && (sleepkolom == dropkolom))
            {
                return true;
            }                
            else
            {
                if (((sleepkolom + 1 == dropkolom) || (sleepkolom - 1 == dropkolom)) && (sleeprij == droprij))
                {
                    return true;
                }
            }                    
            return false;
        }
    }
}
