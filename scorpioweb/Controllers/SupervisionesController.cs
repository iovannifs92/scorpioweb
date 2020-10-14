﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;

namespace scorpioweb.Controllers
{
    public class SupervisionesController : Controller
    {
        #region -Variables globales-
        private readonly penas2Context _context;
        private List<SelectListItem> listaNaSiNo = new List<SelectListItem>

        {
            new SelectListItem{ Text="Na", Value="NA"},
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };
        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
            }
            return normalizar;
        }
        String BuscaId(List<SelectListItem> lista, String texto)
        {
            foreach (var item in lista)
            {
                if (normaliza(item.Value) == normaliza(texto))
                {
                    return item.Value;
                }
            }
            return "";
        }
        #endregion


        public SupervisionesController(penas2Context context)
        {
            _context = context;
        }

        #region -Index-
        public async Task<IActionResult> Index()
        {
            return View(await _context.Supervision.ToListAsync());
        }
        #endregion

        #region -Details-
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        #endregion

        #region -Create-
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSupervision,Inicio,Termino,EstadoSupervision,PersonaIdPersona,EstadoCumplimiento,CausaPenalIdCausaPenal")] Supervision supervision)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supervision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supervision);
        }

        #endregion

        #region -Edit-
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            #region Estado Suprvición
            List<SelectListItem> ListaEstadoS;
            ListaEstadoS = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Concluido", Value = "CONCLUIDO" },
                new SelectListItem{ Text = "Vigentes", Value = "VIGENTE" },
                new SelectListItem{ Text = "Pendiente", Value = "PENDIENTE" },
                };

            ViewBag.listaEstadoSupervision = ListaEstadoS;
            ViewBag.idEstadoSupervision = BuscaId(ListaEstadoS, supervision.EstadoSupervision);
            #endregion



            #region Estado Cumplimiento
            List<SelectListItem> ListaEstadoC;
            ListaEstadoC = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Cumpliendo", Value = "Cumpliendo" },
                new SelectListItem{ Text = "Cumplimiento Parcial", Value = "VIGENTE" },
                new SelectListItem{ Text = "Incumplimiento Total", Value = "PENDIENTE" },
                };

            ViewBag.listaEstadoCumplimiento = ListaEstadoC;
            ViewBag.idEstadoCumplimiento = BuscaId(ListaEstadoC, supervision.EstadoCumplimiento);
            #endregion




            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSupervision,Inicio,Termino,EstadoSupervision,PersonaIdPersona,EstadoCumplimiento,CausaPenalIdCausaPenal")] Supervision supervision)
        {
            if (id != supervision.IdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supervision);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(supervision.IdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(supervision);
        }

        #endregion

        #region -Delete-
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            _context.Supervision.Remove(supervision);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region -SupervisionExists-
        private bool SupervisionExists(int id)
        {
            return _context.Supervision.Any(e => e.IdSupervision == id);
        }

        #endregion

        #region -MenuSupervision-
        public IActionResult MenuSupervision()
        {
            return View();
        }
        #endregion

        #region -PersonaSupervicion-
        public ActionResult PersonaSupervision()
        {
            List<Supervision> SupervisionVM = _context.Supervision.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();
            List<Persona> personaVM = _context.Persona.ToList();
            #region -Jointables-
            ViewData["joinTablesSupervision"] = from supervisiontable in SupervisionVM
                                                join personatable in personaVM on supervisiontable.PersonaIdPersona equals personatable.IdPersona
                                                join causapenaltable in causaPenalVM on supervisiontable.CausaPenalIdCausaPenal equals causapenaltable.IdCausaPenal

                                                select new SupervisionPyCP
                                                {
                                                    causapenalVM = causapenaltable,
                                                    supervisionVM = supervisiontable,
                                                    personaVM = personatable
                                                };
           
            #endregion
            return View();
        }
        #endregion

        #region -Supervision-
        public async Task<IActionResult> Supervision(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            List<Supervision> SupervisionVM = _context.Supervision.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();
            List<Persona> personaVM = _context.Persona.ToList();
            #region -Jointables-
            ViewData["joinTablesSupervision"] = from supervisiontable in SupervisionVM
                                          join  personatable in personaVM on supervisiontable.PersonaIdPersona equals personatable.IdPersona
                                          join causapenaltable in causaPenalVM on supervisiontable.CausaPenalIdCausaPenal equals causapenaltable.IdCausaPenal
                                          where supervisiontable.IdSupervision == id
                                 
                                          select new SupervisionPyCP
                                          {
                                              causapenalVM = causapenaltable,
                                              supervisionVM = supervisiontable,
                                              personaVM = personatable 
                                          };
            #endregion


            return View();
        }
        #endregion

      




        #region -Aer-
        public async Task<IActionResult> EditAer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Aer.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaCuentaEvaluacion = listaNaSiNo;
            ViewBag.idCuentaEvaluacion = BuscaId(listaNaSiNo, supervision.CuentaEvaluacion);



            #region Riesgo
            List<SelectListItem> ListaEstadoC;
            ListaEstadoC = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Baja", Value = "BAJA" },
                new SelectListItem{ Text = "Media", Value = "MEDIA" },
                new SelectListItem{ Text = "Alta", Value = "ALTA" }
                };

            ViewBag.listaRiesgoDetectado = ListaEstadoC;
            ViewBag.idRiesgoDetectado = BuscaId(ListaEstadoC, supervision.RiesgoDetectado);

            ViewBag.listaRiesgoSustraccion = ListaEstadoC;
            ViewBag.idRiesgoSustraccion = BuscaId(ListaEstadoC, supervision.RiesgoSustraccion);

            ViewBag.listaRiesgoObstaculizacion = ListaEstadoC;
            ViewBag.idRiesgoObstaculizacion = BuscaId(ListaEstadoC, supervision.RiesgoObstaculizacion);

            ViewBag.listaRiesgoVictima = ListaEstadoC;
            ViewBag.idRiesgoVictima = BuscaId(ListaEstadoC, supervision.RiesgoVictima);
            #endregion



            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAer(int id, [Bind("IdAer,CuentaEvaluacion,FechaEntrega,EvaluadorCaso,RiesgoDetectado,RiesgoSustraccion,RiesgoObstaculizacion,RiesgoVictima,SupervisionIdSupervision")] Aer aer)
        {
            if (id != aer.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(aer.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(aer);
        }
        #endregion

        #region -EditCambiodeobligaciones-
        public async Task<IActionResult>  EditCambiodeobligaciones(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Cambiodeobligaciones.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaSediocambio = listaNaSiNo;
            ViewBag.idSediocambio = BuscaId(listaNaSiNo, supervision.SeDioCambio);

            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCambiodeobligaciones(int id, [Bind("IdCambiodeObligaciones,SeDioCambio,FechaAprobacion,MotivoAprobacion,SupervisionIdSupervision")] Cambiodeobligaciones cambiodeobligaciones)
        {
            if (id != cambiodeobligaciones.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cambiodeobligaciones);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(cambiodeobligaciones.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(cambiodeobligaciones);
        }
        #endregion

        #region -EditCierredecaso-
        public async Task<IActionResult> EditCierredecaso(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Cierredecaso.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            ViewBag.listaSeCerroCaso = listaNaSiNo;
            ViewBag.idSeCerroCaso = BuscaId(listaNaSiNo, supervision.SeCerroCaso);
            #region Autorizo
            List<SelectListItem> ListaAutorizo;
            ListaAutorizo = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Director", Value = "DIRECTOR" },
                new SelectListItem{ Text = "Coordinador", Value = "COORDINADOR" }
                };

            ViewBag.listaAutorizo = ListaAutorizo;
            ViewBag.idAutorizo = BuscaId(ListaAutorizo, supervision.Autorizo);
            #endregion



            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCierredecaso(int id, [Bind("IdCierreDeCaso,SeCerroCaso,ComoConcluyo,NoArchivo,FechaAprobacion,Autorizo,SupervisionIdSupervision")] Cierredecaso cierredecaso)
        {
            if (id != cierredecaso.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cierredecaso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(cierredecaso.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(cierredecaso);
        }
        #endregion

        #region -Fraccionesimpuestas-
        public async Task<IActionResult> EditFraccionesimpuestas(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Fraccionesimpuestas.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }
            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFraccionesimpuestas(int id, [Bind("IdFracciones,Tipo,Autoridad,FechaInicio,FechaTermino,Estado,Evidencia,FiguraJudicial,SupervisionIdSupervision")] Fraccionesimpuestas fraccionesimpuestas)
        {
            if (id != fraccionesimpuestas.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fraccionesimpuestas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(fraccionesimpuestas.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(fraccionesimpuestas);
        }
        #endregion

        #region -EditPlaneacionestrategica-
        public async Task<IActionResult> EditPlaneacionestrategica(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Planeacionestrategica.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            ViewBag.listaPlanSupervision = listaNaSiNo;
            ViewBag.idPlanSupervision = BuscaId(listaNaSiNo, supervision.PlanSupervision);


            #region Estado Suprvición
            List<SelectListItem> ListaPeriodicidadFirma;
            ListaPeriodicidadFirma = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Diaria", Value = "DIARIA" },
                new SelectListItem{ Text = "Semanal", Value = "SEMANAL" },
                new SelectListItem{ Text = "Mensual", Value = "MENSUAL" },
                new SelectListItem{ Text = "Bimestral", Value = "BIMESTRAL" },
                new SelectListItem{ Text = "Trimestral", Value = "TRIMESTRAL" },
                new SelectListItem{ Text = "Semestral", Value = "SEMESTRAL" },
                new SelectListItem{ Text = "Anual", Value = "ANUAL" },
                };

            ViewBag.listaPeriodicidadFirma = ListaPeriodicidadFirma;
            ViewBag.idPeriodicidadFirma = BuscaId(ListaPeriodicidadFirma, supervision.PeriodicidadFirma);
            #endregion


            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlaneacionestrategica(int id, [Bind("IdPlaneacionEstrategica,PlanSupervision,MotivoNoPlaneacion,FechaAprobacion,UltimoInforme,FechaInforme,FechaUltimoContacto,FechaProximoContacto,DiaFirma,PeriodicidadFirma,SupervisionIdSupervision")] Planeacionestrategica planeacionestrategica)
        {
            if (id != planeacionestrategica.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planeacionestrategica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(planeacionestrategica.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(planeacionestrategica);
        }
        #endregion

        #region -Revocacion-
        public async Task<IActionResult> EditRevocacion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Revocacion.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaRevocado = listaNaSiNo;
            ViewBag.idRevocado = BuscaId(listaNaSiNo, supervision.Revocado);




            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRevocacion(int id, [Bind("IdRevocacion,Revocado,FechaAprobacion,MotivoRevocacion,SupervisionIdSupervision")] Revocacion revocacion)
        {
            if (id != revocacion.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(revocacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(revocacion.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(revocacion);
        }
        #endregion

        #region -EditSuspensionseguimiento-
        public async Task<IActionResult> EditSuspensionseguimiento(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Suspensionseguimiento.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaSuspendido = listaNaSiNo;
            ViewBag.idSuspendido = BuscaId(listaNaSiNo, supervision.Suspendido);


            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSuspensionseguimiento(int id, [Bind("IdSuspensionSeguimiento,Suspendido,FechaAprobacion,MotivoSuspension,SupervisionIdSupervision")] Suspensionseguimiento suspensionseguimiento)
        {
            if (id != suspensionseguimiento.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suspensionseguimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(suspensionseguimiento.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View();
        }
        #endregion



        #region -Graficos-
        #endregion


    }
}
