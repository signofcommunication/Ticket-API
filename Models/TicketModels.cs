using System.ComponentModel.DataAnnotations;

namespace Ticket_API.Models;

public sealed record CreateTicketRequest(
    [Required(ErrorMessage = "Judul wajib diisi.")]
    [MaxLength(120, ErrorMessage = "Judul maksimal 120 karakter.")]
    string Title,

    [Required(ErrorMessage = "Nama wajib diisi.")]
    [MaxLength(100, ErrorMessage = "Nama maksimal 100 karakter.")]
    string Name,

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [MaxLength(160, ErrorMessage = "Email maksimal 160 karakter.")]
    string Email,

    [Required(ErrorMessage = "Deskripsi wajib diisi.")]
    [MinLength(10, ErrorMessage = "Deskripsi minimal 10 karakter.")]
    [MaxLength(2000, ErrorMessage = "Deskripsi maksimal 2000 karakter.")]
    string Description,

    // Bebas dulu, tidak dibatasi enum. Contoh: "login", "printer", "akses folder".
    [Required(ErrorMessage = "Tipe masalah wajib diisi.")]
    [MaxLength(60, ErrorMessage = "Tipe masalah maksimal 60 karakter.")]
    string IssueType);
