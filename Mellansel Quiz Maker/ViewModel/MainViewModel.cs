using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Mellansel_Quiz_Maker.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace Mellansel_Quiz_Maker.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".BMP", ".GIF", ".PNG" };
        private static readonly List<char> ForbiddenCharacters = new List<char> { ':', '<', '>', '|', '*', '?','"','/','\\' };
        public List<BitmapImage> Pictures { get; set; }
        public List<QuestionData> QuestionsAnswers { get; set; }

        private BitmapImage _selectedPicture;
        public BitmapImage SelectedPicture
        {
            get
            {
                return _selectedPicture;
            }
            set
            {
                _selectedPicture = value;
                RaisePropertyChanged(() => SelectedPicture);
            }
        }

        private BitmapImage _backgroundPicture;
        public BitmapImage BackgroundPicture
        {
            get
            {
                return _backgroundPicture;
            }
            set
            {
                _backgroundPicture = value;
                RaisePropertyChanged(() => BackgroundPicture);
                SaveQuestionsCommand.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand _selectPictureCommand;
        public RelayCommand SelectPictureCommand
        {
            get
            {
                if (_selectPictureCommand == null)
                {
                    _selectPictureCommand = new RelayCommand(() => SelectPictureDialog());
                }
                return _selectPictureCommand;
            }
        }

        private RelayCommand _selectBackgroundCommand;
        public RelayCommand SelectBackgroundCommand
        {
            get
            {
                if (_selectBackgroundCommand == null)
                {
                    _selectBackgroundCommand = new RelayCommand(() => SelectBackgroundDialog());
                }
                return _selectBackgroundCommand;
            }
        }

        private RelayCommand _deleteQuestionCommand;
        public RelayCommand DeleteQuestionCommand
        {
            get
            {
                if (_deleteQuestionCommand == null)
                {
                    _deleteQuestionCommand = new RelayCommand(() => DeleteQuestion(),() => QuestionsAnswers.Count > 1);
                }
                return _deleteQuestionCommand;
            }
        }

        private RelayCommand _nextQuestionCommand;
        public RelayCommand NextQuestionCommand
        {
            get
            {
                if (_nextQuestionCommand == null)
                {
                    _nextQuestionCommand = new RelayCommand(() => LoadNextQuestion());
                }
                return _nextQuestionCommand;
            }
        }

        private RelayCommand _previousQuestionCommand;
        public RelayCommand PreviousQuestionCommand
        {
            get
            {
                if (_previousQuestionCommand == null)
                {
                    _previousQuestionCommand = new RelayCommand(() => LoadPreviousQuestion());
                }
                return _previousQuestionCommand;
            }
        }

        private RelayCommand _saveQuestionsCommand;
        public RelayCommand SaveQuestionsCommand
        {
            get
            {
                if (_saveQuestionsCommand == null)
                {
                    _saveQuestionsCommand = new RelayCommand(() => SaveQuestions(),() => BackgroundPicture != null && HeaderText.Length >= 1);
                }
                return _saveQuestionsCommand;
            }
        }

        private RelayCommand<string> _answerSelectedCommand;
        public RelayCommand<string> AnswerSelectedCommand
        {
            get
            {
                if (_answerSelectedCommand == null)
                {
                    _answerSelectedCommand = new RelayCommand<string>((answer) => AnswerSelected(answer));
                }
                return _answerSelectedCommand;
            }
        }

        private bool _alt1Correct;
        public bool Alt1Correct
        {
            get { return _alt1Correct; }
            set { _alt1Correct = value; RaisePropertyChanged(() => Alt1Correct); }
        }

        private bool _alt2Correct;
        public bool Alt2Correct
        {
            get { return _alt2Correct; }
            set { _alt2Correct = value; RaisePropertyChanged(() => Alt2Correct); }
        }

        private bool _alt3Correct;
        public bool Alt3Correct
        {
            get { return _alt3Correct; }
            set { _alt3Correct = value; RaisePropertyChanged(() => Alt3Correct); }
        }

        private int _selectedQuestion = 1;
        public int SelectedQuestion
        {
            get
            {
                return _selectedQuestion;
            }
            set
            {
                _selectedQuestion = value;
                RaisePropertyChanged(() => SelectedQuestion);
            }
        }

        private string _headerText = "Här kan du skriva vilken rubrik du vill ha.";
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                var validCharacter = true;
                foreach (var c in ForbiddenCharacters)
                {
                    if(value.Contains(c.ToString()))
                    {
                        validCharacter = false;
                    }
                }
                if(validCharacter)
                {
                    _headerText = value;
                    RaisePropertyChanged(() => HeaderText);
                }
                else
                {
                    RaisePropertyChanged(() => HeaderText);
                }
                
            }
        }

        private string _alternative1Text;
        public string Alternative1Text
        {
            get
            {
                return _alternative1Text;
            }
            set
            {
                _alternative1Text = value;
                QuestionsAnswers[SelectedQuestion - 1].AlternativeOne = value;
                RaisePropertyChanged(() => Alternative1Text);
            }
        }

        private string _alternative2Text;
        public string Alternative2Text
        {
            get
            {
                return _alternative2Text;
            }
            set
            {
                _alternative2Text = value;
                QuestionsAnswers[SelectedQuestion - 1].AlternativeTwo = value;
                RaisePropertyChanged(() => Alternative2Text);
            }
        }

        private string _alternative3Text;
        public string Alternative3Text
        {
            get
            {
                return _alternative3Text;
            }
            set
            {
                _alternative3Text = value;
                QuestionsAnswers[SelectedQuestion - 1].AlternativeThree = value;
                RaisePropertyChanged(() => Alternative3Text);
            }
        }

        private string _questionText;
        public string QuestionText
        {
            get
            {
                return _questionText;
            }
            set
            {
                _questionText = value;
                QuestionsAnswers[SelectedQuestion - 1].Question = value;
                RaisePropertyChanged(() => QuestionText);
            }
        }

        public MainViewModel()
        {
            initializeApplication();
        }

        private void initializeApplication()
        {
            Pictures = new List<BitmapImage>();
            Pictures.Add(new BitmapImage());
            QuestionsAnswers = new List<QuestionData>();
            QuestionsAnswers.Add(new QuestionData());
            SelectedPicture = null;
            BackgroundPicture = null;
            SelectedQuestion = 1;
            QuestionText = "";
            Alternative1Text = "";
            Alternative2Text = "";
            Alternative3Text = "";
            Alt1Correct = false;
            Alt2Correct = false;
            Alt3Correct = false;
            DeleteQuestionCommand.RaiseCanExecuteChanged();
            HeaderText = "Här kan du skriva vilken rubrik du vill ha.";
        }

        private void SaveQuestions()
        {
            if(Pictures[QuestionsAnswers.Count -1].UriSource == null || BackgroundPicture.UriSource == null || QuestionsAnswers[QuestionsAnswers.Count - 1].Question == null ||
                QuestionsAnswers[QuestionsAnswers.Count - 1].AlternativeOne == null || QuestionsAnswers[QuestionsAnswers.Count - 1].AlternativeTwo == null ||
                QuestionsAnswers[QuestionsAnswers.Count - 1].AlternativeThree == null || HeaderText == null || QuestionsAnswers[QuestionsAnswers.Count - 1].Answer == null)
            {
                MessageBox.Show("Du måste fylla ialla fält eller ta bort den påbörjade frågan innan du exporterar.");
                return;
            }
            string dummyFileName = "Spara här";
            SaveFileDialog sf = new SaveFileDialog();
           
            sf.FileName = dummyFileName;
            var result = sf.ShowDialog();
            if((bool)result)
            {
                string savePath = Path.GetDirectoryName(sf.FileName);
                var index = 1;
                foreach(var pic in Pictures)
                {
                    if (pic.UriSource != null)
                    {
                        var extension = Path.GetExtension(pic.UriSource.AbsolutePath.ToLower());

                        switch (extension)
                        {
                            case ".jpg":
                            case ".jpeg":
                                {
                                    FileStream stream = new FileStream(Path.Combine(savePath, index + extension), FileMode.Create);
                                    BitmapEncoder encoder = new JpegBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(pic));
                                    encoder.Save(stream);
                                    stream.Dispose();
                                    index++;
                                    break;
                                }
                            case ".png":
                                {
                                    FileStream stream = new FileStream(Path.Combine(savePath, index + extension), FileMode.Create);
                                    BitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(pic));
                                    encoder.Save(stream);
                                    stream.Dispose();
                                    index++;
                                    break;
                                }
                            case ".bmp":
                                {
                                    FileStream stream = new FileStream(Path.Combine(savePath, index + extension), FileMode.Create);
                                    BitmapEncoder encoder = new BmpBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(pic));
                                    encoder.Save(stream);
                                    stream.Dispose();
                                    index++;
                                    break;
                                }
                            case ".gif":
                                {
                                    FileStream stream = new FileStream(Path.Combine(savePath, index + extension), FileMode.Create);
                                    BitmapEncoder encoder = new GifBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(pic));
                                    encoder.Save(stream);
                                    stream.Dispose();
                                    index++;
                                    break;
                                }
                        }
                    }
                }
                var backgroundExtension = Path.GetExtension(BackgroundPicture.UriSource.AbsoluteUri.ToLower());
                switch (backgroundExtension)
                {
                    case ".jpg": case ".jpeg":
                        {
                            FileStream stream = new FileStream(Path.Combine(savePath, "background" + "#" + HeaderText + backgroundExtension), FileMode.Create);
                            BitmapEncoder encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(BackgroundPicture));
                            encoder.Save(stream);
                            stream.Dispose();
                            break;
                        }
                    case ".png":
                        {
                            FileStream stream = new FileStream(Path.Combine(savePath, "background" + "#" + HeaderText + backgroundExtension), FileMode.Create);
                            BitmapEncoder encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(BackgroundPicture));
                            encoder.Save(stream);
                            stream.Dispose();
                            break;
                        }
                    case ".bmp":
                        {
                            FileStream stream = new FileStream(Path.Combine(savePath, "background" + "#" + HeaderText + backgroundExtension), FileMode.Create);
                            BitmapEncoder encoder = new BmpBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(BackgroundPicture));
                            encoder.Save(stream);
                            stream.Dispose();
                            break;
                        }
                    case ".gif":
                        {
                            FileStream stream = new FileStream(Path.Combine(savePath, "background" + "#" + HeaderText + backgroundExtension), FileMode.Create);
                            BitmapEncoder encoder = new GifBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(BackgroundPicture));
                            encoder.Save(stream);
                            stream.Dispose();
                            break;
                        }
                }
                var json = JsonConvert.SerializeObject(QuestionsAnswers.ToArray());
                File.WriteAllText(Path.Combine(savePath, "questions.json"), json);
                initializeApplication();
            }
        }

        private void LoadPreviousQuestion()
        {
            DeleteQuestionCommand.RaiseCanExecuteChanged();
            if (SelectedQuestion != 1)
            {
                SelectedQuestion--;
                SelectedPicture = Pictures[SelectedQuestion - 1];
                QuestionText = QuestionsAnswers[SelectedQuestion - 1].Question;
                Alternative1Text = QuestionsAnswers[SelectedQuestion - 1].AlternativeOne;
                Alternative2Text = QuestionsAnswers[SelectedQuestion - 1].AlternativeTwo;
                Alternative3Text = QuestionsAnswers[SelectedQuestion - 1].AlternativeThree;
                var CA = QuestionsAnswers[SelectedQuestion - 1].Answer;
                if (CA == Alternative1Text)
                {
                    Alt1Correct = true;
                    Alt2Correct = false;
                    Alt3Correct = false;
                }
                else if (CA == Alternative2Text)
                {
                    Alt2Correct = true;
                    Alt1Correct = false;
                    Alt3Correct = false;
                }
                else
                {
                    Alt3Correct = true;
                    Alt1Correct = false;
                    Alt2Correct = false;
                }
            }
            DeleteQuestionCommand.RaiseCanExecuteChanged();
        }

        private void LoadNextQuestion()
        {
            if(SelectedPicture == null || SelectedPicture.UriSource == null)
            {
                MessageBox.Show("Du måste välja en bild till frågan!");
                return;
            }
            if (string.IsNullOrEmpty(QuestionText))
            {
                MessageBox.Show("Du måste skriva en fråga!");
                return;
            }
            if (string.IsNullOrEmpty(Alternative1Text) || string.IsNullOrEmpty(Alternative2Text) || string.IsNullOrEmpty(Alternative3Text))
            {
                MessageBox.Show("Du måste skriva alla 3 svars alternativ!");
                return;
            }
            if (Alt1Correct == false && Alt2Correct == false && Alt3Correct == false)
            {
                MessageBox.Show("Du måste välja vad som är rätt svar!");
                return;
            }

            SelectedQuestion++;
            if (QuestionsAnswers.Count < SelectedQuestion)
            {
                QuestionsAnswers.Add(new QuestionData());
                SelectedPicture = new BitmapImage();
                Pictures.Add(new BitmapImage());
                QuestionText = "";
                Alternative1Text = "";
                Alternative2Text = "";
                Alternative3Text = "";
                Alt1Correct = false;
                Alt2Correct = false;
                Alt3Correct = false;
            }
            else
            {
                SelectedPicture = Pictures[SelectedQuestion - 1];
                QuestionText = QuestionsAnswers[SelectedQuestion - 1].Question;
                Alternative1Text = QuestionsAnswers[SelectedQuestion - 1].AlternativeOne;
                Alternative2Text = QuestionsAnswers[SelectedQuestion - 1].AlternativeTwo;
                Alternative3Text = QuestionsAnswers[SelectedQuestion - 1].AlternativeThree;
                var CA = QuestionsAnswers[SelectedQuestion - 1].Answer;
                if (CA == Alternative1Text)
                {
                    Alt1Correct = true;
                    Alt2Correct = false;
                    Alt3Correct = false;
                }
                else if (CA == Alternative2Text)
                {
                    Alt2Correct = true;
                    Alt1Correct = false;
                    Alt3Correct = false;
                }
                else if(CA == Alternative3Text)
                {
                    Alt3Correct = true;
                    Alt1Correct = false;
                    Alt2Correct = false;
                }
                else
                {
                    Alt1Correct = false;
                    Alt2Correct = false;
                    Alt3Correct = false;
                }
            }
            DeleteQuestionCommand.RaiseCanExecuteChanged();
        }

        private void DeleteQuestion()
        {
            if(MessageBox.Show("Är du säker på att du vill ta bort frågan?", "Bekräftelse", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Pictures.Remove(SelectedPicture);
                QuestionsAnswers.RemoveAt(SelectedQuestion - 1);
                LoadPreviousQuestion();
            }
        }

        private void SelectPictureDialog()
        {
            OpenFileDialog file = new OpenFileDialog();
            bool? result = file.ShowDialog();

            if (File.Exists(file.FileName))
            {
                if (ImageExtensions.Contains(Path.GetExtension(file.FileName).ToUpper()))
                {
                    var selectedPic = new BitmapImage();
                    selectedPic.BeginInit();
                    selectedPic.UriSource = new Uri(file.FileName, UriKind.Absolute);
                    selectedPic.EndInit();
                    SelectedPicture = selectedPic;
                    Pictures[SelectedQuestion - 1] = SelectedPicture;
                }
                else
                {
                    MessageBox.Show("Du har inte valt en bild med giltigt bildformat.");
                }
            }
        }

        private void SelectBackgroundDialog()
        {
            OpenFileDialog file = new OpenFileDialog();
            bool? result = file.ShowDialog();

            if (File.Exists(file.FileName))
            {
                if (ImageExtensions.Contains(Path.GetExtension(file.FileName).ToUpper()))
                {
                    var selectedPic = new BitmapImage();
                    selectedPic.BeginInit();
                    selectedPic.UriSource = new Uri(file.FileName, UriKind.Absolute);
                    selectedPic.EndInit();
                    BackgroundPicture = selectedPic;
                }
                else
                {
                    MessageBox.Show("Du har inte valt en bild med giltigt bildformat.");
                }
            }
        }

        private void AnswerSelected(string answer)
        {
            switch (answer)
            {
                case "alt1":
                    {
                        QuestionsAnswers[SelectedQuestion - 1].Answer = Alternative1Text;
                        break;
                    }
                case "alt2":
                    {
                        QuestionsAnswers[SelectedQuestion - 1].Answer = Alternative2Text;
                        break;
                    }
                case "alt3":
                    {
                        QuestionsAnswers[SelectedQuestion - 1].Answer = Alternative3Text;
                        break;
                    }
            }
        }
    }
}