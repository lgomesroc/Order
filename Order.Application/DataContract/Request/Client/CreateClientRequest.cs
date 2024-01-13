﻿namespace Order.Application.DataContract.Request.Client
{
    public sealed class CreateClientRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public  string Address { get; set; }
    }
}
