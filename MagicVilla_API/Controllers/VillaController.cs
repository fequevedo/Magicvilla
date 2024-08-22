﻿using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private ApplicationDBcontext _db;

        public VillaController(ILogger<VillaController> logger, ApplicationDBcontext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Obtener las villas");
            //return Ok(VillaStore.villaList);
            return  Ok(_db.Villas.ToList());
        }

        [HttpGet("id", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id) {

            if (id == 0) {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            var villa = _db.Villas.FirstOrDefault(x => x.Id == id);

            if (villa == null) {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            else if (_db.Villas.FirstOrDefault(x => x.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null) {
                ModelState.AddModelError("NombreExiste", "La villa con ese Nombre ya existe");
                return BadRequest(ModelState);
            }
            else if (villaDto == null)
            {
                return BadRequest(villaDto);
            }
            else if (villaDto.Id > 0) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            //villaDto.Id = VillaStore.villaList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villaDto);
            //return Ok(villaDto);

            Villa modelo = new Villa()
            {
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };

            _db.Villas.Add(modelo);
            _db.SaveChanges();

            return CreatedAtRoute("GetVilla", new { Id = villaDto.Id }, villaDto);
        }

        [HttpDelete("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id) {
            if (id == 0) { 
                return BadRequest();
            }
            
            //var villa = VillaStore.villaList.FirstOrDefault(x=> x.Id==id);
            var villa = _db.Villas.FirstOrDefault(x => x.Id == id);

            if (villa == null) { 
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("id:int")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto) {

            if(villaDto==null || id!=villaDto.Id){ 
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            //villa.Nombre= villaDto.Nombre;
            //villa.Ocupantes= villaDto.Ocupantes;
            //villa.MetrosCuadrados=villaDto.MetrosCuadrados;

            Villa modelo = new ()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };

            _db.Villas.Update(modelo);
            _db.SaveChanges();

            return  NoContent();
        }

        [HttpPatch("id:int")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {

            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);

            var villa = _db.Villas.AsNoTracking().FirstOrDefault(x => x.Id == id);

            VillaDto villaDto = new ()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };

            if (villa == null) { 
                return BadRequest();
            }

            patchDto.ApplyTo(villaDto,ModelState);

            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }

            Villa modelo = new ()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };

            _db.Villas.Update(modelo);
            _db.SaveChanges();

            return NoContent();
        }


    }
}
