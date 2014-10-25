﻿/*
This source file is subject to version 3 of the GPL license, 
that is bundled with this package in the file LICENSE, and is 
available online at http://www.gnu.org/licenses/gpl.txt; 
you may not use this file except in compliance with the License. 

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

All portions of the code written by Whoaverse are Copyright (c) 2014 Whoaverse
All Rights Reserved.
*/

using System;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using Whoaverse.Models;
using Whoaverse.Utils;

namespace Whoaverse.Controllers
{
    public class PartnerController : Controller
    {
        // GET: PartnerIntentRegistration
        public ActionResult PartnerProgramInformation()
        {
            return View();
        }

        // GET: PartnerIntentRegistration
        [RequireHttps]
        [Authorize]
        public ActionResult PartnerIntentRegistration()
        {
            PartnerIntent model = new PartnerIntent();
            model.UserName = User.Identity.Name;
            return View(model);
        }

        [Authorize]
        [RequireHttps]
        [HttpPost]
        [PreventSpam(DelayRequest = 300, ErrorMessage = "Sorry, you are doing that too fast. Please try again later.")]
        public ActionResult PartnerIntentRegistration(PartnerIntent partnerModel)
        {
            if (ModelState.IsValid)
            {
                MailAddress from = new MailAddress(partnerModel.Email);
                MailAddress to = new MailAddress("legal@whoaverse.com");
                StringBuilder sb = new StringBuilder();
                MailMessage msg = new MailMessage(from, to);

                msg.Subject = "New Partner Intent registration from " + partnerModel.FullName;
                msg.IsBodyHtml = false;

                // format Partner Intent Email
                sb.Append("Full name: " + partnerModel.FullName);
                sb.Append(Environment.NewLine);
                sb.Append("Email: " + partnerModel.Email);
                sb.Append(Environment.NewLine);
                sb.Append("Mailing address: " + partnerModel.MailingAddress);
                sb.Append(Environment.NewLine);
                sb.Append("City: " + partnerModel.City);
                sb.Append(Environment.NewLine);
                sb.Append("Country: " + partnerModel.Country);
                sb.Append(Environment.NewLine);
                sb.Append("Phone number: " + partnerModel.PhoneNumber);
                sb.Append(Environment.NewLine);
                sb.Append("Username: " + partnerModel.UserName);
                sb.Append(Environment.NewLine);

                msg.Body = sb.ToString();

                // send the email with Partner Intent data
                if (EmailUtility.sendEmail(msg))
                {
                    msg.Dispose();
                    ViewBag.SelectedSubverse = string.Empty;
                    return View("~/Views/Partner/PartnerProgramIntentSent.cshtml");
                }
                else
                {
                    ViewBag.SelectedSubverse = string.Empty;
                    return View("~/Views/Errors/Error.cshtml");
                }
            }
            else
            {
                return View();
            }
        }
    }
}