﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Connection;
using API.Modelos;
using API.Models;
using Biblioteca.Estructuras;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using API.Connection;
using API.Models;
using MongoDB.Driver;
using MongoDB.Bson;


namespace API.Controllers
{
    [Route("api/main")]
    [ApiController]
    public class mainController : ControllerBase
    {
        [HttpPost]
        [Route("Login/{user}/{password}")]
        public IActionResult Login(string user, string password)
        {
            DbConnection connection = new DbConnection();
            var db = connection.Client.GetDatabase(connection.DBName);
            var usersCollection = db.GetCollection<Usuario>("users");
            var filter = Builders<Usuario>.Filter.Eq("user", user);
            List<Usuario> usuariosLog = usersCollection.Find<Usuario>(filter).ToList();
            Usuario userLog = UserLog(usuariosLog, user, password);
            if (userLog != null)
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("Nuevo/{nombre}/{apellido}/{password}/{user}/{email}/{llave}")]
        public IActionResult NuevoUsuario(string nombre, string apellido, string password, string user, string email, string llave)
        {
            Cesar cifrado = new Cesar("Centrifugados");
            DbConnection mongo = new DbConnection();
            try
            {
                users newUser = new users();
                newUser.Nombre = nombre;
                newUser.Apellido = apellido;
                newUser.Password = cifrado.Cifrar(password);
                newUser.User = user;
                newUser.EMail = email;
                newUser.LlaveSDES = llave;

                var json = JsonConvert.SerializeObject(newUser);
                mongo.InsertDb<users>("users", newUser);
                return StatusCode(200);
            }
            catch
            {
                return StatusCode(500);
            }

        }

        static Usuario UserLog(List<Usuario> usuariosLog, string user, string pass)
        {
            Usuario retorno = null;
            int i; bool exist = false;
            for (i = 0; i < usuariosLog.Count; i++)
            {
                if (usuariosLog[i].Password == pass && usuariosLog[i].User == user)
                {
                    exist = true;
                    break;
                }
            }
            if (exist)
            {
                retorno = usuariosLog[i];
            }
            return retorno;
        }    }
}
