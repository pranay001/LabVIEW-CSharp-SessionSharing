using System;
using System.Threading;
using NationalInstruments.ModularInstruments.NIDCPower;


namespace TestProgram
{
    public static class SupplyTest
    {
        public static double[] StressTest(NIDCPower supplySession, string channelList, double voltageLevel)
        {
            //configure the supply function to DC Voltage and source the voltage provided as input
            supplySession.Control.Abort();
            supplySession.Source.Mode = DCPowerSourceMode.SinglePoint;
            supplySession.Outputs[channelList].Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
            supplySession.Outputs[channelList].Source.Voltage.VoltageLevel = voltageLevel;
            supplySession.Outputs[channelList].Source.Voltage.CurrentLimit = 0.1;

            // initiate the session and enable the output
            supplySession.Control.Initiate();
            supplySession.Outputs[channelList].Source.Output.Enabled = true;

            // apply the stress voltage for 3 seconds
            Thread.Sleep(3000);

            // measure the supply currents at the end of stress interval
            var stressCurrent = supplySession.Measurement.Measure(channelList).CurrentMeasurements;

            // abort the session and disable the output
            supplySession.Control.Abort();
            supplySession.Outputs[channelList].Source.Output.Enabled = false;

            return stressCurrent;
        }

        public static double[] LV_StressTest(ulong sessionHandle, string channelList, double voltageLevel)
        {
            // Create a new instrument driver session from the existing session handle
            var dcpowerDotNetSession = new NIDCPower((IntPtr)sessionHandle);

            // call the stress test method
            var stressCurrent = StressTest(dcpowerDotNetSession, channelList, voltageLevel);

            // close and dispose the handled created in this .net method call
            dcpowerDotNetSession.Close();
            dcpowerDotNetSession.Dispose();

            return stressCurrent;
        }
    }
}
