using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueXSOAP
{
    public abstract class BxData
    {
        public List<BxValue> Values { get; set; }

        public BxData()
        {
            Values = new List<BxValue>();
        }

        public override string ToString()
        {
            string ret = @"<bx:data>";
            foreach (BxValue value in Values)
            {
                ret += value.ToString();
            }
            ret += @"</bx:data>";

            return ret;
        }

        public BxValue getBxValue(string name)
        {
            foreach (var v in Values)
            {
                if (v.Name.Equals(name))
                {
                    return v;
                }
            }
            return null;
        }

        public void setValue(string name, string value)
        {
            getBxValue(name).Value = value;
        }

        public abstract void createBxData();
    }
}
