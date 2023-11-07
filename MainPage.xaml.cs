using System.IO.Ports;
using System.Text;
using System.Timers;
//using Windows.Devices.Enumeration;


namespace SolarLabRight2023
{
    public partial class MainPage : ContentPage
    {

        //set up variables needed for our main page code
        private bool bPortOpen = false;
        private string newPacket = "";
        private int oldPacketNumber = -1;
        private int newPacketNumber = 0;
        private int lostPacketCount = 0;
        private int packetRollOver = 0;
        private int chkSumError = 0;

        //set up variables for the graph
        public int Yaxis = 0;
        public double degrees = 0;
        public int count = 0;
        public int graphHeight = 500;


        //create a new instances needed for our program
        StringBuilder stringBuilderSend = new StringBuilder("###1111196");

        SerialPort serialPort = new SerialPort();

        SolarCalc solarCalc = new SolarCalc();

        //initialize main page and then generate it with the ComPort picker setup
        public MainPage()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            portPicker.ItemsSource = ports; //has all the port names and will add all at once
            portPicker.SelectedIndex = ports.Length; //prepopulate port to last on selected
            Loaded += MainPage_Loaded; //make sure the whole page is loaded before advancing in your code

            //foreach (string port in ports)
            //{
            //    portPicker.Items.Add(port);
            //}
        }

        //function to open/close the desired comport when needed the button is clicked
        private void btnOpenClose_Clicked(object sender, EventArgs e)
        {

            //if the button isnt open and the button is pressed, open the port and change the button text to close
            if (!bPortOpen)
            {
                serialPort.PortName = portPicker.SelectedItem.ToString();
                serialPort.Open();
                btnOpenClose.Text = "Close";
                bPortOpen = true;
            }
            //if the button is open, close the port and update the button text
            else
            {
                serialPort.Close();
                btnOpenClose.Text = "Open";
                bPortOpen = false;
            }


        }

        //function that initialize the serialPort with our desired data rate 
        private void MainPage_Loaded(object sender, EventArgs e)
        {
            //set up serial ports 
            serialPort.BaudRate = 115200;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.DataReceived += SerialPort_DataReceived;

            //set up graph using timer
            var timer = new System.Timers.Timer(16);
            timer.Elapsed += new ElapsedEventHandler(DrawNewPointOnGraph);
            timer.Start();

        }

        private void DrawNewPointOnGraph(object sender, ElapsedEventArgs e)
        {
            //function to draw the desired graphs 
            var graphicsView = this.LineGraphView;
            var lineGraphDrawable = (LineDrawable)graphicsView.Drawable;

            double angle = Math.PI * degrees++ / 180;
            //lineGraphDrawable.baseGraphs[0].Yaxis = (int)((graphHeight / 2 * Math.Sin(angle)) + graphHeight / 2); //sin
            //lineGraphDrawable.baseGraphs[1].Yaxis = (int)(-0.002 * Math.Pow((500 - count), 2) + graphHeight); //quadratic
            //lineGraphDrawable.baseGraphs[2].Yaxis = count--; //sawtooth
            lineGraphDrawable.baseGraphs[0].Yaxis = (int)((solarCalc.analogVoltage[0]) * (500/3300)); //solar voltage
            lineGraphDrawable.baseGraphs[0].Yaxis = (int)((solarCalc.analogVoltage[1]) * (500/3300));//battery voltage
            if (count < 0)
            {
                count = graphHeight;
            }

            //lineGraphDrawable.baseGraphs[0].Yaxis = (int)solarCalc.analogVoltage[0];
            //lineGraphDrawable.baseGraphs[1].Yaxis = (int)solarCalc.analogVoltage[1];
            //lineGraphDrawable.baseGraphs[2].Yaxis = (int)solarCalc.analogVoltage[2];
            //lineGraphDrawable.baseGraphs[3].Yaxis = (int)solarCalc.analogVoltage[3];
            //lineGraphDrawable.baseGraphs[4].Yaxis = (int)solarCalc.analogVoltage[4];

            graphicsView.Invalidate();
        }

        //function that recieves the serial port data and invokes an action on the main thread using information provided by MyMainThreadCode fucntion .  
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            newPacket = serialPort.ReadLine();
            //labelRXdata.Text = newPacket;
            MainThread.BeginInvokeOnMainThread(MyMainThreadCode);

        }

        //function that contains the main code for the program 
        private void MyMainThreadCode()
        {
            //if the box is checked, display the new packet information on the line after the previous packet. if  the box isnt check, replace the oldPacket information with the newpacket information 
            if (checkBoxHistory.IsChecked == true)
            {
                labelRXdata.Text = newPacket + labelRXdata.Text;
            }
            else
            {
                labelRXdata.Text = newPacket;
            }
            int calChkSum = 0;

            //check to see if the packet is 38 in length, i.e a valid packet 
            if (newPacket.Length >= 38)
            {

                //if the newPacket starts with the characters ###
                if (newPacket.Substring(0, 3) == "###")
                {
                    //code block that takes parsed data and sends it to a vertical stack that then displays it

                    newPacketNumber = Convert.ToInt32(newPacket.Substring(3, 3));


                    //this if statement is used to detect a packet number rollover, i.e., when the packetNumber > 999, as well as to detect packet loss. 
                    if (oldPacketNumber > -1)
                    {
                        if (newPacketNumber < oldPacketNumber)
                        {
                            packetRollOver++;

                            if (oldPacketNumber != 999)
                            {
                                lostPacketCount += 999 - oldPacketNumber + newPacketNumber;

                            }

                        }
                        else
                        {
                            if (newPacketNumber != oldPacketNumber + 1)
                            {
                                lostPacketCount += newPacketNumber - oldPacketNumber - 1;

                            }

                        }
                    }

                    //add all the values after the ### to verify the sum of the packet 
                    for (int i = 3; i < newPacket.Length && i < 34; i++)
                    {
                        calChkSum += (byte)newPacket[i];
                    }



                    calChkSum %= 1000;
                    int recChkSum = Convert.ToInt32(newPacket.Substring(34, 3));

                    //display the verified data if the reciever chkSum == calulated chkSum
                    if (recChkSum == calChkSum)
                    {
                        DisplaySolarData(newPacket);

                        oldPacketNumber = newPacketNumber;
                    }
                    //otherwise return add 1 to the count of chkSumError 
                    else
                    {
                        chkSumError++;


                    }



                    DisplayVerifiedData(calChkSum);

                }

            }
        }

        //function gets our solar data from the SolarCalc class and diplays it in its respective spot in the application. 
        private void DisplaySolarData(string validPacket)
        {
            solarCalc.ParseSolarData(validPacket);
            labelSolarVoltage.Text = solarCalc.GetVoltageString(solarCalc.analogVoltage[0]);
            labelBatteryVoltage.Text = solarCalc.GetVoltageString(solarCalc.analogVoltage[1]);
            labelBatteryCurrent.Text = solarCalc.GetCurrent(solarCalc.analogVoltage[2], solarCalc.analogVoltage[1]);
            labelLED1Current.Text = solarCalc.GetLEDCurrent(solarCalc.analogVoltage[2], solarCalc.analogVoltage[4]);
            labelLED2Current.Text = solarCalc.GetLEDCurrent(solarCalc.analogVoltage[2], solarCalc.analogVoltage[3]);
        }

        //function created to display the verified packet data 
        private void DisplayVerifiedData(int calChkSum1)
        {
            string parsedData = $"{newPacket.Length,-14}" +
                                       $"{newPacket.Substring(0, 3),-14}" +
                                       $"{newPacket.Substring(3, 3),-14}" +
                                       $"{newPacket.Substring(6, 4),-14}" +
                                       $"{newPacket.Substring(10, 4),-14}" +
                                       $"{newPacket.Substring(14, 4),-14}" +
                                       $"{newPacket.Substring(18, 4),-14}" +
                                       $"{newPacket.Substring(22, 4),-14}" +
                                       $"{newPacket.Substring(26, 4),-14}" +
                                       $"{newPacket.Substring(30, 4),-14}" +
                                       $"{newPacket.Substring(34, 3),-14}" +
                                       $"{calChkSum1,-17}" +
                                       $"{lostPacketCount,-16}" +
                                       $"{lostPacketCount,-10}" +
                                       $"{packetRollOver}" + "\r\n";




            //if box is checked, display the parsedData on the next line, thus keeping the previous parsed packet information displayed. Else, keep replacing the previous parsed packet information with the newPacket parsed information. 
            if (checkBoxParsedHistory.IsChecked == true)
            {
                labelParsedData.Text = parsedData + labelParsedData.Text;
            }
            else
            {
                labelParsedData.Text = parsedData;
            }
        }

        //function sends desired packet information out the serial port when the desired button is clicked. 
        private async void btnSend_Clicked(object sender, EventArgs e)
        {
            //try to send the information out the serial port. if it doesnt work, display the error message for the user. 
            try
            {
                string messageOut = entrySend.Text;
                messageOut += "\r\n";
                byte[] messageByte = Encoding.UTF8.GetBytes(messageOut);
                serialPort.Write(messageByte, 0, messageByte.Length);
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
            }


        }


        private void btnClear_Clicked(object sender, EventArgs e)
        {

        }

        //if any of the bit buttons are pressed, call the ButtonClicked function and modify the clicked buttons label and value. 
        private void btnBit3_Clicked(object sender, EventArgs e)
        {
            ButtonClicked(3);
        }

        private void btnBit2_Clicked(object sender, EventArgs e)
        {
            ButtonClicked(2);
        }

        private void btnBit1_Clicked(object sender, EventArgs e)
        {
            ButtonClicked(1);
        }

        private void btnBit0_Clicked(object sender, EventArgs e)
        {
            ButtonClicked(0);
        }

        //called when a button is clicked. Checks to see what the button of the value is equal to, then changes the value and text dependant on the checked value. After modifying the text and value of the button, a switch case displays the associated image, i.e., when 1, ledoff, when 0, ledon 
        private void ButtonClicked(int i)
        {
            Button[] btnBit = new Button[] { btnBit0, btnBit1, btnBit2, btnBit3, };
            if (btnBit[i].Text == "0")
            {
                btnBit[i].Text = "1";
                stringBuilderSend[i + 3] = '1';
                switch (i)
                {
                    case 0:
                        imgLED1.Source = "ledoff.png";
                        break;
                    case 1:
                        imgLED2.Source = "ledoff.png";
                        break;
                }
            }
            else
            {
                btnBit[i].Text = "0";
                stringBuilderSend[i + 3] = '0';
                switch (i)
                {
                    case 0:
                        imgLED1.Source = "ledon.png";
                        break;
                    case 1:
                        imgLED2.Source = "ledon.png";
                        break;
                }
            }
            sendPacket();
        }

        //function used to modify(change values within the string) the packet dependant on the values of bits 0-3 and then send out the modified packets value. 
        private void sendPacket()
        {
            int calSendChkSum = 0;
            try
            {

                for (int i = 3; i < stringBuilderSend.Length && i < 7; i++)
                {
                    calSendChkSum += (byte)stringBuilderSend[i];
                }
                calSendChkSum %= 1000;
                stringBuilderSend.Remove(7, 3);
                stringBuilderSend.Insert(7, calSendChkSum.ToString());
                string messageOut = stringBuilderSend.ToString();
                entrySend.Text = stringBuilderSend.ToString();
                messageOut += "\r\n";
                byte[] messageByte = Encoding.UTF8.GetBytes(messageOut);
                serialPort.Write(messageByte, 0, messageByte.Length);
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "OK");
            }

        }

        private void imgLED1_Clicked(object sender, EventArgs e)
        {
            ButtonClicked(0);
        }

        private void imgLED2_Clicked(object sender, EventArgs e)
        {
            ButtonClicked(1);
        }
    }
}