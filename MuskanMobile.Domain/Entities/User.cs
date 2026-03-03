using MuskanMobile.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuskanMobile.Domain.Entities;

public partial class User : BaseEntity
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }
}
