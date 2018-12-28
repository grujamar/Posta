using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SerialNoVariable
{
    public string PropertyName { get; set; }
    public int PropertyValue { get; set; }

    public SerialNoVariable(string propertyname, int propertyvalue)
    {
        PropertyName = propertyname;
        PropertyValue = propertyvalue;
    }
}