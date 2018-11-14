using System;
using System.Collections.Generic;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;

namespace Edison.Devices.Common
{
    public class GPIOService : IDisposable
    {
        private readonly LoggingChannel _logging;
        private Dictionary<int, GpioPin> _pins;

        public GPIOService(Guid providerId)
        {
            _logging = new LoggingChannel("GPIOController", null, providerId);
            _pins = new Dictionary<int, GpioPin>();
        }

        public bool InitGPIOInput(int pinIndex, TypedEventHandler<GpioPin, GpioPinValueChangedEventArgs> callback)
        {
            
            try
            {
                GpioPin pin = GetPin(pinIndex);
                pin.SetDriveMode(GpioPinDriveMode.Input);
                pin.ValueChanged += callback;

                return true;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"InitGPIOInput: Error while trying to init gpio '{pinIndex}': {e.Message}", LoggingLevel.Error);
                throw e;
            }
        }

        public bool InitGPIOOutput(int pinIndex)
        {

            try
            {
                GpioPin pin = GetPin(pinIndex);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                pin.DebounceTimeout = TimeSpan.FromSeconds(1);
                pin.Write(GpioPinValue.High);

                return true;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"InitGPIOInput: Error while trying to init gpio '{pinIndex}': {e.Message}", LoggingLevel.Error);
                throw e;
            }
        }

        private GpioPin GetPin(int pinIndex)
        {
            GpioPin pin = null;
            var gpioController = GetGPIOController();
            if (gpioController != null)
            {
                if (!_pins.ContainsKey(pinIndex))
                {
                    try
                    {
                        pin = gpioController.OpenPin(pinIndex);
                        if (pin == null)
                            _logging.LogMessage($"There is no GPIO pin with index '{pinIndex}'.", LoggingLevel.Error);
                        else
                            _pins.Add(pinIndex, pin);
                    }
                    catch(Exception e)
                    {
                        _logging.LogMessage($"GetPin: Error while trying to open gpio '{pinIndex}': {e.Message}", LoggingLevel.Error);
                        throw e;
                    }
                }
                else
                    pin = _pins[pinIndex];
            }
            return pin;
        }

        private GpioController GetGPIOController()
        {
            var gpioController = GpioController.GetDefault();

            if (gpioController == null)
                _logging.LogMessage("There is no GPIO controller on this device.", LoggingLevel.Warning);

            return gpioController;
        }

        public void PinSetHigh(int pinIndex)
        {
            GpioPin pin = GetPin(pinIndex);
            if(pin != null)
                pin.Write(GpioPinValue.High);
        }

        public void PinSetLow(int pinIndex)
        {
            GpioPin pin = GetPin(pinIndex);
            if (pin != null)
                pin.Write(GpioPinValue.Low);
        }

        public void Dispose()
        {
            try
            {
                foreach (var pin in _pins.Values)
                {
                    if (pin != null)
                    {
                        pin.Write(GpioPinValue.High);
                        pin.Dispose();
                    }
                }

                _pins = new Dictionary<int, GpioPin>();
            }
            catch (Exception e)
            {
                _logging.LogMessage($"ClearPins: {e.Message}", LoggingLevel.Error);
                _logging.LogMessage(e.StackTrace, LoggingLevel.Error);
            }
        }
    }
}
