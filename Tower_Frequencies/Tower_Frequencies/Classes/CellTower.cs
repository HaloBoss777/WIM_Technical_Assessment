using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Frequencies.Classes
{
    //Create Cell Tower Class, containing all relevant information
    public class CellTower
    {
        // The identifier of the Tower
        public string Tower_Name { get; set; }
        // The easting coordinate (UTM coordinate system)
        public double Easting { get; set; }
        // The northing coordinate (UTM coordinate system)
        public double Northing { get; set; }
        // Geographic longitude
        public double Longitude { get; set; }
        // Geographic latitude
        public double Latitude { get; set; }
        // Frequency used by the tower
        public int Frequency { get; set; }

        // Default Constructor
        public CellTower()
        {
            Tower_Name = "Unknown";
        }

        // Parameterized Constructor
        public CellTower(string tower_name = "Unknow", double easting = 0, double northing = 0, double longitude = 0, double latitude = 0, int frequency = 0)
        {
            this.Tower_Name = tower_name;
            this.Easting = easting;
            this.Northing = northing;
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.Frequency = frequency;
        }

        public override string ToString()
        {
            return $"Tower Name: {Tower_Name}\tEasting: {Easting}\tNorthing: {Northing}\tLongitude: {Longitude}\tLatitude: {Latitude}\tFrequency: {Frequency}";
        }


    }
}
