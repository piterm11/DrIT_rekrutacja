using Rekrutacja.Workers;
using Rekrutacja.Workers.Template;
using Soneta.Business;
using Soneta.Kadry;
using Soneta.KadryPlace;
using Soneta.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: Worker(typeof(Zadanie3), typeof(Pracownicy))]
namespace Rekrutacja.Workers
{
    public class Zadanie3
    {
        public class Zadanie3Parametry : ContextBase
        {
            [Caption("Data obliczeń")]
            public Date DataObliczen { get; set; }
            [Caption("A")]
            public string A { get; set; }
            [Caption("B")]
            public string B { get; set; }
            [Caption("Operacja")]
            public string Operacja { get; set; }
            public Zadanie3Parametry(Context context) : base(context)
            {
                this.DataObliczen = Date.Today;
            }
        }
        [Context]
        public Context Cx { get; set; }
        [Context]
        public Zadanie3Parametry Parametry { get; set; }

        [Action("Kalkulator - Zadanie 3",
            Description = "Prosty kalkulator ",
            Priority = 10,
            Mode = ActionMode.ReadOnlySession,
            Icon = ActionIcon.Accept,
            Target = ActionTarget.ToolbarWithText)]
        public void WykonajAkcje()
        {
            DebuggerSession.MarkLineAsBreakPoint();
            if (this.Cx.Contains(typeof(Pracownik[])))
            {
                Pracownik[] pracownicy = (Pracownik[])this.Cx[typeof(Pracownik[])];
                foreach (Pracownik pracownik in pracownicy)
                {
                    using (Session nowaSesja = this.Cx.Login.CreateSession(false, false, "ModyfikacjaPracownika"))
                    {
                        using (ITransaction trans = nowaSesja.Logout(true))
                        {
                            var pracownikZSesja = nowaSesja.Get(pracownik);
                            pracownikZSesja.Features["DataObliczen"] = Parametry.DataObliczen;
                            pracownikZSesja.Features["Wynik"] = Oblicz();
                            trans.CommitUI();
                        }
                        nowaSesja.Save();
                    }
                }
            }


        }

        private Double Oblicz()
        {
            var operacja = Parametry.Operacja;
            int a = MyParser(Parametry.A);
            int b = MyParser(Parametry.B);
            if(operacja == "+")
            {
                return a + b;
            } 
            else if (operacja == "-")
            {
                return a - b;
            }
            else if (operacja == "*")
            {
                return a * b;
            }
            else if (operacja == "/")
            {
                if (b == 0)
                {
                    throw new DivideByZeroException("Nie można dzielić przez zero.");
                }
                return a / b;
            }
            else
            {
                throw new InvalidOperationException($"Nieznana operacja: {operacja}");
            }
        }

        private int MyParser(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Parametr nie może być pusty.");
            int wynik = 0;
            bool ujemna = false;
            int index = 0;
            if (input[0] == '-')
            {
                ujemna = true;
                index = 1;
            }
            for (int i = index; i < input.Length; i++)
            {
                char c = input[i];
                if (c < '0' || c > '9')
                    throw new FormatException($"Nieprawidłowy znak '{c}' w parametrze wejściowym.");
                int cyfra = c-'0';
                wynik = wynik * 10 + cyfra;
            }
            return ujemna ? -wynik : wynik;
        }
    }
}
