using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                Console.WriteLine("4. Search Contact & Edit Contact");
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

        private bool IsDuplicateContact(string name = "", string phone = "", string email = "", string ignoreName = "")
        {
            foreach (var contact in contacts)
            {
                if ((contact.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && !contact.Name.Equals(ignoreName, StringComparison.OrdinalIgnoreCase)) ||
                    contact.PhoneNumber.Equals(phone, StringComparison.OrdinalIgnoreCase) ||
                    contact.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private string GetValidatedInput(string prompt, string regexPattern, string invalidMessage, string type = "", string ignoreName = "")
        {
            Console.Write(prompt);
            var inputValue = Console.ReadLine();

            while (!Regex.IsMatch(inputValue, regexPattern) || IsDuplicateContact(name: type == "name" ? inputValue : "",
                                                                                  phone: type == "phone" ? inputValue : "",
                                                                                  email: type == "email" ? inputValue : "",
                                                                                  ignoreName: ignoreName))
            {
                string errorMessage = Regex.IsMatch(inputValue, regexPattern) ? $"{type} telah digunakan!" : invalidMessage;
                Console.WriteLine(errorMessage);
                Console.Write(prompt);
                inputValue = Console.ReadLine();
            }

            return inputValue;
        }

        private void CreateContact()
        {
            Console.Clear();
            Console.WriteLine("== Tambah Kontak ==\n");

            string name = GetValidatedInput("Masukkan Nama: ", @".\S", "Nama tidak boleh kosong. Silakan masukkan nama yang valid.", "name");
            string phone = GetValidatedInput("Masukkan Nomor Telepon: ", @"^\+?[0-9]{8,15}$", "Nomor telepon tidak valid!", "phone");
            string email = GetValidatedInput("Masukkan Email: ", @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$", "Email tidak valid!", "email");

            var contact = new Contact(name, phone, email);
            contacts.Add(contact);
            Console.WriteLine("\nKontak berhasil ditambahkan.");
            Console.WriteLine("Tekan Enter untuk kembali ke menu.");
            Console.ReadLine();
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

        private void ViewDeletedContacts()
        {
            Console.Clear();
            Console.WriteLine("== Daftar Kontak yang Dihapus ==\n");
            foreach (var contact in deletedContacts)
            {
                Console.WriteLine($"Nama: {contact.Name}, Telepon: {contact.PhoneNumber}, Email: {contact.EmailAddress}");
            }
            Console.ReadKey();
        }

        private void SearchContact()
        {
            Console.Clear();
            Console.WriteLine("== Cari Kontak ==\n");
            Console.Write("Masukkan Nama / Nomor HP / Email: ");
            var searchTerm = Console.ReadLine().ToLower(); // Diubah ke huruf kecil untuk pencarian yang case-insensitive

            var matchedContacts = contacts.Where(contact =>
                contact.Name.ToLower().Contains(searchTerm) ||
                contact.PhoneNumber.Contains(searchTerm) ||
                contact.EmailAddress.ToLower().Contains(searchTerm)
            ).ToList();

            if (matchedContacts.Count == 0)
            {
                Console.WriteLine("Tidak ada pengguna yang cocok dengan kata kunci yang diberikan!");
                Console.WriteLine("Tekan Enter untuk kembali ke menu.");
                Console.ReadLine();
                return;
            }
            else
            {
                Console.WriteLine("--------------------------");
                foreach (var contact in matchedContacts)
                {
                    Console.WriteLine($"\nNama: {contact.Name}\nPhoneNumber: {contact.PhoneNumber}\nEmailAddress: {contact.EmailAddress}");
                }
            }

            int choice;
            do
            {
                Console.WriteLine("\nMenu");
                if (matchedContacts.Count > 0)
                {
                    Console.WriteLine("1. Edit Kontak");
                    Console.WriteLine("2. Hapus Kontak");
                    Console.WriteLine("3. Back");
                    Console.Write("Masukkan Pilihan: ");
                }

                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            if (matchedContacts.Count > 0)
                            {
                                UpdateContact();
                            }
                            break;
                        case 2:
                            if (matchedContacts.Count > 0)
                            {
                                DeleteContact();
                            }
                            break;
                        case 3:
                            return;
                        default:
                            Console.WriteLine("Invalid choice, please try again.");
                            break;
                    }
                }
            }
            while (true);
        }


        private void UpdateContact()
        {
            Console.Write("Masukkan nama kontak yang ingin diperbarui: ");
            var nameToSearch = Console.ReadLine();
            var contact = contacts.Find(c => c.Name == nameToSearch);

            if (contact == null)
            {
                Console.WriteLine("Kontak tidak ditemukan!");
                return;
            }

            var nameToUpdate = GetValidatedInput("Masukkan nama Baru: ", @".\S", "Nama tidak boleh kosong. Silakan masukkan nama yang valid.", "name", nameToSearch);
            contact.Name = nameToUpdate;

            var newPhone = GetValidatedInput("Masukkan Nomor Telepon baru: ", @"^\+?[0-9]{8,15}$", "Nomor telepon tidak valid!", "phone", nameToUpdate);
            contact.PhoneNumber = newPhone;

            var newEmail = GetValidatedInput("Masukkan Email baru: ", @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$", "Email tidak valid!", "email", nameToUpdate);
            contact.EmailAddress = newEmail;
        }



        private void DeleteContact()
        {
            Console.Write("Masukkan nama kontak yang ingin dihapus: ");
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

    }
}