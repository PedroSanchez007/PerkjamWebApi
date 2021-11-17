using Perkjam.Client.Controllers;
using Perkjam.Model;
using System.Collections.Generic;

namespace Perkjam.Client.ViewModels
{
    public class PerkIndexViewModel
    {
        public IEnumerable<User> Users { get; private set; } = new List<User>();

        public PerkIndexViewModel(IEnumerable<User> users)
        {
           Users = users;
        }
    }
}
