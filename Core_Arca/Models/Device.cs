namespace Core_Arca.Models
{
    public class Device
    {
        public Device()
        {
            unit = "";
            customerNumber = "";
            serial = "";
            model = "";
            agreement = "";
            address1 = "";
            address2 = "";
            city = "";
            state = "";
            zip = "";
        }

        public string unit { get; set; }
        public string customerNumber { get; set; }
        public string serial { get; set; }
        public string model { get; set; }
        public string agreement { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; } 
    }
}
