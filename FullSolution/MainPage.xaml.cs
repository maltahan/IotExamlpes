﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Lesson_201
{
    /// <summary>
    /// The application main page.  Because we are running headless we will not see anything
    /// even though it is begin generated at runtime.  This acts as the main entry point for the 
    /// application functionality.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // which GPIO pin do we want to use to control the LED light
        const int GPIOToUse = 23;

        // The class which wraps our LED.
        InternetLed internetLed1;
        InternetLed internetLed2;

        public MainPage()
        {
            this.InitializeComponent();
        }

        // This method will be called by the application framework when the page is first loaded.
        protected override async void OnNavigatedTo(NavigationEventArgs navArgs)
        {
            Debug.WriteLine("MainPage::OnNavigatedTo");

           // MakePinWebAPICall();

            try
            {
                // Create a new InternetLed object
                internetLed1 = new InternetLed(GPIOToUse);

                // Create a new InternetLed object
                internetLed2 = new InternetLed(18);

                // Initialize it for use
                internetLed1.InitalizeLed();
                internetLed2.InitalizeLed();
   

                // Now have it make the web API call and get the led blink delay
                int blinkDelay = await internetLed1.GetBlinkDelayFromWeb();
                int blinkDelay1 = await internetLed2.GetBlinkDelayFromWeb();

                for (int i = 0; i < 100; i++)
                {
                    internetLed1.Blink();
                    internetLed2.Blink();
                    //internetLed1.LedState = InternetLed.eLedState.On;
                    await Task.Delay(blinkDelay);
                    //internetLed1.LedState = InternetLed.eLedState.Off;
                    //internetLed2.LedState = InternetLed.eLedState.On;
                    //internetLed2.LedState = InternetLed.eLedState.Off;
                    
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        // This method will put your pin on the world map of makers using this lesson.
        // This uses imprecise location (for example, a location derived from your IP 
        // address with less precision such as at a city or postal code level). 
        // No personal information is stored.  It simply
        // collects the total count and other aggregate information.
        // http://www.microsoft.com/en-us/privacystatement/default.aspx
        // Comment out the line below to opt-out
        /// </summary>
        public void MakePinWebAPICall()
        {
            try {
                HttpClient client = new HttpClient();

                // Comment this line to opt out of the pin map.
                client.GetStringAsync("http://adafruitsample.azurewebsites.net/api?Lesson=201");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Web call failed: " + e.Message);
            }
        }

    }
}