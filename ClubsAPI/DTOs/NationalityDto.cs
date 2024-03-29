﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
  public class NationalityDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string GetExportObject()
    {
      return $"{Id};{Name};";
    }
  }
}
