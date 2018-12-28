using BlueXSOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class BxDataChallengeResponse : BxData
{
    public BxDataChallengeResponse()
        : base()
    {
        createBxData();
    }

    public override void createBxData()
    {
        Values.Add(new BxValue(@"requestNumber", @"string")); 
        Values.Add(new BxValue(@"challenge", @"string"));
    }
}