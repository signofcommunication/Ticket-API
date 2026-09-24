using Ticket_API.Models;

namespace Ticket_API.Data;

public static class InMemoryStore
{
    // Demo agent: email = admin@helpdesk.test, password = password
    public static readonly IReadOnlyList<(ApiUser User, string Password)> Users =
    [
        (new ApiUser("user-1", "Nico Abel", "admin@helpdesk.test", "Agent"), "password"),
    ];

    public static readonly IReadOnlyList<Ticket> Tickets =
    [
        new Ticket(
            "ticket-1", "HD-2409-001", "raka.pratama@aksadigitex.test",
            "Tidak bisa login ke portal karyawan",
            "Setelah reset password, halaman login selalu kembali ke form awal tanpa pesan error.",
            "open", "urgent",
            DateTimeOffset.Parse("2026-09-22T08:20:00+07:00"),
            DateTimeOffset.Parse("2026-09-22T09:10:00+07:00"),
            "Nico Abel",
            [
                new TicketReply("reply-1", "Raka Pratama",
                    "Saya sudah coba di Chrome dan Edge, hasilnya sama.",
                    DateTimeOffset.Parse("2026-09-22T08:25:00+07:00"), false),
            ]),
        new Ticket(
            "ticket-2", "HD-2409-002", "melati.sari@aksadigitex.test",
            "Permintaan akses folder finance",
            "Mohon dibantu akses folder finance Q4 karena saya perlu menyiapkan rekonsiliasi mingguan.",
            "pending", "medium",
            DateTimeOffset.Parse("2026-09-22T10:45:00+07:00"),
            DateTimeOffset.Parse("2026-09-22T11:18:00+07:00"),
            "Support Team",
            [
                new TicketReply("reply-2", "Support Team",
                    "Kami sedang menunggu approval dari owner folder finance.",
                    DateTimeOffset.Parse("2026-09-22T11:18:00+07:00"), true),
            ]),
        new Ticket(
            "ticket-3", "HD-2409-003", "dimas.wijaya@aksadigitex.test",
            "Email approval tidak masuk",
            "Approval cuti dari atasan belum masuk ke email, padahal status di aplikasi sudah approved.",
            "replied", "high",
            DateTimeOffset.Parse("2026-09-21T15:05:00+07:00"),
            DateTimeOffset.Parse("2026-09-22T08:30:00+07:00"),
            "Nico Abel",
            [
                new TicketReply("reply-3", "Support Team",
                    "Kami sudah trigger ulang email approval. Mohon cek inbox dan spam.",
                    DateTimeOffset.Parse("2026-09-22T08:30:00+07:00"), true),
            ]),
        new Ticket(
            "ticket-4", "HD-2409-004", "nadya.lestari@aksadigitex.test",
            "Printer lantai 3 offline",
            "Printer finance lantai 3 tidak muncul di daftar printer sejak pagi.",
            "resolved", "low",
            DateTimeOffset.Parse("2026-09-20T09:42:00+07:00"),
            DateTimeOffset.Parse("2026-09-20T13:05:00+07:00"),
            "Field Support",
            [
                new TicketReply("reply-4", "Field Support",
                    "Printer sudah online kembali setelah konfigurasi jaringan diperbarui.",
                    DateTimeOffset.Parse("2026-09-20T13:05:00+07:00"), true),
            ]),
        new Ticket(
            "ticket-5", "HD-2409-005", "andini.putri@aksadigitex.test",
            "Data absensi tidak sinkron",
            "Absensi tanggal 21 September belum muncul di dashboard HR, namun sudah berhasil clock-in.",
            "open", "high",
            DateTimeOffset.Parse("2026-09-22T12:15:00+07:00"),
            DateTimeOffset.Parse("2026-09-22T12:15:00+07:00"),
            "HRIS Support",
            []),
    ];

    public static TicketSummary BuildSummary() => new(
        Total: Tickets.Count,
        Open: Tickets.Count(t => t.Status == "open"),
        Pending: Tickets.Count(t => t.Status == "pending"),
        Replied: Tickets.Count(t => t.Status == "replied"),
        Resolved: Tickets.Count(t => t.Status == "resolved"),
        Urgent: Tickets.Count(t => t.Priority == "urgent"));
}
