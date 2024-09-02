namespace Core_Arca.Models
{
    public class RequestData
    {
        public string u_buildtype { get; set; }
        public string u_model { get; set; }
        public string u_serial { get; set; }
        public string u_contactfirstname { get; set; }
        public string u_contactlastname { get; set; }
        public string u_address1 { get; set; }
        public string u_address2 { get; set; }
        public string u_address3 { get; set; }
        public string u_city { get; set; }
        public string u_state { get; set; }
        public string u_zip { get; set; }
        public string u_country { get; set; }
        public string u_contactphone { get; set; }
        public string u_contactextension { get; set; }
        public string u_contacttitle { get; set; }
        public string u_transmissionpurpose { get; set; }
        public string u_customerticketid { get; set; }
        public string u_ticketcustnumber { get; set; }
        public string u_problemsummary { get; set; }
        public string u_problemdetails { get; set; }
        public string u_calltype { get; set; }
        public string u_sender { get; set; }
        public string u_requestcreationdate { get; set; }
        public string u_requestdispatchstamp { get; set; }
        public string u_contractid { get; set; }
        public string u_status { get; set; }
        public string u_manufacturer { get; set; }
        public string u_servicesitephone { get; set; }
        public string u_timestampatarrival { get; set; }
        public string u_timestampatcompletion { get; set; }
        public string u_employeeapproving { get; set; }
        public string u_rmareturntrackingnumber { get; set; }
        public string u_serviceassessment { get; set; }
        public string u_timestampcallassigned { get; set; }
        public string u_devicename { get; set; }
        public string u_devicelocation { get; set; }
        public string u_litigationhold { get; set; }
        public string u_tickercustomername { get; set; }
    }

    public class ResponseData
    {
        public string transform_map { get; set; }
        public string table { get; set; }
        public string display_name { get; set; }
        public string display_value { get; set; }
        public string record_link { get; set; }
        public string status { get; set; }
        public string sys_id { get; set; }
        public string status_message { get; set; }
        public string customerTicketId { get; set; }
        public string message { get; set; }
        public string ticketId { get; set; }
    }

    public class ApiResponse
    {
        public string import_set { get; set; }
        public string staging_table { get; set; }
        public List<ResponseData> result { get; set; }
    }
}
