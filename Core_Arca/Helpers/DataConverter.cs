using Core_Arca.Models;

namespace Core_Arca.Helpers
{
    public static class DataConverter
    {
        internal static RequestData GetSNArcaRequest(ArcaRequest data, Device device)
        {
            RequestData requestData = new()
            {
                u_buildtype = "arca",
                u_model = data.model,
                u_serial = data.serial,
                u_contactfirstname = data.contactFirstName,
                u_contactlastname = data.contactLastName,
                u_address1 = data.address1,
                u_city = data.city,
                u_state = data.state,
                u_zip = data.zip,
                u_contactphone = data.contactPhone,
                u_contactextension = data.contactExtension,
                u_customerticketid = data.arcaTicketId,
                u_problemsummary = data.problemSummary,
                u_ticketcustnumber = device.customerNumber,
                u_problemdetails = data.note,
            };
            return requestData;
        }
        #region Arca

        internal static string[] Validate_ArcaRequest(ArcaRequest request, Device device)
        {
            string[] saIssuesDecision = { "", "" };  // Decision: Continue, Stop
            string sIssues = "";
            string sDecision = "No Issues";
            string sUsStateIsValid = "";

            // iSeverity -> sDecision
            // 0=No Issues, 1=Auto Ticket With Warning, 2=Auto Ticket With Review, 3=Manual Ticket, 4=Rejection
            int iSeverity = 0;
            // string sTemp = "";

            if (string.IsNullOrEmpty(request.arcaTicketId))
            {
                sIssues += "arcaTicketId required.  ";
                if (iSeverity < 4) iSeverity = 4;
                request.arcaTicketId = ""; // initialize if null
            }
            else if (request.arcaTicketId.Length > 50)
            {
                sIssues += "arcaTicketId exceeds 50 char max (" + request.arcaTicketId.Length + ")  [" + request.arcaTicketId + "]  ";
                if (iSeverity < 4) iSeverity = 4;
                request.arcaTicketId = request.arcaTicketId.Substring(0, 50);
            }

            if (string.IsNullOrEmpty(request.serial))
            {
                sIssues += "serial required  ";
                if (iSeverity < 4) iSeverity = 4;
                request.serial = ""; // initialize if null
            }
            else if (request.serial.Length > 25)
            {
                sIssues += "serial exceeds 25 char max (" + request.serial.Length + ")  [" + request.serial + "]   ";
                if (iSeverity < 3) iSeverity = 3;
                request.serial = request.serial.Substring(0, 25);
            }

            if (!string.IsNullOrEmpty(request.serial) && device.unit == "")
            {
                sIssues += "serial not found on currently active contract  [" + request.serial + "]   ";
                if (iSeverity < 3) iSeverity = 3;
            }

            if (string.IsNullOrEmpty(request.model))
            {
                request.model = ""; // initialize if null
            }
            else if (request.model.Length > 30)
            {
                sIssues += "model exceeds 30 char max (" + request.model.Length + ")  [" + request.model + "]  ";
                request.model = request.model.Substring(0, 30);
                if (iSeverity < 1) iSeverity = 1;
            }

            if (string.IsNullOrEmpty(request.problemSummary))
            {
                sIssues += "problemSummary required.  ";
                if (iSeverity < 4) iSeverity = 4;
                request.problemSummary = ""; // initialize if null
            }
            else if (request.problemSummary.Length > 50)
            {
                sIssues += "problemSummary exceeds 50 char max (" + request.problemSummary.Length + ")  [" + request.problemSummary + "] ";
                request.problemSummary = request.problemSummary.Substring(0, 50);
                if (iSeverity < 1) iSeverity = 1;
            }

            if (string.IsNullOrEmpty(request.contactFirstName))
            {
                sIssues += "contactFirstName required.  ";
                if (iSeverity < 4) iSeverity = 4;
                request.contactFirstName = ""; // initialize if null
            }
            else if (request.contactFirstName.Length > 30)
            {
                sIssues += "contactFirstName exceeds 30 char max (" + request.contactFirstName.Length + ")  [" + request.contactFirstName + "] ";
                request.contactFirstName = request.contactFirstName.Substring(0, 30);
                if (iSeverity < 1) iSeverity = 1;
            }

            if (string.IsNullOrEmpty(request.contactLastName))
            {
                request.contactLastName = ""; // initialize if null
            }
            else if (request.contactLastName.Length > 30)
            {
                sIssues += "contactLastName exceeds the 30 char max (" + request.contactLastName.Length + ") [" + request.contactLastName + "] ";
                request.contactLastName = request.contactLastName.Substring(0, 30);
                if (iSeverity < 1) iSeverity = 1;
            }

            string sNumbers = "0123456789";
            string sPhone = "";
            string sPhoneNumbers = "";

            if (string.IsNullOrEmpty(request.contactPhone))
            {
                sIssues += "contactPhone required  ";
                if (iSeverity < 4) iSeverity = 4;
                request.contactPhone = ""; // initialize if null
            }
            else if (request.contactPhone.Length < 10)
            {
                sIssues += "contactPhone contains less than 10 numbers  ";
                if (iSeverity < 4) iSeverity = 4;
            }
            else if (request.contactPhone.Length > 10)
            {
                sPhone = request.contactPhone.Trim();
                for (int i = 0; i < sPhone.Length; i++)
                {
                    if (sNumbers.Contains(sPhone.Substring(i, 1)))
                        sPhoneNumbers += sPhone.Substring(i, 1);
                }
                if (sPhoneNumbers.Length < 10)
                {
                    sIssues += "contactPhone contains less than 10 numbers  ";
                    if (iSeverity < 4) iSeverity = 4;
                }
                else if (sPhoneNumbers.Length > 10)
                {
                    sIssues += "contactPhone contains more than 10 numbers  [" + request.contactPhone + "] ";
                    request.contactPhone = sPhoneNumbers.Substring(0, 10);
                    if (iSeverity < 2) iSeverity = 2;
                }
            }

            string sExtensionNumbers = "";
            if (string.IsNullOrEmpty(request.contactExtension))
            {
                request.contactExtension = ""; // initialize if null
            }
            else
            {
                string sExtension = "";
                sExtension = request.contactExtension.Trim();
                for (int i = 0; i < sExtension.Length; i++)
                {
                    if (sNumbers.Contains(sExtension.Substring(i, 1)))
                        sExtensionNumbers += sExtension.Substring(i, 1);
                }
                if (sExtensionNumbers.Length > 7)
                {
                    sIssues += "contactExtension exceeds the 7 number max  [" + request.contactExtension + "] ";
                    request.contactExtension = sExtensionNumbers.Substring(0, 7);
                    if (iSeverity < 1) iSeverity = 1;
                }
            }


            string sMultipleEmails = "";
            string[] saEml = { "" };
            if (string.IsNullOrEmpty(request.contactEmail))
            {
                request.contactEmail = ""; // initialize if null
            }
            else
            {
                // "abc@company.com" = { "abc", "company.com" }; == 2 elements
                // "abc@company.com, def@company.com" = { "abc", "company.com, def", "company.com }; == 3 elements = multiple emails
                saEml = request.contactEmail.Split("@");
                if (saEml.Length > 2)
                    sMultipleEmails = "Y";
            }

            if (!string.IsNullOrEmpty(request.contactEmail) && request.contactEmail.Length > 50)
            {
                sIssues += "contactEmail exceeds 50 char max (" + request.contactEmail.Length + ")  [" + request.contactEmail + "] ";
                request.contactEmail = request.contactEmail.Substring(0, 50);
                if (iSeverity < 1) iSeverity = 1;
            }

            if (sMultipleEmails == "Y")
            {
                sIssues += "Only one contactEmail is permitted (field appears to have multiple) [" + request.contactEmail + "] ";
                if (iSeverity < 1) iSeverity = 1;
            }

            // Address Section
            if (string.IsNullOrEmpty(request.serviceSiteName))
            {
                sIssues += "serviceSiteName required.  ";
                if (iSeverity < 4) iSeverity = 4;
            }
            else if (request.serviceSiteName.Length > 40)
            {
                sIssues += "serviceSiteName exceeds 40 char max (" + request.serviceSiteName.Length + ")  [" + request.serviceSiteName + "] ";
                request.serviceSiteName = request.serviceSiteName.Substring(0, 40);
                if (iSeverity < 1) iSeverity = 1;
            }

            if (string.IsNullOrEmpty(request.address1))
            {
                sIssues += "address1 required  ";
                if (iSeverity < 4) iSeverity = 4;
                request.address1 = ""; // initialize if null
            }
            else if (request.address1.Length > 50)
            {
                sIssues += "address1 exceeds 50 char max (" + request.address1.Length + ")  [" + request.address1 + "] ";
                request.address1 = request.address1.Substring(0, 50);
                if (iSeverity < 1) iSeverity = 1;
            }

            if (string.IsNullOrEmpty(request.address2))
            {
                request.address2 = ""; // initialize if null
            }
            else if (request.address2.Length > 50)
            {
                sIssues += "address2 exceeds 50 char max (" + request.address2.Length + ")  [" + request.address2 + "] ";
                request.address2 = request.address2.Substring(0, 50);
                if (iSeverity < 1) iSeverity = 1;
            }

            if (string.IsNullOrEmpty(request.city))
            {
                sIssues += "city required.  ";
                if (iSeverity < 4) iSeverity = 4;
                request.city = ""; // initialize if null
            }
            else if (request.city.Length > 30)
            {
                sIssues += "city exceeds 30 char max (" + request.city.Length + ")  [" + request.city + "] ";
                request.city = request.city.Substring(0, 30);
                if (iSeverity < 1) iSeverity = 1;
            }

            // State
            string sState = request.state;
            var validStates = new HashSet<string>
            {
                "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA",
                "HI", "IA", "ID", "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME",
                "MI", "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH", "NJ", "NM",
                "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX",
                "UT", "VA", "VT", "WA", "WI", "WV", "WY"
            };

            if (string.IsNullOrEmpty(sState))
            {
                sIssues += "state required  ";
                if (iSeverity < 4) iSeverity = 4;
                request.state = ""; // initialize if null
            }
            else if (!validStates.Contains(sState))
            {
                sIssues += $"state is not a valid 2 character US state abbreviation. [{request.state}] ";
                if (iSeverity < 4) iSeverity = 4;

                if (sState.Length > 2)
                {
                    sIssues += $"state abbreviation exceeds 2 char max ({request.state.Length}) [{request.state}] ";
                    request.state = request.state.Substring(0, 2);
                    if (iSeverity < 4) iSeverity = 4;
                }
            }
            else
            {
                sUsStateIsValid = "Y";
            }


            // Mismatch section (city and state only)
            if (device.unit != "")
            {
                string sAddressMismatch = "";
                if (device.state != request.state)
                {
                    sIssues += "Address mismatch: state on request <> state on contract  [" + request.state + " vs " + device.state + "]  ";
                    sAddressMismatch = "Y";
                    if (iSeverity < 3) iSeverity = 3;
                }
                else if (device.city != request.city)
                {
                    sIssues += "Address mismatch: city on request <> city on contract  [" + request.city + " vs " + device.city + "] ";
                    sAddressMismatch = "Y";
                    if (iSeverity < 2) iSeverity = 2;
                }

                if (sAddressMismatch == "Y")
                {
                    sIssues += "(request address ";
                    if (request.address1 != "")
                        sIssues += request.address1 + " ";
                    if (request.address2 != "")
                        sIssues += request.address2 + " ";
                    if (request.city != "")
                        sIssues += request.city + " ";
                    if (request.state != "")
                        sIssues += request.state + " ";
                    if (request.zip != "")
                        sIssues += request.zip + " ";
                    sIssues += " -- contract address: ";
                    if (device.address1 != "")
                        sIssues += device.address1 + " ";
                    if (device.address2 != "")
                        sIssues += device.address2 + " ";
                    if (device.city != "")
                        sIssues += device.city + " ";
                    if (device.state != "")
                        sIssues += device.state + " ";
                    if (device.zip != "")
                        sIssues += device.zip + ")  ";
                }
            }

            if (string.IsNullOrEmpty(request.zip))
            {
                sIssues += "zip required  ";
                if (iSeverity < 4) iSeverity = 4;
                request.zip = ""; // initialize if null
            }
            else if (request.zip.Length > 30)
            {
                if (sUsStateIsValid == "Y")
                {
                    if (request.zip.Length > 5)
                    {
                        sIssues += "zip exceeds 5 char max for US zips (" + request.zip.Length + "). [" + request.zip + "] ";
                        request.zip = request.zip.Substring(0, 5);
                        if (iSeverity < 1) iSeverity = 1;
                    }
                }
            }

            if (request.note.Length > 5000)
            {
                sIssues += "note exceeds 5000 char max (" + request.note.Length + ")  ";
                request.note = request.note.Substring(0, 5000);
                if (iSeverity < 1) iSeverity = 1;
            }


            // 0=No Issues, 1=Auto Ticket With Warning, 2=Auto Ticket With Review, 3=Manual Ticket, 4=Rejection
            if (iSeverity == 0)
                sDecision = "No Issues";
            else if (iSeverity == 1)
                sDecision = "Auto Ticket With Warning";
            else if (iSeverity == 2)
                sDecision = "Auto Ticket With Review";
            else if (iSeverity == 3)
                sDecision = "Manual Ticket";
            else if (iSeverity == 4)
                sDecision = "Rejection";

            saIssuesDecision[0] = sIssues;
            saIssuesDecision[1] = sDecision;

            return saIssuesDecision;
        }

        #endregion
    }
}
