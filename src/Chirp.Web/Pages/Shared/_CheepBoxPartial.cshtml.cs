using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages.Shared;



public class CheepBoxModel
{

    [Required]
    [StringLength(160, ErrorMessage = "Message too long. Maximum {1}")]
    public string? CheepText { get; set; }

}