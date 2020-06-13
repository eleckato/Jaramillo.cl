using jaramillo.cl.APICallers;
using jaramillo.cl.Common;
using jaramillo.cl.Models.APIModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace jaramillo.cl.Controllers
{
    [AllowAnonymous]
    public class BookServController : BaseController
    {
        readonly BookingCaller BC = new BookingCaller();
        readonly ServCaller SC = new ServCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();

        /* ---------------------------------------------------------------- */
        /* SERVICES LIST */
        /* ---------------------------------------------------------------- */

        [HttpGet]
        public ActionResult ServList(string name)
        {
            List<Servicio> serv;

            try
            {
                serv = SC.GetAllServ(name, "ACT").ToList();
                if (serv == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.name = name;

            return View(serv);
        }


        /* ---------------------------------------------------------------- */
        /* SERVICE DETAILS */
        /* ---------------------------------------------------------------- */

        [HttpGet]
        public ActionResult ServDetails(string servId)
        {
            if (string.IsNullOrEmpty(servId)) return Error_InvalidUrl();

            Servicio serv;
            BookingVM bookTemplate;
            List<BookingRestVM> bookRestList;

            try
            {
                serv = SC.GetServ(servId);
                if (serv == null) return Error_FailedRequest();

                var userId = User.Identity.GetUserId();

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = UC.GetUser(userId);
                    if (user == null) return Error_FailedRequest();

                    bookRestList = BC.GetAllBookRest(servId).ToList();
                    if (bookRestList == null) return Error_FailedRequest();

                    bookRestList = bookRestList.Where(x => x.start_date_hour > DateTime.Now).ToList();

                    bookTemplate = new BookingVM()
                    {
                        booking_id = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                        serv_id = serv.serv_id,
                        appuser_id = userId,
                        status_booking_id = "ACT",
                        updated_at = default, // Update on post
                        created_at = default, // Update on post
                        deleted = false,

                        start_date_hour = DateTime.Now.AddDays(1), // Update on form
                        end_date_hour = DateTime.Now.AddDays(1).AddMinutes(serv.estimated_time), // Update on form

                        serv = serv,
                        user = user
                    };

                    ViewBag.newBooking = bookTemplate;
                    ViewBag.bookRestList = bookRestList;
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            return View(serv);
        }

        [HttpPost]
        [Authorize(Roles="CLI")]
        public ActionResult BookService(BookingVM newBook)
        {
            if (newBook == null) return Error_FailedRequest();

            try
            {
                var isAvailable = CheckBookAvailability(newBook);
                if (isAvailable == null)
                {
                    Error_FailedRequest();
                    return RedirectToAction("ServDetails", new { servId = newBook.serv_id });
                }
                else if (isAvailable == false)
                {
                    SetErrorMsg("Ya hay una hora agendada para esa hora o esta fuera del horario de la tienda, por favor seleccione una diferente");
                    return RedirectToAction("ServDetails", new { servId = newBook.serv_id });
                }

                Booking apiBook = new Booking();
                apiBook = PropertyCopier.Copy(newBook, apiBook);

                apiBook.created_at = DateTime.Now;
                apiBook.updated_at = DateTime.Now;

                var bookId = BC.AddBooking(apiBook);
                if (bookId == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                SetErrorMsg(e.Message);
                return RedirectToAction("ServDetails", new { servId = newBook.serv_id });
            }

            SetSuccessMsg("Su hora fue agendada, puede revisar el estado en su perfil");
            return RedirectToAction("ServDetails", new { servId = newBook.serv_id });
        }


        /* ---------------------------------------------------------------- */
        /* HELPERS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// Revisa la disponibilidad para una reserva, comparando si hay conflicto con el horario de la tienda, otras reservas o restricciones de horario
        /// </summary>
        /// <param name="book"> Reserva a revisar </param>
        [NonAction]
        private bool? CheckBookAvailability(BookingVM book)
        {
            if (book == null || book.start_date_hour == null || book.end_date_hour == null) 
            {
                ErrorWriter.InvalidArgumentsError();
                Debug.WriteLine($"book.start_date_hour = {book?.start_date_hour ?? null}  |  book.end_date_hour = {book?.end_date_hour ?? null}");
                return null;
            }

            Debug.WriteLine($" ---------------- CHECKING BOOKING AVAILABILITY ---------------- ");

            try
            {
                DateTime start = book.start_date_hour ?? default;
                DateTime end = book.end_date_hour ?? default;
                if (start == default || end == default) return null;

                var servId = book.serv_id;

                var bookDate = start.Date;

                var startTime = start.TimeOfDay;
                var endTime = end.TimeOfDay;

                Debug.WriteLine("----------------------------------------------------------------");
                Debug.WriteLine($"SERV : {book.servName}");
                Debug.WriteLine($"CHEDULE : {book.schedule}");
                Debug.WriteLine("----------------------------------------------------------------");

                //!? GET STORE SCHEDULE
                Debug.WriteLine($" ---------------- CHECKING STORE SCHEDULE ---------------- ");
                var storeSche = BC.GetStoreSche();
                if (storeSche == null) return null;

                // Extraer la hora de apertura y cierre de la tienda
                var scheSt = storeSche.start_hour.TimeOfDay;
                var scheEn = storeSche.end_hour.TimeOfDay;
                // Extraer el horario de almuerzo de la tienda
                var scheLunchSt = storeSche.start_lunch_hour.TimeOfDay;
                var scheLunchEn = storeSche.end_lunch_hour.TimeOfDay;

                Debug.WriteLine($"SCHEDULE : {scheSt} - {scheLunchSt} / {scheLunchEn} - {scheEn}");

                // Check por la apertura y cierre de la tienda
                bool checkMorning = startTime >= scheSt && endTime > scheSt;
                bool checkEvening = startTime < scheEn && endTime <= scheEn;
                // Check la hora de almuerzo como una restricción
                bool checkLunch = CheckTimeConflict(startTime, endTime, scheLunchSt, scheLunchEn);

                // Si cualquiera fallo, conflicto
                if (!checkMorning || !checkEvening || !checkLunch)
                {
                    Debug.WriteLine("> CONFLICT FOUND;");
                    return false;
                }


                Debug.WriteLine($" ---------------- CHECKING RESTRICTIONS ---------------- ");
                //!? GET RESTRICTIONS
                var restList = BC.GetAllBookRest(servId).ToList();
                if (restList == null) return null;

                // Saca solo las reservas que son para el mismo día
                restList = restList.Where(x =>
                    x.start_date_hour?.Date == bookDate)
                    .ToList();

                foreach (var rest in restList)
                {
                    DateTime chStart = rest.start_date_hour ?? default;
                    DateTime chEnd = rest.end_date_hour ?? default;
                    if (chStart == default || chEnd == default) return null;

                    Debug.WriteLine($"TIME : {chStart.Hour} - {chEnd.Hour}");

                    var check = CheckTimeConflict(start, end, chStart, chEnd);

                    if (!check)
                    {
                        Debug.WriteLine("> CONFLICT FOUND;");
                        return false;
                    }
                }



                Debug.WriteLine($" ---------------- CHECKING OTHER BOOKINGS ---------------- ");
                //!? GET OTHERS BOOKING FROM THIS SERV
                var otherBookList = BC.GetAllBookings(status_booking_id: "ACT", serv_id: servId);
                if (otherBookList == null) return null;

                // Saca solo las reservas que son para el mismo día
                // Recordar sacar a book de la lista
                otherBookList = otherBookList.Where(x =>
                    x.start_date_hour?.Date == bookDate &&
                    !x.booking_id.Equals(book.booking_id))
                    .ToList();

                // Revisar si hay conflicto con otras reservas
                foreach (var bk in otherBookList)
                {
                    DateTime chStart = bk.start_date_hour ?? default;
                    DateTime chEnd = bk.end_date_hour ?? default;
                    if (chStart == default || chEnd == default) return null;

                    Debug.WriteLine($"TIME : {chStart.Hour} - {chEnd.Hour}");

                    var check = CheckTimeConflict(start, end, chStart, chEnd);

                    if (!check)
                    {
                        Debug.WriteLine("> CONFLICT FOUND;");
                        return false;
                    }
                }

                // Si no hay conflicto, return true
                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        [NonAction]
        private bool CheckTimeConflict(TimeSpan start, TimeSpan end, TimeSpan chStart, TimeSpan chFinish)
        {
            int st_st = TimeSpan.Compare(start, chStart);
            int st_en = TimeSpan.Compare(start, chFinish);
            int en_st = TimeSpan.Compare(end, chFinish);
            int en_en = TimeSpan.Compare(end, chStart);

            var lst = new List<int>() { st_st, st_en, en_st, en_en };

            var checkMinus = lst.All(x => x <= 0);
            var checkPlus = lst.All(x => x >= 0);

            if (checkMinus || checkPlus) return true;
            else return false;
        }
        [NonAction]
        private bool CheckTimeConflict(DateTime start, DateTime end, DateTime chStart, DateTime chFinish)
        {
            int st_st = DateTime.Compare(start, chStart);
            int st_en = DateTime.Compare(start, chFinish);
            int en_st = DateTime.Compare(end, chFinish);
            int en_en = DateTime.Compare(end, chStart);

            var lst = new List<int>() { st_st, st_en, en_st, en_en };

            var checkMinus = lst.All(x => x <= 0);
            var checkPlus = lst.All(x => x >= 0);

            if (checkMinus || checkPlus) return true;
            else return false;
        }

    }
}