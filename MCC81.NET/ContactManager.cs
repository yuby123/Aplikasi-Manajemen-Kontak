using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCC81.NET
{
    public class ContactManager
    {
        // Menyimpan daftar kontak
        private List<Contact> contacts = new List<Contact>();

        // Implementasi Stack<T> yang digunakan untuk menampung kontak-kontak yang telah dihapus dari daftar dan dapat dilacak.
        private Stack<Contact> deletedContacts = new Stack<Contact>();

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== Aplikasi Manajemen Kontak ==\n");
                Console.WriteLine("1. Create Contact");
                Console.WriteLine("2. View Contact");
                Console.WriteLine("3. View Deleted");
                Console.WriteLine("4. Search Contact");
                Console.WriteLine("5. Exit");
                Console.Write("Input :  ");

                int choice;

                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            CreateContact();
                            break;

                        case 2:
                            ViewContacts();
                            break;

                        case 3:
                            ViewDeletedContacts();
                            break;

                        case 4:
                            SearchContact();
                            break;

                        case 5:
                            return;

                        default:
                            Console.WriteLine("Pilihan tidak valid!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Masukkan pilihan yang valid.");
                    Console.ReadLine();
                }
            }
        }



        private void ViewContacts()
        {
            Console.Clear();
            Console.WriteLine("== Daftar Kontak ==\n");

            if (contacts.Count == 0)
            {
                Console.WriteLine("Tidak ada kontak yang tersimpan.");
            }

            foreach (var contact in contacts)
            {
                Console.WriteLine($"Nama: {contact.Name}");
                Console.WriteLine($"Telepon: {contact.PhoneNumber}");
                Console.WriteLine($"Email: {contact.EmailAddress}\n");
            }
            Console.WriteLine("Tekan Enter untuk kembali ke menu.");
            Console.ReadKey();
        }

        private void CreateContact()
        {
            Console.Clear();
            Console.WriteLine("== Tambah Kontak ==\n");
            Console.Write("Masukkan Nama: ");
            var name = Console.ReadLine();

            Console.Write("Masukkan Nomor Telepon: ");
            var phone = Console.ReadLine();
            while (!Regex.IsMatch(phone, @"^\+?[0-9]{8,15}$"))
            {
                Console.Write("Nomor telepon tidak valid!\n\nMasukkan Nomor Telepon: ");
                phone = Console.ReadLine();
            }

            Console.Write("Masukkan Email: ");
            var email = Console.ReadLine();
            while (!Regex.IsMatch(email, @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$"))
            {
                Console.Write("Email tidak valid!\n\nMasukkan Email: ");
                email = Console.ReadLine();
            }

            var contact = new Contact(name, phone, email);
            contacts.Add(contact);
            Console.WriteLine("\nKontak berhasil ditambahkan.");
            Console.WriteLine("Tekan Enter untuk kembali ke menu.");
            Console.ReadLine();
        }

        private void UpdateContact()
        {
            Console.WriteLine("Masukkan nama kontak yang ingin diperbarui:");
            var name = Console.ReadLine();

            var contact = contacts.Find(c => c.Name == name);
            if (contact == null)
            {
                Console.WriteLine("Kontak tidak ditemukan!");
                return;
            }

            Console.WriteLine("Masukkan Nomor Telepon baru:");
            var phone = Console.ReadLine();
            while (!Regex.IsMatch(phone, @"^\+?[0-9]{8,15}$"))
            {
                Console.WriteLine("Nomor telepon tidak valid! Masukkan lagi:");
                phone = Console.ReadLine();
            }
            contact.PhoneNumber = phone;

            Console.WriteLine("Masukkan Email baru:");
            var email = Console.ReadLine();
            while (!Regex.IsMatch(email, @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$"))
            {
                Console.WriteLine("Email tidak valid! Masukkan lagi:");
                email = Console.ReadLine();
            }
            contact.EmailAddress = email;
        }

        private void DeleteContact()
        {
            Console.WriteLine("Masukkan nama kontak yang ingin dihapus:");
            var name = Console.ReadLine();

            var contact = contacts.Find(c => c.Name == name);
            if (contact == null)
            {
                Console.WriteLine("Kontak tidak ditemukan!");
                return;
            }

            contacts.Remove(contact);
            deletedContacts.Push(contact);
            Console.WriteLine("Kontak telah dihapus.");
        }

        private void ViewDeletedContacts()
        {
            Console.WriteLine("Daftar Kontak yang Dihapus:");
            foreach (var contact in deletedContacts)
            {
                Console.WriteLine($"Nama: {contact.Name}, Telepon: {contact.PhoneNumber}, Email: {contact.EmailAddress}");
            }
            Console.ReadKey();
        }

        private void SearchContact()
        {
            ViewContacts();
            Console.Write("Masukkan Nama : ");
            var name = Console.ReadLine();


            // Mencari pengguna yang cocok berdasarkan nama yang mengandung kata kunci
            var searchUser = contacts.Where(u => Regex.IsMatch(u.Name, name, RegexOptions.IgnoreCase) ||
                                              Regex.IsMatch(u.PhoneNumber, name, RegexOptions.IgnoreCase) ||
                                              Regex.IsMatch(u.EmailAddress, name, RegexOptions.IgnoreCase)).ToList();

            if (searchUser.Count == 0)
            {
                Console.WriteLine("Tidak ada pengguna yang cocok dengan kata kunci yang diberikan !");
            }
            else
            {
                Console.WriteLine("--------------------------");
                foreach (var item in searchUser)
                {
                    Console.WriteLine($"\nNama : {item.Name}");
                    Console.WriteLine($"PhoneNumber : {item.PhoneNumber}");
                    Console.WriteLine($"EmailAddress : {item.EmailAddress}");
                }
            }

            int choice;
            do
            {
                Console.WriteLine("Menu");
                Console.WriteLine("1. Edit User");
                Console.WriteLine("2. Delete User");
                Console.WriteLine("3. Back");
                Console.Write("Masukkan Pilihan: ");
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            UpdateContact();
                            break;
                        case 2:
                            DeleteContact();
                            break;
                        case 3:
                            return;
                        default:
                            Console.WriteLine("Invalid choice, please try again.");
                            break;
                    }
                }
            } while (true);
            //Console.ReadLine();
        }
    }
}