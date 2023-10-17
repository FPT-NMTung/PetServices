using System;
using System.Collections.Generic;

namespace PetServices.Models
{
    public partial class Otp
    {
        public int Otpid { get; set; }
        public string? Code { get; set; }
    }
}
