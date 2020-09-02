using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Zadatak_1.Models;

namespace Zadatak_1.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        MainWindow mainWindow;
        //path to the folder with images
        readonly string directory = @"..\..\Images/";
        //path to the txt file with winning games
        readonly string txtFile = @"..\..\IgraPamcenja.txt";
        //choosen images for matching
        public ImageViewModel FirstSelectedCard { get; set; }
        public ImageViewModel SecondSelectedCard { get; set; }
        //timers for memorizing and choosing images
        public DispatcherTimer TimerForMemorizing { get; set; }
        public DispatcherTimer TimerForChoosing { get; set; }
        //interval for how long a user choosing
        private const int secondOfChoosing = 1;
        //interval for how long a user has to memorize images
        private const int secondsForMemorizing = 5;
        //background worker for displaying game time
        BackgroundWorker backgroundWorker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        //colection of cards
        private ObservableCollection<ImageViewModel> cardCollection;

        public ObservableCollection<ImageViewModel> CardCollection
        {
            get
            {
                return cardCollection;
            }
            set
            {
                cardCollection = value;
                OnPropertyChanged("CardCollection");
            }
        }

        private Image card;

        public Image Card
        {
            get
            {
                return card;
            }
            set
            {
                card = value;
                OnPropertyChanged("Card");
            }
        }
        //for the countdown of game time
        private string time;

        public string Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }
        //can user selected a card
        public bool CanSelect { get; set; }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            TimerForChoosing = new DispatcherTimer();
            TimerForChoosing.Interval = new TimeSpan(0, 0, secondOfChoosing);
            TimerForChoosing.Tick += PeekTimer_Tick;

            TimerForMemorizing = new DispatcherTimer();
            TimerForMemorizing.Interval = new TimeSpan(0, 0, secondsForMemorizing);
            TimerForMemorizing.Tick += OpeningTimer_Tick;
            CreateCards();
            TimerForMemorizing.Start();
            //adding handler to DoWork
            backgroundWorker.DoWork += BW_DoWork;
            //adding handler to ProgressChanged event
            backgroundWorker.ProgressChanged += BW_ProgressChanged;
            //adding handler to RunWorkerCompleted event
            backgroundWorker.RunWorkerCompleted += BW_RunWorkerCompleted;
        }

        /// <summary>
        /// This method adding all images from file to the collection of cards.
        /// </summary>        
        public void CreateCards()
        {
            CardCollection = new ObservableCollection<ImageViewModel>();
            var models = GetImages();
            //creating two card for every image 
            for (int i = 0; i < 8; i++)
            {
                //creating matching cards
                var newSlide = new ImageViewModel(models[i]);
                var newSlideMatch = new ImageViewModel(models[i]);
                //adding cards to collection
                cardCollection.Add(newSlide);
                cardCollection.Add(newSlideMatch);
                //Initially display images for user
                newSlide.PeekAtImage();
                newSlideMatch.PeekAtImage();
            }
            //when cards is added to the collection, then get random layout
            RandomCardlayout();
            OnPropertyChanged("CardCollection");
        }
        /// <summary>
        /// This method for every image from file creates a object of class Image.
        /// </summary>
        /// <returns>List of images.</returns>
        private List<Image> GetImages()
        {            
            List<Image> listOfImages = new List<Image>();
            //getting URIs of images
            var images = Directory.GetFiles(directory, "*.jpg");
            //counter for id
            int id = 0;
            //based on every image from folder create object and add to the list
            foreach (string i in images)
            {
                listOfImages.Add(new Image() { Id = id, Source = i });
                id++;
            }
            return listOfImages;
        }
        /// <summary>
        /// This method creates a random card layout.
        /// </summary>
        private void RandomCardlayout()
        {            
            Random random = new Random();            
            for (int i = 0; i < 64; i++)
            {
                CardCollection.Reverse();
                CardCollection.Move(random.Next(0, CardCollection.Count), random.Next(0, CardCollection.Count));
            }
        }
        /// <summary>
        /// This method writes data about winning games to txt file.
        /// </summary>
        public void ToTxtFile()
        {
            StreamWriter str = new StreamWriter(txtFile, true);
            //writing date, time and duration of game
            str.WriteLine("[" + DateTime.Now + "] Duration:" + (60 - Int32.Parse(Time)));
            str.Close();
        }
        /// <summary>
        /// This method displays a countdown timer of sixty seconds.
        /// </summary>
        /// <param name="sender">Object.</param>
        /// <param name="e">DoWorkEventArgs object.</param>
        public void BW_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 60; i >= 0; i--)
            {
                //if canceling is requested
                if (backgroundWorker.CancellationPending)
                {
                    //setting property to true
                    e.Cancel = true;
                    //passing zero to reset timer
                    backgroundWorker.ReportProgress(0);
                    return;
                }

                //invoking method that raises ProgressChanged event and passing the value of time
                backgroundWorker.ReportProgress(i);
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// This method updates user interface element with the value of time.
        /// </summary>
        /// <param name="sender">Object.</param>
        /// <param name="e">ProgressChangedEventArgs object.</param>
        public void BW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //setting value of user interface element
            Time = e.ProgressPercentage.ToString();
        }
        /// <summary>
        /// This method invokes a method for displaying a message to the user if time is elapsed or cancel timer if not elapsed, but user finished a game.
        /// </summary>
        /// <param name="sender">Object.</param>
        /// <param name="e">RunWorkerCompletedEventArgs object.</param>
        public void BW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {                
                return;

            }
            //if background worker finished (time is up), informing the user that he is lost and asking him if he wants to play again
            MessageBoxResult result = MessageBox.Show("You lost. Do you want to play again?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                backgroundWorker.CancelAsync();
                MainWindow newMain = new MainWindow();
                mainWindow.Close();
                newMain.ShowDialog();
            }
            else
            {
                mainWindow.Close();
            }
        }
        /// <summary>
        /// This method displays cards to the user, then hidden them and starts a countdown timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpeningTimer_Tick(object sender, EventArgs e)
        {
            foreach (var slide in CardCollection)
            {
                slide.CloseCard();
                CanSelect = true;
            }
            OnPropertyChanged("AreSlidesActive");
            TimerForMemorizing.Stop();
            backgroundWorker.RunWorkerAsync();
        }
        /// <summary>
        /// This method checks if the card is matched. If it is not matched image is hidden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeekTimer_Tick(object sender, EventArgs e)
        {
            foreach (var slide in CardCollection)
            {
                if (!slide.IsMatched)
                {
                    slide.CloseCard();
                    CanSelect = true;
                }
            }
            OnPropertyChanged("AreSlidesActive");
            TimerForChoosing.Stop();
        }
    }
}
