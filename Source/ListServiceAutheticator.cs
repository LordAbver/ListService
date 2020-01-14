using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;


namespace Harris.Automation.ADC.Services.ListService
{
    internal class ListServiceAutheticator : UserNamePasswordValidator
    {
        public override void Validate(String userName, String password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException(@"UserName and Password must not be null");
            }

            if (!(userName == "test1" && password == "test1"))
            {
                throw new SecurityTokenException("Authorisation Failed");
            }
        }
    }
}