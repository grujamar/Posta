using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueXSOAP
{
    public class BxDataRequestStatus : BxData
    {
        public BxDataRequestStatus()
            : base()
        {
            createBxData();
        }

        public override void createBxData()
        {
            Values.Add(new BxValue(@"requestNumber", @"string"));
        }
    }
}