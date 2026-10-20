# Checklist Pengujian Manual KopiStock

> Centang **lulus** hanya setelah diuji di Windows. Mulai dengan database baru atau kode lot yang belum pernah ada.

| ID | Skenario | Harapan | Hasil nyata |
|---|---|---|---|
| AT-01 | Password login salah | Ditolak, ada pesan, dashboard tidak terbuka | Belum diuji |
| AT-02 | Login admin / Admin123! | Dashboard terbuka | Belum diuji |
| AT-03 | Tambah jenis Arabika dan pemasok Koperasi Gayo | Tampil pada tabel dan pilihan penerimaan | Belum diuji |
| AT-04 | Terima lot L-001 500 kg | 1 lot + 1 IN, saldo 500 kg | Belum diuji |
| AT-05 | Terima lagi dengan kode lot L-001 | Ditolak, tidak menambah stok | Belum diuji |
| AT-06 | Keluar 150 kg | OUT tersimpan, saldo lot 350 kg | Belum diuji |
| AT-07 | Coba keluar 400 kg dari lot itu | Ditolak, saldo tetap 350 kg | Belum diuji |
| AT-08 | Jumlah kosong / 0 / negatif / >3 desimal | Ditolak, tidak menambah mutasi | Belum diuji |
| AT-09 | Buka Dashboard, Stok, Detail Lot, Riwayat | Nilai dan transaksi konsisten | Belum diuji |
| AT-10 | Tutup aplikasi lalu buka lagi | Data tetap ada di SQLite | Belum diuji |
| AT-11 | Ubah nama tampilan pada Profil | Header memperbarui nama | Belum diuji |
| AT-12 | FAQ dan formulir bantuan | FAQ expand, laporan kendala tersalin | Belum diuji |
| AT-13 | Uji source di komputer Windows lain | Aplikasi membuka dan database siap | Belum diuji |

**Perhatian:** jika kode lot L-001 sudah tersimpan, pakai kode lain atau backup lalu pindahkan database demo secara aman saat aplikasi sudah ditutup.
