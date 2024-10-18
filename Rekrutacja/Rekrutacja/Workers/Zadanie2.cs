using Rekrutacja.Workers;
using Rekrutacja.Workers.Template;
using Soneta.Business;
using Soneta.Kadry;
using Soneta.KadryPlace;
using Soneta.Produkcja;
using Soneta.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: Worker(typeof(Zadanie2), typeof(Pracownicy))]
namespace Rekrutacja.Workers
{
    public class Zadanie2
    {
        public enum Figura
        {
            [Caption("Kwadrat")]
            Kwadrat,
            [Caption("Prostokąt")]
            Prostokat,
            [Caption("Trójkąt")]
            Trojkat,
            [Caption("Koło")]
            Kolo
        }
        public class Zadanie2Parametry : ContextBase
        {
            [Caption("Data obliczeń")]
            public Date DataObliczen { get; set; }
            [Caption("A")]
            public Double A { get; set; }
            [Caption("B")]
            public Double B { get; set; }
            [Caption("Figura")]
            public Figura Figura { get; set; }
            public Zadanie2Parametry(Context context) : base(context)
            {
                this.DataObliczen = Date.Today;
            }
        }
        [Context]
        public Context Cx { get; set; }
        [Context]
        public Zadanie2Parametry Parametry { get; set; }

        [Action("Kalkulator - Zadanie 2",
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
                            pracownikZSesja.Features["Wynik"] = (Double)Oblicz();
                            trans.CommitUI();
                        }
                        nowaSesja.Save();
                    }
                }
            }


        }

        private int Oblicz()
        {
            switch(Parametry.Figura)
            {
                case Figura.Kwadrat:
                    return (int)Math.Pow(Parametry.A, 2);
                case Figura.Prostokat:
                    return (int)(Parametry.A * Parametry.B);
                case Figura.Trojkat:
                    return (int)(Parametry.A * Parametry.B / 2);
                case Figura.Kolo:
                    return (int)(Math.PI* Math.Pow(Parametry.A, 2));
                default:
                    throw new InvalidOperationException($"Nieznana figura");
            }
            
        }

    }
}
