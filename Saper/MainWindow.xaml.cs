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

namespace Saper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static int blocks = 25; // Размер поля int х int

        public static Random rnd = new Random(); // Рандом

        int flaging = 0; // Флажки
        int noHid = 0; // Кол-во нажатий
        Boolean bls = false; // Определение пустой ли блок или нет
       
        Label[,] lb = new Label[blocks,blocks]; // Блоки 
        Button[,] visb = new Button[blocks, blocks]; // Блоки
    
        //Кол-во мин
        static int valueMins = 100;
        

        int ns = 0; // Кол-во разгаданных блоков
        Label[] thMina = new Label[valueMins]; // Мины

        Button[] flegs = new Button[valueMins]; // Флаги

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Загрузка окна
        /// </summary>
        /// <param name="sender">Window</param>
        /// <param name="e">Loaded</param>
        private void LoadGame(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < blocks; i++ )
            {
                ColumnDefinition c = new ColumnDefinition();
                GamePane.ColumnDefinitions.Add(c);
                RowDefinition r = new RowDefinition();
                GamePane.RowDefinitions.Add(r);
            }

            PaneSize.Content = "Поле на : " + blocks + " x " + blocks;

            Mines.Content = "Кол-во мин : " + valueMins;

            PaneSize.BorderBrush = Brushes.Gray;
            PaneSize.BorderThickness = new Thickness(1);

            Mines.BorderBrush = Brushes.Gray;
            Mines.BorderThickness = new Thickness(1);
            //Заполнение поля
            for (int i = 0; i < blocks; i++)
            {
                for (int j = 0; j < blocks; j++)
                {
                    lb[i, j] = CreateLb();
                    visb[i,j] = new Button();
                    visb[i, j].Tag = lb[i, j];

                    Grid.SetColumn(lb[i, j], j);
                    Grid.SetRow(lb[i, j], i);
                    GamePane.Children.Add(lb[i, j]);

                    Grid.SetColumn(visb[i, j], j);
                    Grid.SetRow(visb[i, j], i);
                    GamePane.Children.Add(visb[i, j]);

                    visb[i,j].Click += ButClick;
                    visb[i,j].MouseRightButtonDown += SetFlag;
                }
            }

            //Мины
            for(int i = 0; i < valueMins; i++)
            {
                CreateMin();
            }
            
            for(int i = 0; i < valueMins; i++)
            {
                ContentLab(thMina[i]);
            }

            for (int i = 0; i < valueMins; i++)
            {
                lb[Grid.GetRow(thMina[i]), Grid.GetColumn(thMina[i])].Content = "";
                lb[Grid.GetRow(thMina[i]), Grid.GetColumn(thMina[i])].Tag = "Mina";
            }
        }

        /// <summary>
        /// Установка флага на объект
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">MouseRightDown</param>
        private void SetFlag(object sender, MouseButtonEventArgs e)
        {
            var seb = sender as Button;

            if (flaging >= valueMins - 1)
            {
                flaging = 0;
                bls = true;
            }
                           

            if(bls)
            {
                visb[Grid.GetRow(flegs[flaging]), Grid.GetColumn(flegs[flaging])].Background = Brushes.LightGray;
                flegs[flaging] = seb;
            }
            else
            {
                flegs[flaging] = seb;
            }

            if(seb.Background == new ImageBrush(new BitmapImage(new Uri("flag.jpg", UriKind.Relative))))
            {
                visb[Grid.GetRow(flegs[flaging]), Grid.GetColumn(flegs[flaging])].Background = Brushes.LightGray;
                flaging--;
            }
            else 
                seb.Background = new ImageBrush(new BitmapImage(new Uri("flag.jpg", UriKind.Relative)));            

            flaging++;
        }

        /// <summary>
        /// Нажатие на объект
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private void ButClick(object sender, RoutedEventArgs e)
        {
            var sen = sender as Button;

            sen.Visibility = Visibility.Hidden;

            if (lb[Grid.GetRow(sen), Grid.GetColumn(sen)].Tag.Equals("NoMina"))
            {
                CheckThisMin(lb[Grid.GetRow(sen), Grid.GetColumn(sen)]);
            }

            // Луз (Всё чётко)
            else if (sen.Tag.Equals("Mina"))
            {
                lb[Grid.GetRow(sen), Grid.GetColumn(sen)].BorderBrush = Brushes.Red;
                lb[Grid.GetRow(sen), Grid.GetColumn(sen)].BorderThickness = new Thickness(3);
                NoHiddesBlock();
                MessageBox.Show("Ты проиграл :(");
                Close();
            }
           
             CheckWins();
        }

        /// <summary>
        /// Показываем все блоки
        /// </summary>
        private void NoHiddesBlock()
        {
            for (int i = 0; i < blocks; i++)
            {
                for (int j = 0; j < blocks; j++)
                {
                    visb[i, j].Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Провека на рядом стоящие мины
        /// </summary>
        /// <param name="ls">Объект который был выбран игроком</param>
        private void CheckThisMin(Label ls)
        {
            
            if (Grid.GetRow(ls) + 1 < blocks) { NoMins(ls, 1, 0); }

            if (Grid.GetColumn(ls) + 1 < blocks) { NoMins(ls, 0, 1); }

            if (Grid.GetRow(ls) - 1 >= 0) { NoMins(ls, -1, 0);  }
            
            if (Grid.GetColumn(ls) - 1 >= 0) { NoMins(ls, 0, -1); }

        }

        /// <summary>
        /// Условие победы
        /// </summary>
        private void CheckWins()
        {
                       
            noHid = 0;
            for (int i = 0; i < blocks; i++)
            {
                for (int j = 0; j < blocks; j++)
                {
                    if (visb[i, j].Visibility == Visibility.Hidden)
                    {
                        noHid++;
                    }
                }
            }

            if (noHid == (blocks * blocks) - valueMins)
            {
                NoHiddesBlock();
                for (int i = 0; i < blocks; i++ )
                {
                    for(int j = 0; j < blocks; j++)
                    {
                        if (lb[i, j].Tag.Equals("Mina"))
                        {
                            lb[i, j].BorderBrush = Brushes.Green;
                            lb[i, j].BorderThickness = new Thickness(0.75);
                        }
                    }
                }
                    MessageBox.Show("Ты выиграл ☺");
                
                Close();     
            }
        }
         
        /// <summary>
        /// Очистка (пустых) рядом стоящих объетов которые находились рядом с объектом который выбрал игрок
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        private void NoMins(Label ls , int px, int py)
        {
            Boolean tr = false;
            if (lb[Grid.GetRow(ls) + px, Grid.GetColumn(ls) + py].Tag.Equals("NoMina") &&
                visb[Grid.GetRow(ls) + px, Grid.GetColumn(ls) + py].Visibility != Visibility.Hidden)
            {
                tr = true;
            }

            visb[Grid.GetRow(ls) + px, Grid.GetColumn(ls) + py].Visibility = Visibility.Hidden;
            if (tr) { CheckThisMin(lb[Grid.GetRow(ls) + px, Grid.GetColumn(ls) + py]); }

        }

        /// <summary>
        /// Создание мин
        /// </summary>
        private void CreateMin()
        {
            int x = rnd.Next(0, blocks);
            int y = rnd.Next(0, blocks);

            if (lb[x, y].Tag.Equals("Mina"))
                CreateMin();
            else
            {
                lb[x, y].Background = new ImageBrush(new BitmapImage(new Uri("Mina.png", UriKind.Relative)));

                lb[x, y].Tag = "Mina";
                visb[x, y].Tag = lb[x, y].Tag;
                thMina[ns] = lb[x, y];
                ns++;
            }

        }
        
        /// <summary>
        /// Определяем количество рядом стоящих мин около пустого объекта
        /// </summary>
        /// <param name="ls">Пустой объект (который без мины)</param>
        private void ContentLab(Label ls)
        {
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (Grid.GetRow(ls) + i < blocks && Grid.GetRow(ls) + i >= 0 &&
                        Grid.GetColumn(ls) + j < blocks && Grid.GetColumn(ls) + j >= 0)
                    {
                        if (lb[Grid.GetRow(ls) + i, Grid.GetColumn(ls) + j].Content.Equals(""))
                            lb[Grid.GetRow(ls) + i, Grid.GetColumn(ls) + j].Content = "" + 1;
                        else
                            lb[Grid.GetRow(ls) + i, Grid.GetColumn(ls) + j].Content = Int32.Parse(lb[Grid.GetRow(ls) + i, Grid.GetColumn(ls) + j].Content.ToString()) + 1;

                        lb[Grid.GetRow(ls) + i, Grid.GetColumn(ls) + j].Tag = "Value";
                    }
                }
            }
        }

        /// <summary>
        /// Создание игрового поля
        /// </summary>
        /// <returns>Объект на поле</returns>
        private Label CreateLb()
        {
            Label ls = new Label();
            ls.Background = Brushes.Gray;

            ls.Tag = "NoMina";
            ls.Content = "";
            ls.FontFamily = new FontFamily("Times new Roman");
            if(blocks < 21)
                ls.FontSize = 21;

            ls.BorderBrush = Brushes.Black;
            ls.BorderThickness = new Thickness(1);

            ls.HorizontalContentAlignment = HorizontalAlignment.Center;
            ls.VerticalContentAlignment = VerticalAlignment.Center;

            return ls;
        }
    }
}
