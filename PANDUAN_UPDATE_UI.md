# KopiStock – Update UI mengikuti Figma (versi revisi)

## Penting sebelum mulai

**Jangan menimpa folder lama yang sedang dibuka Visual Studio.** Versi ini adalah paket project pengganti yang sudah berisi desain WPF baru dan logika database dari versi sebelumnya. Kode baru **belum diuji melalui compiler Visual Studio Windows** di lingkungan pembuat paket; uji Build diperlukan pada komputermu.

1. Hentikan aplikasi KopiStock yang sedang berjalan dan tutup Visual Studio.
2. Pastikan data lama aman: tekan `Win + R`, masukkan `%LOCALAPPDATA%\KopiStock`, kemudian salin `kopistock.db` ke lokasi cadangan, **saat aplikasi sudah ditutup**. Jangan hapus file database aslinya.
3. Ekstrak ZIP ini ke folder **baru**, misalnya `Documents\KopiStock_UI_Figma_v2`.
4. Buka file `KopiStock.sln` di folder baru melalui Visual Studio Community 2026.
5. Tunggu Restore NuGet, lalu tekan `Ctrl + Shift + B` (Build Solution).
6. Jika muncul error, kirim gambar **Error List** serta error pertamanya. Jika 0 errors, tekan `F5`.
7. Login dengan `admin` / `Admin123!` (hanya demo). Periksa bahwa sidebar teks terang dan tombol coklat tulisan putih.

**Database lama tidak di-reset.** Kedua versi memakai lokasi `%LOCALAPPDATA%\KopiStock\kopistock.db`, lima tabel yang sama, dan `schema.sql` yang tidak diubah.

## Perubahan tampilan

- **Login:** dua panel, foto biji kopi/gudang (crop dari screenshot referensi yang diberikan pengguna), palet espresso–cream, form dan tipografi baru.
- **Sidebar:** warna teks terang yang bisa terbaca, navigasi aktif, pemisah RUANG KERJA dan AKUN & BANTUAN.
- **Dashboard:** 4 kartu metrik real-time dari SQLite (saldo, lot aktif, penerimaan, pengeluaran), aktivitas terakhir, lot terbaru, tindakan cepat.
- **Master Data:** tab Jenis Kopi / Pemasok, tabel dan form pada panel berdampingan.
- **Penerimaan:** form 2 kolom, panduan samping dan ringkasan penerimaan terakhir.
- **Stok & Pengeluaran:** daftar lot dan form pengeluaran berdampingan, kartu rincian stok lot yang dipilih.
- **Riwayat:** filter kode dan IN/OUT, ringkasan IN / OUT / saldo dari keseluruhan transaksi.
- **Detail Lot:** informasi lot di kiri, metrik dan riwayat di kanan.
- **Profil:** identitas akun, edit nama tampilan (fitur dari kode sebelumnya dipertahankan).
- **Pengaturan:** pilihan tabel Nyaman/Ringkas tersimpan di file lokal, informasi gudang, dan tombol buka/salin lokasi database.
- **Pusat Bantuan:** 6 FAQ yang dapat dibuka/tutup dan dicari, formulir laporan kendala yang menyalin pesan (tanpa server).

## Kesetiaan terhadap Figma

Desain dibuat **mendekati layout, warna, dan hirarki** dari 10 screenshot. Beberapa penyesuaian sengaja dilakukan karena ini aplikasi nyata, bukan mockup: angka tidak dipatok `350 kg`, tidak ada status error yang selalu muncul, status transaksi mengambil database, dan tabel Master Data memakai field yang memang ada di skema P0. Penggantian foto login yang beresolusi lebih tinggi bisa dilakukan kemudian jika aset asli dari Figma tersedia.

Tidak memakai framework UI tambahan, EF Core, MVVM Toolkit, layanan cloud, atau database baru. Yang tetap dipakai hanya WPF/XAML, C#, dan Microsoft.Data.Sqlite.

## Pengujian minimum setelah build

- Login `admin` / `Admin123!`.
- Klik seluruh menu sidebar; teks dan state aktif terbaca.
- Tambah jenis kopi dan pemasok.
- Terima lot L-001 sebesar 500 kg; jika database lama sudah mempunyai L-001, gunakan kode unik lain.
- Keluar 150 kg, cek saldo 350 kg; lalu coba lebih dari saldo untuk memastikan ditolak.
- Bandingkan metrik Dashboard, Stok, Riwayat, Detail Lot.
- Uji pencarian Help Center, salin laporan kendala, profil, dan kerapatan tabel.
- Tutup dan buka kembali; data tetap tersedia.

**Hasil validasi yang sudah dikerjakan di lingkungan ini:** seluruh 12 XAML terbaca sebagai XML, event handler terhubung secara statis, dan tidak ditemukan duplikasi ID x:Name per file. **Belum ada build/run Windows**, jadi hasil akhir UI baru tetap perlu dicek di Visual Studio.
