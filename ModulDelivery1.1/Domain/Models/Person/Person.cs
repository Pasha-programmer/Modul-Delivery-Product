using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ModulDelivery.Domain.Models
{
    [Table("Organization")]
    public abstract class Person
    {
        public Person(string name, string surname, string patronymic, DateTime birth)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Birth = birth;
        }
        
        public abstract string Name { get;  set; }
        public abstract string Surname { get;  set; }
        public abstract string Patronymic { get;  set; }
        public abstract DateTime Birth { get;  set; }
        public int Age
        {
            get => (DateTime.Now.Year - Birth.Year);
        }
        protected void UserInitializationPerson(bool isFull, Person person)
        {
            if (isFull)
            {
                if (string.IsNullOrEmpty(person.Name))
                {
                    Console.Write("Ваше имя: ");
                    person.Name = Console.ReadLine().Trim().ToLower();
                }
                if (string.IsNullOrEmpty(person.Patronymic))
                {
                    Console.Write("Ваше отчество: ");
                    person.Patronymic = Console.ReadLine().Trim().ToLower();
                }
            }

            if (string.IsNullOrEmpty(person.Surname))
            {
                Console.Write("Ваше фамилия: ");
                person.Surname = Console.ReadLine().Trim().ToLower();
            }
            if (person.Birth != default(DateTime))
            {
                Console.Write($"{person.Birth.ToString("D")} ваша дата рождения? (Yes/No) : ");
                if (Console.ReadLine().Trim().ToUpper() == "NO")
                {
                    Console.Write("Ваша дата рождения(xx.xx.xxxx): ");
                    person.Birth = (DateTime.Parse(Console.ReadLine()));
                }
            }
            else
            {
                Console.Write("Ваша дата рождения(xx.xx.xxxx): ");
                person.Birth = DateTime.Parse(Console.ReadLine());
            }
        }
    }
}
