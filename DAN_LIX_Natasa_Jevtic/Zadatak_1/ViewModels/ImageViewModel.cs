using Zadatak_1.Models;

namespace Zadatak_1.ViewModels
{
    class ImageViewModel : BaseViewModel
    {
        //card
        public Image image { get; set; }
        //Id for card
        public int Id { get; private set; }
        //is user viewed a card
        private bool isViewed;        
        public bool IsViewed
        {
            get
            {
                return isViewed;
            }
            private set
            {
                isViewed = value;
                OnPropertyChanged("CardImage");
            }
        }
        //is user guess the card
        private bool isMatched;        
        public bool IsMatched
        {
            get
            {
                return isMatched;
            }
            private set
            {
                isMatched = value;
                OnPropertyChanged("CardImage");
            }
        }
        //if user doesn't guess the card
        private bool isFailed;
        public bool IsFailed
        {
            get
            {
                return isFailed;
            }
            private set
            {
                isFailed = value;
                OnPropertyChanged("SlideImage");
            }
        }
        //if user can select card
        public bool CanSelect
        {
            get
            {
                if (isMatched == true)
                {
                    return false;
                }
                if (isViewed == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //which image will be displayed
        public string CardImage
        {
            get
            {
                if (isMatched == true)
                {
                    return image.Source;
                }
                if (isViewed == true)
                {
                    return image.Source;
                }
                else
                {
                    return @"..\..\Images/question_image.jpg";
                }
            }
        }
        /// <summary>
        /// Constructor with parameter.
        /// </summary>
        /// <param name="image">Image.</param>
        public ImageViewModel(Image image)
        {
            this.image = image;
            Id = image.Id;
        }
        /// <summary>
        /// This method set property IsMatched to true, when user matchs the cards.
        /// </summary>
        public void MarkMatched()
        {
            IsMatched = true;
        }

        /// <summary>
        /// This method set property IsFailed to true, when user doesn't match the cards.
        /// </summary>
        public void MarkFailed()
        {
            IsFailed = true;
        }
        /// <summary>
        /// This method closes a card.
        /// </summary>
        public void CloseCard()
        {
            isViewed = false;
            isFailed = false;
            OnPropertyChanged("isSelectable");
            OnPropertyChanged("CardImage");
        }
        /// <summary>
        /// This method allows user to view a card.
        /// </summary>
        public void PeekAtImage()
        {
            isViewed = true;
            OnPropertyChanged("CardImage");
        }
    }
}