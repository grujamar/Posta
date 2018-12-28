using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WordToPdf;

/// <summary>
/// Summary description for TemplateDocumentIssuingIndividualPaymentOrder
/// </summary>
public class TemplateDocumentIssuingIndividualPaymentOrder : WordToPdf.WordTemplateDocument
{
    public TemplateDocumentIssuingIndividualPaymentOrder(string wordSaveAs, string firstName, string lastName, string jik, string price, string Date, bool alsoCreatePdf, string pdfSaveDocumentAsFullPath)
: base()
    {
        //napravljen je prazan template dokument objekat
        //ovde sada treba da se definisu podaci kao sto su putanja dokumenta, na osnovu kog template-a je napravljen, lista bukmarka
        FullName = wordSaveAs;
        Template.FullName = pdfSaveDocumentAsFullPath;

        Bookmarks.Add(new Bookmark(@"Payer", firstName + " " + lastName));
        Bookmarks.Add(new Bookmark(@"Date", Date));
        Bookmarks.Add(new Bookmark(@"RequestNo", jik));
        Bookmarks.Add(new Bookmark(@"Price", price));
        Bookmarks.Add(new Bookmark(@"RequestNo1", jik));
    }
}