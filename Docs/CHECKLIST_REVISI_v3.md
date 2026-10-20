# Checklist revisi KopiStock v3

| No | Kasus uji | Hasil yang diharapkan | Status |
|---|---|---|---|
| 1 | Buka database existing dari v2 | Data lot, master, dan mutasi sebelumnya tetap ada | Belum diuji di Windows |
| 2 | Master kopi: tambah tipe baru + status Aktif | Tipe terlihat di daftar dan pilihan Penerimaan | Belum diuji di Windows |
| 3 | Ubah status kopi Nonaktif | Masih terlihat di Master; tidak tersedia untuk penerimaan baru | Belum diuji di Windows |
| 4 | Klik Lihat detail jenis kopi | Menampilkan nama, status, ringkasan asal, dan jumlah lot | Belum diuji di Windows |
| 5 | Masukkan pemasok beserta nomor kontak | Kontak dan status terlihat di tabel dan detail | Belum diuji di Windows |
| 6 | Ubah pemasok terpilih | Nilai baru tampil tanpa merusak lot lama | Belum diuji di Windows |
| 7 | Buat lot (500 kg) dari satu daerah | Master jenis kopi memperlihatkan asal berdasarkan lot | Belum diuji di Windows |
| 8 | Keluarkan 150 kg | Saldo tersisa 350 kg | Belum diuji di Windows |
| 9 | Buat transaksi baru dan buka Riwayat | Tanggal transaksi dan waktu pencatatan yang sesungguhnya terlihat | Belum diuji di Windows |
| 10 | Buka transaksi lama sebelum v3 | Waktu tercatat "— (data lama)", tidak menebak jam | Belum diuji di Windows |
| 11 | Ganti nama tampilan | Nama di profil/sidebar berubah, username login tetap | Belum diuji di Windows |
| 12 | Simpan preferensi tampilan | Tabel baru mengikuti pilihan Nyaman / Ringkas | Belum diuji di Windows |
| 13 | Tutup dan buka ulang aplikasi | Data tetap tersimpan | Belum diuji di Windows |

**Catatan:** Data contoh tidak boleh disimpan langsung sebagai angka statis dashboard. Semua angka inventaris berasal dari SQLite.
