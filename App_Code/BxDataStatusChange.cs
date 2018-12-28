using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BlueXSOAP
{
    public class BxDataStatusChange : BxData
    {
        public BxDataStatusChange()
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
            Values.Add(new BxValue(@"isLegalEntity", @"string"));
            Values.Add(new BxValue(@"legalEntityName", @"string"));
            Values.Add(new BxValue(@"legalEntityGivenName", @"string"));
            Values.Add(new BxValue(@"legalEntityLastName", @"string"));
            Values.Add(new BxValue(@"businessRegistrationNumber", @"string"));
            Values.Add(new BxValue(@"taxNumber", @"string"));
            Values.Add(new BxValue(@"statusChange", @"string"));
            Values.Add(new BxValue(@"statusChangeReason", @"string"));
            Values.Add(new BxValue(@"dateTokenCompromise", @"string"));
            Values.Add(new BxValue(@"dateTokenLost", @"string"));
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