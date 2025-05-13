## 🖥️ Meadow Solar Lab - Serial Communication & Data Visualization
This app connects to a solar system through a serial port, collects real-time data, and visualizes the solar and battery voltages in a dynamic graph. It offers both historical data tracking and packet validation functionalities, with options to send and receive custom data over serial communication.

### ✨ Features
- Serial communication with a solar system via the selected COM port

- Real-time data display: solar voltage, battery voltage, and current

- Dynamic graph updating every 16ms to show solar and battery voltages

- Data history display: view received data as a log or overwrite with each packet

- LED control: toggle LED indicators and visualize the status with custom messages

- Error detection: packet checksum verification and lost packet count

### 🖼️ UI Overview
- Serial Port Picker: Allows you to select the COM port for communication

- Open/Close Button: Opens and closes the selected port

- Data Display: Real-time monitoring of solar voltage, battery voltage, and current

- Graph: Live graph of the solar and battery voltages

- LED Controls: Interactive buttons to control and visualize LED states

- History Checkbox: Displays data history or overwrites with the latest packet

### 🔧 Tech Stack
- .NET MAUI

- C#

- XAML

- Serial Communication via System.IO.Ports

- Data Visualization via Custom Drawable in XAML

### 🚀 How to Run
- Clone this repo

- Open in Visual Studio 2022+ 

- Connect the app to a serial device for real-time data collection

- Run the project on Windows or a compatible device with serial port access
