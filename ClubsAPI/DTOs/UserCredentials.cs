﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
  public class UserCredentials
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public string GetExportObject()
    {
      return $"{Email};{Password};";
    }
  }
}
