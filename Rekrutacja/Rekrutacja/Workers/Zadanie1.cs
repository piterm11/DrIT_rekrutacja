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
[assembly: Worker(typeof(Zadanie1), typeof(Pracownicy))]
namespace Rekrutacja.Workers
{
    public class Zadanie1
    {
        public class Zadanie1Parametry : ContextBase
        {
            [Caption("Data obliczeń")]
            public Date DataObliczen { get; set; }
            [Caption("A")]
            public Double A { get; set; }
            [Caption("B")]
            public Double B { get; set; }
            [Caption("Operacja")]
            public string Operacja { get; set; }
            public Zadanie1Parametry(Context context) : base(context)
            {
                this.DataObliczen = Date.Today;
            }
        }
        [Context]
        public Context Cx { get; set; }
        [Context]
        public Zadanie1Parametry Parametry { get; set; }

        [Action("Kalkulator - Zadanie 1",
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
            if(Parametry.Operacja == "+")
            {
                return Parametry.A+Parametry.B;
            } 
            else if (Parametry.Operacja == "-")
            {
                return Parametry.A + Parametry.B;
            }
            else if (Parametry.Operacja == "*")
            {
                return Parametry.A * Parametry.B;
            }
            else if (Parametry.Operacja == "/")
            {
                if (Parametry.B == 0)
                {
                    throw new DivideByZeroException("Nie można dzielić przez zero.");
                }
                return Parametry.A / Parametry.B;
            }
            else
            {
                throw new InvalidOperationException($"Nieznana operacja: {Parametry.Operacja}");
            }
        }

        

    }
}
