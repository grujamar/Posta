using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueXSOAP
{
    public class BxDataPKCS12 : BxData
    {
        public BxDataPKCS12()
            : base()
        {
            createBxData();
        }

        public override void createBxData()
        {
            Values.Add(new BxValue(@"requestNumber", @"string"));
            Values.Add(new BxValue(@"downloadAuthorizationCode", @"string"));
            Values.Add(new BxValue(@"sendingMethod", @"string"));
        }
    }
}