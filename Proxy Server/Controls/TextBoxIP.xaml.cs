using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StrongProxy.Controls
{
    /// <summary>
    /// Interaction logic for TextBoxIP.xaml
    /// </summary>
    public partial class TextBoxIP : UserControl
    {
        #region class variables and properties

        #region public variables and properties
        public TextBox FirstBox { get { return firstBox; } }
        public TextBox SecondBox { get { return secondBox; } }
        public TextBox ThirdBox { get { return thirdBox; } }
        public TextBox FourthBox { get { return fourthBox; } }
        public TextBox FivethBox { get { return fivethBox; } }
        #endregion

        #region private variables and properties

        #endregion

        #endregion


        #region constructors
        public TextBoxIP()
        {
            InitializeComponent();
        }

        public TextBoxIP(byte[] bytesToFill)
        {
            InitializeComponent();

            firstBox.Text = Convert.ToString(bytesToFill[0]);
            secondBox.Text = Convert.ToString(bytesToFill[1]);
            thirdBox.Text = Convert.ToString(bytesToFill[2]);
            fourthBox.Text = Convert.ToString(bytesToFill[3]);
            fivethBox.Text = Convert.ToString(bytesToFill[4]);
        }
        #endregion


        #region methods

        #region public methods
        public byte[] GetByteArray()
        {
            byte[] userInput = new byte[5];

            userInput[0] = Convert.ToByte(firstBox.Text);
            userInput[1] = Convert.ToByte(secondBox.Text);
            userInput[2] = Convert.ToByte(thirdBox.Text);
            userInput[3] = Convert.ToByte(fourthBox.Text);
            userInput[4] = Convert.ToByte(fivethBox.Text);

            return userInput;
        }
        #endregion

        #region private methods
        private void jumpRight(TextBox rightNeighborBox, KeyEventArgs e)
        {
            rightNeighborBox.Focus();
            rightNeighborBox.CaretIndex = 0;
            e.Handled = true;
        }

        private void jumpLeft(TextBox leftNeighborBox, KeyEventArgs e)
        {
            leftNeighborBox.Focus();
            if (leftNeighborBox.Text != "")
            {
                leftNeighborBox.CaretIndex = leftNeighborBox.Text.Length;
            }
            e.Handled = true;
        }

        //checks for backspace, arrow and decimal key presses and jumps boxes if needed.
        //returns true when key was matched, false if not.
        private bool checkJumpRight(TextBox currentBox, TextBox rightNeighborBox, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    if (currentBox.CaretIndex == currentBox.Text.Length || currentBox.Text == "")
                    {
                        jumpRight(rightNeighborBox, e);
                    }
                    return true;
                case Key.OemPeriod:
                case Key.Decimal:
                case Key.Space:
                    jumpRight(rightNeighborBox, e);
                    rightNeighborBox.SelectAll();
                    return true;
                default:
                    return false;
            }
        }

        private bool checkJumpLeft(TextBox currentBox, TextBox leftNeighborBox, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (currentBox.CaretIndex == 0 || currentBox.Text == "")
                    {
                        jumpLeft(leftNeighborBox, e);
                    }
                    return true;
                case Key.Back:
                    if ((currentBox.CaretIndex == 0 || currentBox.Text == "") && currentBox.SelectionLength == 0)
                    {
                        jumpLeft(leftNeighborBox, e);
                    }
                    return true;
                default:
                    return false;
            }
        }

        //discards non digits, prepares IPMaskedBox for textchange.
        private void handleTextInput(TextBox currentBox, TextBox rightNeighborBox, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(Convert.ToChar(e.Text)))
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
                return;
            }

            if (currentBox.Text.Length == 5 && currentBox.SelectionLength == 0)
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
                if (currentBox != fivethBox)
                {
                    rightNeighborBox.Focus();
                    rightNeighborBox.SelectAll();
                }
            }
        }

        //checks whether textbox content > 255 when 3 characters have been entered.
        //clears if > 255, switches to next textbox otherwise 
        private void handleTextChange(TextBox currentBox, TextBox rightNeighborBox)
        {
            if (currentBox.Text.Length == 3)
            {
                try
                {
                    Convert.ToByte(currentBox.Text);

                }
                catch (Exception exception) when (exception is FormatException || exception is OverflowException)
                {
                    currentBox.Clear();
                    currentBox.Focus();
                    SystemSounds.Beep.Play();
                    MessageBox.Show("" + Constant.DISPLAY_ERROR_IP, "" + Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (currentBox.CaretIndex != 2 && currentBox != fivethBox)
                {
                    rightNeighborBox.CaretIndex = rightNeighborBox.Text.Length;
                    rightNeighborBox.SelectAll();
                    rightNeighborBox.Focus();
                }
            }
        }
        #endregion      

        #endregion


        #region Events
        //jump right, left or stay. 
        private void firstByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            checkJumpRight(firstBox, secondBox, e);
        }

        private void secondByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (checkJumpRight(secondBox, thirdBox, e))
                return;

            checkJumpLeft(secondBox, firstBox, e);
        }

        private void thirdByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (checkJumpRight(thirdBox, fourthBox, e))
                return;

            checkJumpLeft(thirdBox, secondBox, e);
        }

        private void fourthByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //checkJumpLeft(fourthBox, thirdBox, e);

            //if (e.Key == Key.Space)
            //{
            //    SystemSounds.Beep.Play();
            //    e.Handled = true;
            //}

            if (checkJumpRight(fourthBox, fivethBox, e))
                return;

            checkJumpLeft(fourthBox, thirdBox, e);
        }


        //discards non digits, prepares IPMaskedBox for textchange.
        private void firstByte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(firstBox, secondBox, e);
        }

        private void secondByte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(secondBox, thirdBox, e);
        }

        private void thirdByte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(thirdBox, fourthBox, e);
        }

        private void fourthByte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(fourthBox, fivethBox, e); //pass fourthbyte twice because no right neighboring box.
        }


        //checks whether textbox content > 255 when 3 characters have been entered.
        //clears if > 255, switches to next textbox otherwise 
        private void firstByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            handleTextChange(firstBox, secondBox);
        }

        private void secondByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            handleTextChange(secondBox, thirdBox);
        }

        private void thirdByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            handleTextChange(thirdBox, fourthBox);
        }

        private void fourthByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            handleTextChange(fourthBox, fivethBox);
        }
        #endregion

        private void fivethBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            checkJumpLeft(fivethBox, fourthBox, e);

            if (e.Key == Key.Space)
            {
                SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }

        private void fivethBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(fivethBox, fivethBox, e);
        }

        //private void fivethBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    int value = Int32.Parse(fivethBox.Text);

        //    if (value >= 65536)
        //    {
        //        fivethBox.Clear();
        //        fivethBox.Focus();
        //        SystemSounds.Beep.Play();
        //        MessageBox.Show("" + Constant.DISPLAY_ERROR_IP, "" + Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
    }
}
