using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Perkjam.Client.ViewModels
{
    public class GetAddressFromIDPViewModel
    {
        public string Address { get; private set; } = string.Empty;

        public GetAddressFromIDPViewModel(string address)
        {
            Address = address;
        }
    }
}
