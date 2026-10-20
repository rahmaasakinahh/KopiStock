# KopiStock — Revisi Master Data, Riwayat, Profil, Pengaturan (v3)

Project Windows **C# WPF (.NET 8) + SQLite lokal** untuk ETS PBKK.

## Isi perubahan

- **Master Data > Jenis Kopi:** tambah kolom Status, Asal lot tercatat, dan aksi Lihat detail / Ubah. Status dapat diubah tanpa menghapus lot historis.
- **Master Data > Pemasok:** kontak ditampilkan, status dapat diubah, tombol Lihat detail / Ubah tersedia. Nomor kontak diisi melalui form pemasok (jika belum pernah diisi, tampil "Belum diisi").
- **Asal:** asal daerah yang ditampilkan di Master Data merupakan ringkasan asal dari lot yang telah diterima untuk jenis tersebut. Asal **tetap dicatat per lot**, karena satu jenis kopi dapat memiliki banyak asal.
- **Riwayat:** menampilkan **Tanggal transaksi** dan **Waktu dicatat**. Waktu dicatat adalah jam lokal ketika pengguna menekan Simpan; bukan jam kejadian di lapangan. Transaksi lama yang belum memiliki jam akan ditandai **— (data lama)** agar tidak ada waktu fiktif.
- **Profil:** nama tampilan adalah label yang terlihat di aplikasi dan dapat diubah; username adalah identitas login dan tetap.
- **Pengaturan:** dua kolom yang dikelompokkan dengan rapi, preferensi kerapatan tabel, info gudang, versi, dan lokasi database.
- **PDM:** mencantumkan kolom baru `StockMovements.created_at`.

## Memulai di Visual Studio Community

1. **Tutup aplikasi KopiStock yang sedang berjalan.**
2. **Cadangkan data** sebelum upgrade: pada File Explorer, tekan `Win + R`, lalu ketik `%LOCALAPPDATA%\KopiStock`. Salin `kopistock.db` ke folder backup di luar folder tersebut. **Jangan hapus / rename / timpa database aslinya.**
3. Ekstrak ZIP ini ke folder **baru** (misalnya `Documents\KopiStock_Revisi_v3`), jangan ditumpuk dengan proyek v2.
4. Buka `KopiStock.sln` dari folder yang baru.
5. Tunggu restore NuGet, tekan `Ctrl + Shift + B`, lihat Build Output, lalu tekan `F5` jika build sukses.
6. Login menggunakan akun demo yang telah disediakan pada aplikasi (untuk prototipe lokal).
7. Database tetap berada di `%LOCALAPPDATA%\KopiStock\kopistock.db` dan tidak disalin ke folder solution. Database yang sudah ada akan ditingkatkan otomatis dengan menambahkan kolom `created_at` tanpa membuang tabel/data lama.

## Catatan penting

- Jangan membuka versi lama dan baru bersamaan ketika mengubah data.
- Lima tabel tetap dipakai. Kolom waktu baru adalah migrasi skema, bukan tabel baru.
- Project sudah melewati validasi struktur XAML, referensi handler, dan simulasi migrasi/SQL. **Build WPF di Windows dan pengujian visual nyata tetap perlu kamu jalankan sendiri di Visual Studio.**
- Jangan menganggap catatan pengujian manual lulus sebelum benar-benar diuji.

## Tes singkat

- Master Data: buat/edit jenis kopi, ubah status Nonaktif lalu Aktif, cek kolom status dan Detail.
- Master Data: buat pemasok + kontak, cek kolom dan Detail, nonaktifkan lalu pastikan masih tampak di daftar Master.
- Penerimaan: buat lot baru, cek asal ditampilkan di ringkasan Master dan detail lot.
- Riwayat: catat transaksi baru, cek tanggal transaksi + **waktu dicatat**; transaksi lama menampilkan data lama, bukan jam buatan.
- Profil: ubah nama tampilan; pastikan username untuk login tidak ikut berubah.
- Pengaturan: Terapkan Nyaman / Ringkas, buka ulang halaman tabel untuk mengecek jarak antarbaris.
- Tutup-buka aplikasi lalu pastikan data tersimpan.

`Docs/PDM_KopiStock.png` dan `Docs/CHECKLIST_REVISI_v3.md` disertakan untuk dokumentasi.
