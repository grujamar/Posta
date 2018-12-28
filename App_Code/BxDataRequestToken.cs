using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueXSOAP
{
    public class BxDataRequestToken : BxData
    {
        public BxDataRequestToken()
            : base()
        {
            createBxData();
        }

        public override void createBxData()
        {
            Values.Add(new BxValue(@"productionProfile", @"string"));
            Values.Add(new BxValue(@"media", @"string"));
            Values.Add(new BxValue(@"validity", @"string"));
            Values.Add(new BxValue(@"givenName", @"string"));
            Values.Add(new BxValue(@"lastName", @"string"));
            Values.Add(new BxValue(@"identificationDocumentNumber", @"string"));
            Values.Add(new BxValue(@"identificationDocumentType", @"string"));
            Values.Add(new BxValue(@"identificationDocumentValidUntil", @"string"));
            Values.Add(new BxValue(@"identificationDocumentValidFrom", @"string"));
            Values.Add(new BxValue(@"identificationIssuer", @"string"));
            Values.Add(new BxValue(@"identificationIssuerName", @"string"));
            Values.Add(new BxValue(@"includeUniqueCitizensNumber", @"string"));
            Values.Add(new BxValue(@"uniqueCitizensNumber", @"string"));
            Values.Add(new BxValue(@"emailAddress", @"string"));
            Values.Add(new BxValue(@"phoneNumber", @"string"));
            Values.Add(new BxValue(@"dateOfBirth", @"string"));
            Values.Add(new BxValue(@"distributionCity", @"string"));
            Values.Add(new BxValue(@"distributionStreet", @"string"));
            Values.Add(new BxValue(@"distributionHouseNumber", @"string"));
            Values.Add(new BxValue(@"distributionPostalCode", @"string"));
            Values.Add(new BxValue(@"distributionPAK", @"string"));         
            Values.Add(new BxValue(@"deliveryLocation", @"string"));
            Values.Add(new BxValue(@"paymentMethod", @"string"));
            Values.Add(new BxValue(@"totalPrice", @"string"));
            Values.Add(new BxValue(@"legalEntityOrderNumber", @"string"));
            Values.Add(new BxValue(@"legalEntityName", @"string"));
            Values.Add(new BxValue(@"legalEntityGivenName", @"string"));
            Values.Add(new BxValue(@"legalEntityLastName", @"string"));
            Values.Add(new BxValue(@"businessRegistrationNumber", @"string"));
            Values.Add(new BxValue(@"taxNumber", @"string"));
            Values.Add(new BxValue(@"addedTax", @"string"));
            Values.Add(new BxValue(@"legalEntityActivityCode", @"string"));  //sifra delatnosti
            Values.Add(new BxValue(@"legalEntityCity", @"string"));
            Values.Add(new BxValue(@"legalEntityStreet", @"string"));
            Values.Add(new BxValue(@"legalEntityHouseNumber", @"string"));
            Values.Add(new BxValue(@"legalEntityPostalCode", @"string"));
            Values.Add(new BxValue(@"legalEntityPAK", @"string"));
            Values.Add(new BxValue(@"legalEntityPhoneNumber", @"string"));
            Values.Add(new BxValue(@"legalEntityEmail", @"string"));
            Values.Add(new BxValue(@"legalEntityURL", @"string"));
            Values.Add(new BxValue(@"legalEntityTotalPrice", @"string"));
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
