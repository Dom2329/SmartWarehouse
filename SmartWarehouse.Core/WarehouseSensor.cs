using System;

namespace SmartWarehouse.Core
{
    public class WarehouseSensor
    {
        //----------------------------------------properties-----------------------

        public string SensorId { get; private set; }//--------------id sensor---------------------
        public string LocationTag { get; private set; }//--------------------------location------------------------------
        public double CurrentTemperature { get; private set; }//------------------------storage for last reading--------


        public bool IsActive { get; private set; }//------------------------on or off------------------
        public bool IsAlertTriggered { get; private set; }//-------------------alert active?----------------------------------


        public double CriticalThresholdCelsius { get; private set; }//-------------------------------

        //----------------------------------------constructor-----------------------

        public WarehouseSensor(string sensorId, string locationTag, double criticalThreshold = 4.0)
        {
            if (string.IsNullOrWhiteSpace(sensorId))
            {
                throw new ArgumentException("sensor ID can't be empty");
            }

            if (string.IsNullOrWhiteSpace(locationTag))
            {
                throw new ArgumentException("location can't be empty");
            }

            if (criticalThreshold < -30.0 || criticalThreshold > 50.0 || double.IsNaN(criticalThreshold))
            {
                throw new ArgumentOutOfRangeException(nameof(criticalThreshold));
            }

            SensorId = sensorId;
            LocationTag = locationTag;
            CriticalThresholdCelsius = criticalThreshold;

            CurrentTemperature = 0.0;
            IsActive = false;
            IsAlertTriggered = false;
        }

        //----------------------------------------activate method-----------------------

        public void Activate()
        {
            IsActive = true;
        }

        //----------------------------------------deactivate method-----------------------

        public void Deactivate()
        {
            IsActive = false;
            IsAlertTriggered = false;
        }

        //----------------------------------------record reading method-----------------------

        public void RecordReading(double newTemperature)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Sensor isn't active");
            }

            if (newTemperature < -50.0 || newTemperature > 80.0 || double.IsNaN(newTemperature))
            {
                throw new ArgumentOutOfRangeException(nameof(newTemperature));
            }

            CurrentTemperature = newTemperature;

            if (newTemperature >= CriticalThresholdCelsius)
            {
                IsAlertTriggered = true;
            }
            else
            {
                IsAlertTriggered = false;
            }
        }

        //----------------------------------------update threshold method-----------------------

        public void UpdateThreshold(double newThreshold)
        {
            if (newThreshold < -30.0 || newThreshold > 50.0 || double.IsNaN(newThreshold))
            {
                throw new ArgumentOutOfRangeException(nameof(newThreshold));
            }

            CriticalThresholdCelsius = newThreshold;

            if (IsActive && CurrentTemperature >= CriticalThresholdCelsius)
            {
                IsAlertTriggered = true;
            }
            else
            {
                IsAlertTriggered = false;
            }
        }
    }
}
