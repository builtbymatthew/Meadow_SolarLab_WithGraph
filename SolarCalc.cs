using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarLabRight2023
{
    internal class SolarCalc
    {

        public double[] analogVoltage = new double[6]; //variable to store values of A0-A5

        //function that accepts two adc values (voltages), calculates the potential difference between then and divides the difference by the resistor value to get the current. It then returns the current value in the form of a string
        internal string GetCurrent(double an2, double shuntResistorAnalog2)
        {
            double current = (an2 - shuntResistorAnalog2) / 100;
            return current.ToString(" 0.0 mA;-0.0 mA; 0.0 mA");
        }


        //function that accepts two adc values (voltages), calculates the potential difference between then and divides the difference by the resistor value to get the current. It then returns the current value in the form of a string
        //note:this function is the same as the GetCurrent function but in future labs we may need to perform different calculations for the LEDCurrent so keep both functions. 
        internal string GetLEDCurrent(double an1, double shuntResistorAnalog)
        {
            double current = (an1 - shuntResistorAnalog) / 100;
            return current.ToString(" 0.0 mA;-0.0 mA; 0.0 mA");
        }

        //function that accepts an analog voltage in mV, divides it by 1000 to get the value in volts, and then converts it to a string and returns it. 
        internal string GetVoltageString(double analogValue)
        {
            double analogVolt = analogValue / 1000;
            return analogVolt.ToString(" 0.0 V");
        }

        //function that parses the incoming newPacket into different components such as length, analog values, etc, and then takes the parsed analog data and stores it in an array called analogVoltage. 
        internal void ParseSolarData(string newPacket)
        {
            string parsedData = $"{newPacket.Length,-14}" +
                                   $"{newPacket.Substring(0, 3),-14}" +
                                   $"{newPacket.Substring(3, 3),-14}" +
                                   $"{newPacket.Substring(6, 4),-14}" + //analo0
                                   $"{newPacket.Substring(10, 4),-14}" +
                                   $"{newPacket.Substring(14, 4),-14}" +
                                   $"{newPacket.Substring(18, 4),-14}" +
                                   $"{newPacket.Substring(22, 4),-14}" + //analog4
                                   $"{newPacket.Substring(26, 4),-14}" +
                                   $"{newPacket.Substring(30, 4),-14}" +
                                   $"{newPacket.Substring(34, 3),-14}" +

                                   "\r\n";

            for (int i = 0; i < 6; i++)
            {

                analogVoltage[i] = Convert.ToDouble(newPacket.Substring(6 + i * 4, 4));
            }



        }
    }
}
