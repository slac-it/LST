//$Header:$
//
// Validation.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the utility class that has validation for certain controls
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LST.Business
{
    public class Validation
    {
        Business.Common_Util objCommon = new Business.Common_Util();
      
        //Validate the username
        public bool CheckUserValidity(string owner)
        {
            int _owner;

            if (owner != "")
            {
                _owner = objCommon.GetEmpid(owner.Trim());
                if (_owner == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else return false;
          
        }

       

        //Dropdown Validation

        public bool CheckIfSelected(string value)
        {
            if (value == "-1")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}