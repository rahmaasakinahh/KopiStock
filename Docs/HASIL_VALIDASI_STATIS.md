# Validasi statis revisi v3

- XML parser: 12 file XAML valid sebagai XML.
- Kesesuaian nama event handler XAML dan code-behind: tidak ada yang hilang.
- Struktur SQL: skema database SQLite baru dapat dibuat.
- Migrasi database lama: kolom `created_at` dapat ditambahkan tanpa menghapus riwayat; baris lama bernilai NULL.
- Query ringkasan master: asal lot dan status terbaca.
- Perhitungan SQLite: 500 kg IN dan 150 kg OUT = 350 kg tersisa.
- Kode sumber belum dibangun dan dijalankan pada Windows dalam sesi ini. Validasi ini tidak menggantikan Build + pengujian fungsi/visual di Visual Studio.
