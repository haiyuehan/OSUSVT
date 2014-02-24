﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS
{

    class GPSclass
    {

        private

            float _latitude = 0;     //Data read from the GPRMC output from GPS.
            float _longitude = 0;    //Data read from the GPRMC output from GPS.
            int _sat_count = 0;      //Number of Sattelites AKA NumInUse (from GPGGA)
            float _velocity = 0;     //Velocity deteriorated to MPH. We really ought to change this to the motorcontroller.
            float _altitude = 0;     //Altitude (from GPGGA) above or below MEAN sea level data (taking into account the earth's ellipsoid shape) in METERS
            string _bearing = "N/A"; //
            int _quality = 0;        //GPS quality indicator (0=invalid; 1=GPS fix; 2=Diff. GPS fix) (from GPGGA)
            bool _NoS = false;
            bool _EoW = false;
            string _UTC = "N/A";     //Time at position of GPS.

            public GPSclass()
            {

            }

            public float latitude
            {
                get{ return _latitude; }
                set{ _latitude = value; }
            }

            public float longitude
            {
                get{ return _longitude; }
                set{ _longitude = value; }
            }

            public int sat_count
            {
                get{ return _sat_count; }
                set{ _sat_count = (int)value; }
            }

            public float velocity
            {
                get{ return _velocity; }
                set{ _velocity = value; }
            }

            public float altitude
            {
                get { return _altitude; }
                set { _altitude = value; }
            }

            public string bearing
            {
                get { return _bearing; }
                set { _bearing = value; }
            }

            public int quality
            {
                get { return _quality; }
                set { _quality = value; }
            }

            bool open_ports()
            {
                return false;
            }
    }
}