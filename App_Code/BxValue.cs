using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueXSOAP
{
    public class BxValue
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public BxValue(string name, string type)
        { 
            setAttributes(name, type);
        }

        public override string ToString()
        {
            //example <bx:value name="taxNumber" type="string">123456789</bx:value>
            return @"<bx:value name=""" + Name + @""" type=""" + Type + @""">" + Value + @"</bx:value>";
        }

        public void setAttributes(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
