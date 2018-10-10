using System;
using System.Collections.Generic;

namespace Edison.Simulators.CSVGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            double longMin = 2.130000;
            double longMax = 2.160000;
            double latMin = 41.400000;
            double latMax = 41.406000;
            int nbrLon = 10;
            int nbrLat = 4;
            double periodLon = (longMax - longMin) / (double)nbrLon;
            double periodLat = (latMax - latMin) / (double)nbrLat;
            List<string> lines = new List<string>();
            lines.Add("DeviceId,DeviceType,LocationName,LocationLevel1,LocationLevel2,LocationLevel3,Longitude,Latitude,Sensor,Demo");

            for(int i = 0; i <= nbrLon; i++)
            {
                var longitude = Math.Round(longMin + (i * periodLon), 6);
                for (int j = 0; j <= nbrLat; j++)
                {
                    var latitude = Math.Round(latMin + (j * periodLat), 6);
                    lines.Add($"{Guid.NewGuid()},ButtonSensor,[{i}][{j}],,,,{longitude},{latitude},TRUE,TRUE");
                    lines.Add($"{Guid.NewGuid()},Lightbulb,[{i}][{j}],,,,{longitude},{latitude},FALSE,TRUE");
                }
            }
            System.IO.File.WriteAllLines("sensors_test.csv", lines);
        }
    }
}
