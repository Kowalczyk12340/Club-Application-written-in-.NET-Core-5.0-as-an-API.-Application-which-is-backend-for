﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
  public class PlayerDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Biography { get; set; }
    public string Picture { get; set; }

    public string GetExportObject()
    {
      return $"{Id};{Name};{DateOfBirth};{Biography};{Picture};";
    }
  }
}
