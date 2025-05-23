﻿using ApiTuEvento_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using WebApiRest.Helpers;

namespace ApiTuEvento_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ContextDB _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(ContextDB context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == loginDto.NombreUsuario);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Contraseña, user.Contraseña))
            {
                return Unauthorized("Credenciales inválidas");
            }

            var token = _jwtHelper.GenerateToken(user.NombreUsuario, user.Rol); // Mejor pasar también el rol

            return Ok(new
            {
                token,
                user = new
                {
                    user.NombreUsuario,
                    user.Correo,
                    user.Rol
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.usuarios.AnyAsync(u => u.NombreUsuario == dto.NombreUsuario))
                return BadRequest("El usuario ya existe.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Contraseña);

            var user = new Usuario
            {
                Cedula = dto.Cedula,
                Rol = dto.Rol,
                NombreUsuario = dto.NombreUsuario,
                Correo = dto.Correo,
                Contraseña = hashedPassword,
                FechaNacimiento = dto.FechaNacimiento
            };

            _context.usuarios.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente.");
        }
    }
}

