using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

//dodatek w commit'cie 2

/*

System wspierający generowanie tras w firmie logistycznej
A route-generation assistant for logistics company

Opiekun pracy
mgr inż. Tomasz Gawron


Cel pracy
Celem projektu jest stworzenie oprogramowania wspierającego pracowników firmy
z branży logistycznej (np. kurierzy) w tworzeniu dziennych tras dla kierowców.
System powinien umożliwiać definiowanie i generowanie tras dla pracowników
centrali, a także ich przeglądanie i drukowanie dla kierowców. Aplikacja powinna
umożliwiać optymalizację trasy wg odległości (najkrótsza trasa) oraz kosztów
(omijanie dróg płatnych).

Zadania do wykonania
1. Zebranie wymagań
2. Wybór technologii i ogólny projekt systemu
3. Iteracyjna implementacja, testowanie i prezentacja systemu
4. Opracowanie dokumentacji projektu i instrukcji użytkownika


Źródła
1. Dokumentacja API systemów map online
2. K. Schwaber, J. Sutherland, The Scrum Guide, Przewodnik po Scrumie: Reguły
Gry, Scrum.org, 2013
3. Dokumentacja wybranych technologii
4. Wymagania zebrane od opiekuna


Uwagi
Każde przedsiębiorstwo nastawione jest na minimalizowanie kosztów. Aplikacja,
która jest celem projketu, powinna dawać taką możliwość poprzez wyznaczanie
najkrótszych tras łączących wszystkie punkty docelowe. Wygenerowana trasa
powinna zostać zwizualizowana w jednym z dostępnych systemów map online.
Zaleca się przyrostowe wytwarzanie systemu metodyką Scrum.

*/


namespace RouteGeneration_assistant_for_logistics_company.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index ()
		{
			var mvcName = typeof(Controller).Assembly.GetName ();
			double x = Math.Sin (10);

			var isMono = Type.GetType ("Mono.Runtime") != null;

			ViewData ["Version"] = mvcName.Version.Major + "." + mvcName.Version.Minor;
			ViewData ["Runtime"] = isMono ? "Mono" : ".NET";

			return View ();
		}
	}
}

