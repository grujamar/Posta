using BlueXSOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BxDataCertificateStatusCheck : BxData
{
    public BxDataCertificateStatusCheck()
        : base()
    {
        createBxData();
    }

    public override void createBxData()
    {
        Values.Add(new BxValue(@"USI", @"string"));
    }
}