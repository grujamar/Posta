using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ProveraAdreseIn
/// </summary>
public class ProveraAdreseIn
{
    public int TipAdrese;//0 preuzimanje, 1 pošiljalac, 2 primalac
    public int IdRukovanje;
    public int IdNaselje;
    public int IdUlica;
    public string BrojPodbroj; //Max dužina 9
    public string Posta; //Max dužina 5
}