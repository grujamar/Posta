using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GetUlica
/// </summary>
public class GetUlicaIn
{
    public int IdNaselje { get; set; }
    public string Naziv { get; set; } //Max dužina 50
    public int BrojRezultata { get; set; }
    public bool PoredjenjePoDeluNaziva { get; set; }
}