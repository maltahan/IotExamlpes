using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Lesson_201
{
    /// <summary>
    /// A class which wraps a GPIO interfaced LED which can get it's display state set based on
    /// a web API.
    /// </summary>
    class InternetLed
    {
        // GPIO controller code
        private GpioController gpio;
        private GpioPin LedControlGPIOPin;
        private int LedControlPin;

        // only used if we don't get a response from the webapi call.
        private const int DefaultBlinkDelay = 1000;

        // An enumeration to store the state of the led
        public enum eLedState { Off, On };
        private eLedState _LedState;

        public InternetLed(int ledControlPin)
        {
            Debug.WriteLine("InternetLed::New InternetLed");

            // Store the selected GPIO control pin id for use when we initialize the GPIO
            LedControlPin = ledControlPin;
        }


        public void InitalizeLed()
        {
            Debug.WriteLine("InternetLed::InitalizeLed");

            // Now setup the LedControlPin
            //Gets the default general-purpose I/O (GPIO) controller for the system
            gpio = GpioController.GetDefault();
            // Opens a connection to the specified general-purpose I/O (GPIO) pin in exclusive mode
            LedControlGPIOPin = gpio.OpenPin(LedControlPin);
            /*Sets the drive mode of the general-purpose I/O (GPIO) pin. 
            The drive mode specifies whether the pin is configured as an input or an output, 
            and determines how values are driven onto the pin.*/
            //Configures the GPIO pin in strong drive mode, with low impedance
            LedControlGPIOPin.SetDriveMode(GpioPinDriveMode.Output);

            // Get the current pin value
            GpioPinValue startingValue = LedControlGPIOPin.Read();
            _LedState = (startingValue == GpioPinValue.Low) ? eLedState.On : eLedState.Off;
        }

        // A public property for interacting with the LED from code.
        public eLedState LedState
        {
            get { return _LedState; }
            set
            {
                Debug.WriteLine("InternetLed::LedState::set " + value.ToString());
                if (LedControlGPIOPin != null)
                {
                    GpioPinValue newValue = (value == eLedState.On ? GpioPinValue.High : GpioPinValue.Low);
                    // Turn the light  on or off 
                    LedControlGPIOPin.Write(newValue);
                    _LedState = value;
                }
            }
        }

        // Change the state of the led from on to off or off to on
        public void Blink()
        {
            if (LedState == eLedState.On)
            {
                LedState = eLedState.Off;
            }
            else
            {
                LedState = eLedState.On;
            }
        }

        // This will call an exposed web API at the indicated URL
        // the API will return a string value to use as the blink delay.
        const string WebAPIURL = "http://adafruitsample.azurewebsites.net/TimeApi";
        public async Task<int> GetBlinkDelayFromWeb()
        {
            Debug.WriteLine("InternetLed::MakeWebApiCall");

            string responseString = "No response";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Make the call
                    responseString = await client.GetStringAsync(WebAPIURL);

                    // Let us know what the returned string was
                    Debug.WriteLine(String.Format("Response string: [{0}]", responseString));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            int delay;

            if (!int.TryParse(responseString, out delay))
            {
                delay = DefaultBlinkDelay;
            }

            // return the blink delay
            return delay;
        }

    }

}