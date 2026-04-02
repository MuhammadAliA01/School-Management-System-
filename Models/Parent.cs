using System;
using System.Collections.Generic;

namespace SchoolManagmentSystem.Models;

public partial class Parent
{
    public string? IdNumber { get; set; }

    public string? FirstName { get; set; }

    public string? Surname { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int Id { get; set; }
}
