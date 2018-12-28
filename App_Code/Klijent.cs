using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;

    public class Klijent  
    {
        public string Username { get; set; } // definisaćemo kasnije
        public string Password { get; set; } // definisaćemo kasnije
        public string Jezik { get; set; } // LAT
        public int IdTipUredjaja { get; set; } // 1
        public string ModelUredjaja { get; set; }
        public string NazivUredjaja { get; set; } // max 50, nije obavezan
        public string VerzijaOS { get; set; } // max 50, nije obavezan
        public string VerzijaAplikacije { get; set; } // max 50, nije obavezan
        public string IPAdresa { get; set; } // max 50, nije obavezan
        public string Geolokacija { get; set; } // max 50, nije obavezan
        
    }

