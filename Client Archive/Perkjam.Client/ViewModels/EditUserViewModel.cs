﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Perkjam.Client.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
