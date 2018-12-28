using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueXSOAP
{
    public class BxDataUnblockMedium : BxData
    {
        public BxDataUnblockMedium()
            : base()
        {
            createBxData();
        }

        public override void createBxData()
        {
            Values.Add(new BxValue(@"USI", @"string"));
            Values.Add(new BxValue(@"givenName", @"string"));
            Values.Add(new BxValue(@"lastName", @"string"));
            Values.Add(new BxValue(@"uniqueCitizensNumber", @"string"));
            Values.Add(new BxValue(@"emailAddress", @"string"));
            Values.Add(new BxValue(@"phoneNumber", @"string"));
            Values.Add(new BxValue(@"paymentMethod", @"string"));
            Values.Add(new BxValue(@"requestChallengeResponse", @"string"));
            //Values.Add(new BxValue(@"deliveryLocation", @"string"));
            Values.Add(new BxValue(@"distributionCity", @"string"));
            Values.Add(new BxValue(@"distributionStreet", @"string"));
            Values.Add(new BxValue(@"distributionHouseNumber", @"string"));
            Values.Add(new BxValue(@"distributionPostalCode", @"string"));
            Values.Add(new BxValue(@"distributionPAK", @"string"));
            Values.Add(new BxValue(@"tokenSerialNumber", @"string"));
            Values.Add(new BxValue(@"totalPrice", @"string"));
            Values.Add(new BxValue(@"ipApplicant", @"string"));
            Values.Add(new BxValue(@"continentApplicant", @"string"));
            Values.Add(new BxValue(@"countryApplicant", @"string"));
            Values.Add(new BxValue(@"countryCodeApplicant", @"string"));
            Values.Add(new BxValue(@"cityApplicant", @"string"));
            Values.Add(new BxValue(@"osApplicant", @"string"));
            Values.Add(new BxValue(@"ispApplicant", @"string"));
            Values.Add(new BxValue(@"browserApplicant", @"string"));
            Values.Add(new BxValue(@"userAgentStringApplicant", @"string"));
            Values.Add(new BxValue(@"ipOperator", @"string"));
        }
    }
}