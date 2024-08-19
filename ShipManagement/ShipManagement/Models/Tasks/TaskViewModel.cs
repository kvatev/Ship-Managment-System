using Microsoft.AspNetCore.Identity;
using ShipManagement.Models.Users;

namespace ShipManagement.Models.Tasks;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TaskViewModel
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; }

    public string Description { get; set; }

    public string AssignedById { get; set; }

    [ForeignKey("AssignedById")]
    public IdentityUser AssignedBy { get; set; }

    public string AssignedToId { get; set; }

    [ForeignKey("AssignedToId")]
    public IdentityUser AssignedTo { get; set; }

    public DateTime AssignedDate { get; set; } 

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; } = false;
}
