
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartWarehouse.Core;
using System;

namespace SmartWarehouse.Tests
{
    [TestClass]
    public class WarehouseSensorTests
    {
        //----------------------------------------constructor test----------------------------------------------

        [TestMethod]
        public void Constructor_ShouldSetDefaultValues()
        {
            // Arrange
            string id = "Sensor:01";
            string location = "Cold-Vault:01";

            // Act
            WarehouseSensor sensor = new WarehouseSensor(id, location);

            // Assert
            Assert.AreEqual(id, sensor.SensorId);
            Assert.AreEqual(location, sensor.LocationTag);
            Assert.AreEqual(0.0, sensor.CurrentTemperature);

            Assert.IsFalse(sensor.IsActive);
            Assert.IsFalse(sensor.IsAlertTriggered);
            Assert.AreEqual(4.0, sensor.CriticalThresholdCelsius);
        }

        //----------------------------------------invalid id test-----------------------------------------------------
        
        [DataTestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Constructor_InvalidId_ShouldThrow(string id)
        {
            // Arrange
            string location = "Zone:01";

            // Act
            Action action = () => new WarehouseSensor(id, location);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        //----------------------------------------invalid location test--------------------------------------------------------------------

        [DataTestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Constructor_InvalidLocation_ShouldThrow(string location)
        {
            // Arrange
            string id = "Sensor:01";

            // Act
            Action action = () => new WarehouseSensor(id, location);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        //----------------------------------------activate test---------------------------------------------

        [TestMethod]
        public void Activate_ShouldTurnSensorOn()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone:01");

            // Act
            sensor.Activate();
            sensor.Activate();

            // Assert
            Assert.IsTrue(sensor.IsActive);
        }

        //----------------------------------------deactivate test------------------------------------------------------------

        [TestMethod]
        public void Deactivate_ShouldClearAlert()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone:01");
            sensor.Activate();
            sensor.RecordReading(10.0);

            // Act
            sensor.Deactivate();

            // Assert
            Assert.IsFalse(sensor.IsActive);
            Assert.IsFalse(sensor.IsAlertTriggered);
        }

        //----------------------------------------inactive sensor test-----------------------

        [TestMethod]
        public void RecordReading_Inactive_ShouldThrow()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone:01");

            // Act
            Action action = () => sensor.RecordReading(5.0);

            // Assert
            Assert.ThrowsException<InvalidOperationException>(action);
        }

        //----------------------------------------alert tests------------------------------------------------------

        [DataTestMethod]
        [DataRow(3.9, false)]
        [DataRow(4.0, true)]
        [DataRow(4.1, true)]
        public void RecordReading_ShouldUpdateAlert(double temperature, bool expected)
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone-01");
            sensor.Activate();

            // Act
            sensor.RecordReading(temperature);

            // Assert
            Assert.AreEqual(temperature, sensor.CurrentTemperature);

            Assert.AreEqual(expected, sensor.IsAlertTriggered);
        }

        //----------------------------------------valid boundary tests--------------------------------------------------------

        [DataTestMethod]
        [DataRow(-50.0)]
        [DataRow(80.0)]
        public void RecordReading_ValidBoundary_ShouldWork(double temperature)
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone-01");
            sensor.Activate();

            // Act
            sensor.RecordReading(temperature);

            // Assert
            Assert.AreEqual(temperature, sensor.CurrentTemperature);
        }





        //----------------------------------------invalid boundary tests------------------------------------

        [DataTestMethod]
        [DataRow(-50.1)]
        [DataRow(80.1)]
        public void RecordReading_InvalidBoundary_ShouldThrow(double temperature)
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone:01");
            sensor.Activate();

            // Act
            Action action = () => sensor.RecordReading(temperature);

            // Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        }

        //----------------------------------------threshold update test-----------------------------------------------

        [TestMethod]
        public void UpdateThreshold_ShouldTriggerAlert()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone:01");
            sensor.Activate();
            sensor.RecordReading(3.0);

            // Act
            sensor.UpdateThreshold(2.0);

            // Assert
            Assert.AreEqual(2.0, sensor.CriticalThresholdCelsius);
            Assert.IsTrue(sensor.IsAlertTriggered);
        }
        
        //----------------------------------------threshold clear test--------------------------------------------------------------

        [TestMethod]
        public void UpdateThreshold_ShouldClearAlert()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone:01");
            sensor.Activate();
            sensor.RecordReading(5.0);

            // Act
            sensor.UpdateThreshold(6.0);

            // Assert
            Assert.IsFalse(sensor.IsAlertTriggered);
        }

        //----------------------------------------invalid threshold tests-------------------------------------------

        [DataTestMethod]

        [DataRow(-30.1)]

        [DataRow(50.1)]
        public void UpdateThreshold_InvalidValue_ShouldThrow(double threshold)
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("S1", "Zone:01");

            // Act
            Action action = () => sensor.UpdateThreshold(threshold);

            // Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        }
    }
}

