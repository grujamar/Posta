using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class Rezultat
    {
        public string Poruka { get; set; }//Sadrži stvaran razlog zašto transakcija nije uspela.
        public string PorukaKorisnik { get; set; }//Sadrži poruku koju treba prikazati krajnjem korisniku.
        public string Info { get; set; }//Sadrži dodatnu informaciju o rezultatu

}
