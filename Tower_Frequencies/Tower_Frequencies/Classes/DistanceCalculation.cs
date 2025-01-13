using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Frequencies.Classes
{
    public class DistanceCalculation
    {

        // Calculate the great-circle distance distance between Towers (Large using longitude(Y) and latitude(X)) (Using Haversine Formula for considering Earth's curvature)
        // Based on info from https://stackoverflow.com/questions/41621957/a-more-efficient-haversine-function and https://www.movable-type.co.uk/scripts/latlong.html
        public static double CalDistance(double dTowerOne_longitude, double dTowerOne_latitude, double dTowerTwo_longitude, double dTowerTwo_latitude)
        {
            // Final great-circle distance result in meters
            double dDistance = 0;

            // Variables used to Compute Haversine Formula
            double dHalf_Chord_Length_Squared = 0;
            double dAngular_Distance = 0;

            // Earths Radius
            const double dEarth_Radius = 6378100; // Meters

            // Convert given longitude and latitude degrees to radians (Standard unit of angular measurement --> degree * (pi / 180) )
            dTowerOne_longitude = dTowerOne_longitude * (Math.PI / 180);
            dTowerOne_latitude = dTowerOne_latitude * (Math.PI / 180);
            dTowerTwo_longitude = dTowerTwo_longitude * (Math.PI / 180);
            dTowerTwo_latitude = dTowerTwo_latitude * (Math.PI / 180);

            //Sine of half the differences between longitude and latitudes --> Sin(ΔRadian / 2)
            double shdLatitude = Math.Sin((dTowerTwo_latitude - dTowerOne_latitude) / 2);
            double shdLongitude = Math.Sin((dTowerTwo_longitude - dTowerOne_longitude) / 2);

            // Calculated the half chord lenght squared --> sin²(Δlat / 2) + cos(lat1) * cos(lat2) * sin²(Δlon / 2)
            dHalf_Chord_Length_Squared = shdLatitude * shdLatitude + Math.Cos(dTowerOne_latitude) * Math.Cos(dTowerTwo_latitude) * shdLongitude * shdLongitude;

            // Calculate the angular distance between two points in radians --> 2 * atan2(√a, √(1-a))
            dAngular_Distance = 2 * Math.Atan2(Math.Sqrt(dHalf_Chord_Length_Squared), Math.Sqrt(1 - dHalf_Chord_Length_Squared));

            // Finaly Calculate the great-circle distance in meters --> earth_radius * angular distance
            dDistance = dEarth_Radius * dAngular_Distance;


            // Return the distance between points in meters
            return dDistance;
        }


    }
}
