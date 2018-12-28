using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WordToPdf;

/// <summary>
/// Summary description for TemplateDocumentIssuingIndividual
/// </summary>
public class TemplateDocumentIssuingIndividual: WordToPdf.WordTemplateDocument
{
    public TemplateDocumentIssuingIndividual(string wordSaveAs, string firstName, string lastName, string jmbg, string jik, string email, string street, string streetNo, string postNo, string place, string phone, bool alsoCreatePdf, string pdfSaveDocumentAsFullPath)
: base()
    {
        //napravljen je prazan template dokument objekat
        //ovde sada treba da se definisu podaci kao sto su putanja dokumenta, na osnovu kog template-a je napravljen, lista bukmarka
        FullName = wordSaveAs;
        Template.FullName = pdfSaveDocumentAsFullPath;
        
        Bookmarks.Add(new Bookmark(@"FirstName", firstName));
        Bookmarks.Add(new Bookmark(@"LastName", lastName));
        Bookmarks.Add(new Bookmark(@"Jmbg", jmbg));
        Bookmarks.Add(new Bookmark(@"Jik", jik));
        Bookmarks.Add(new Bookmark(@"Email", email));
        Bookmarks.Add(new Bookmark(@"Street", street));
        Bookmarks.Add(new Bookmark(@"StreetNo", streetNo));
        Bookmarks.Add(new Bookmark(@"PostNo", postNo));
        Bookmarks.Add(new Bookmark(@"Place", place));
        Bookmarks.Add(new Bookmark(@"Phone", phone));
    }
}