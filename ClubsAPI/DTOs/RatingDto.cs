﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
    public class RatingDto
    {
        [Range(1,5)]
        public int Rating { get; set; }
        public int ClubId { get; set; }
    }
}
