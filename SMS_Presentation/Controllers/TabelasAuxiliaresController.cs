using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SMS_Presentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_CRM_Solution.ViewModels;
using System.IO;
using Correios.Net;
using Canducci.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EntitiesServices.Attributes;
using OfficeOpenXml.Table;
using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using CrossCutting;
using System.Net.Mail;
using System.Net.Http;

namespace ERP_CRM_Solution.Controllers
{
    public class TabelasAuxiliaresController : Controller
    {
        private readonly ICategoriaClienteAppService ccApp;
        private readonly ICargoAppService caApp;
        private readonly ICRMOrigemAppService orApp;
        private readonly IMotivoCancelamentoAppService mcApp;
        private readonly IMotivoEncerramentoAppService meApp;
        private readonly ITipoAcaoAppService taApp;
        private readonly ICategoriaEquipamentoAppService ceApp;
        private readonly ICategoriaFornecedorAppService cfApp;

        private String msg;
        private Exception exception;
        CATEGORIA_CLIENTE objetoCC = new CATEGORIA_CLIENTE();
        CATEGORIA_CLIENTE objetoCCAntes = new CATEGORIA_CLIENTE();
        List<CATEGORIA_CLIENTE> listaMasterCC = new List<CATEGORIA_CLIENTE>();
        CARGO objetoCG = new CARGO();
        CARGO objetoCGAntes = new CARGO();
        List<CARGO> listaMasterCG = new List<CARGO>();
        CRM_ORIGEM objetoOR = new CRM_ORIGEM();
        CRM_ORIGEM objetoORAntes = new CRM_ORIGEM();
        List<CRM_ORIGEM> listaMasterOR = new List<CRM_ORIGEM>();
        MOTIVO_CANCELAMENTO objetoMC = new MOTIVO_CANCELAMENTO();
        MOTIVO_CANCELAMENTO objetoMCAntes = new MOTIVO_CANCELAMENTO();
        List<MOTIVO_CANCELAMENTO> listaMasterMC = new List<MOTIVO_CANCELAMENTO>();
        MOTIVO_ENCERRAMENTO objetoME = new MOTIVO_ENCERRAMENTO();
        MOTIVO_ENCERRAMENTO objetoMEAntes = new MOTIVO_ENCERRAMENTO();
        List<MOTIVO_ENCERRAMENTO> listaMasterME = new List<MOTIVO_ENCERRAMENTO>();
        TIPO_ACAO objetoTA = new TIPO_ACAO();
        TIPO_ACAO objetoTAAntes = new TIPO_ACAO();
        List<TIPO_ACAO> listaMasterTA = new List<TIPO_ACAO>();
        CATEGORIA_EQUIPAMENTO objetoCE = new CATEGORIA_EQUIPAMENTO();
        CATEGORIA_EQUIPAMENTO objetoCEAntes = new CATEGORIA_EQUIPAMENTO();
        List<CATEGORIA_EQUIPAMENTO> listaMasterCE = new List<CATEGORIA_EQUIPAMENTO>();
        CATEGORIA_FORNECEDOR objetoCF = new CATEGORIA_FORNECEDOR();
        CATEGORIA_FORNECEDOR objetoCFAntes = new CATEGORIA_FORNECEDOR();
        List<CATEGORIA_FORNECEDOR> listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
        String extensao;

        public TabelasAuxiliaresController(ICategoriaClienteAppService ccApps, ICargoAppService caApps, ICRMOrigemAppService orApps, IMotivoCancelamentoAppService mcApps, IMotivoEncerramentoAppService meApps, ITipoAcaoAppService taApps, ICategoriaFornecedorAppService cfApps, ICategoriaEquipamentoAppService ceApps)
        {
            ccApp = ccApps;
            caApp = caApps;
            orApp = orApps;
            mcApp = mcApps;
            meApp = meApps;
            taApp = taApps;
            ceApp = ceApps;
            cfApp = cfApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {

            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaCatCliente()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CATEGORIA_CLIENTE>)Session["ListaCatCliente"] == null)
            {
                listaMasterCC = ccApp.GetAllItens(idAss);
                Session["ListaCatCliente"] = listaMasterCC;
            }
            ViewBag.Listas = (List<CATEGORIA_CLIENTE>)Session["ListaCatCliente"];
            Session["CatCliente"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCatCliente"] != null)
            {
                if ((Int32)Session["MensCatCliente"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCliente"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0066", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCliente"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0069", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatCliente"] = 1;
            objetoCC = new CATEGORIA_CLIENTE();
            return View(objetoCC);
        }

        public ActionResult RetirarFiltroCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        public ActionResult MostrarTudoCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCC = ccApp.GetAllItensAdm(idAss);
            Session["ListaCatCliente"] = listaMasterCC;
            return RedirectToAction("MontarTelaCatCliente");
        }

        public ActionResult VoltarBaseCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult IncluirCatCliente()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatCliente"] = 2;
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            CATEGORIA_CLIENTE item = new CATEGORIA_CLIENTE();
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CACL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCatCliente(CategoriaClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ccApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatCliente"] = 3;
                        return RedirectToAction("MontarTelaCatCliente");
                    }

                    // Sucesso
                    listaMasterCC = new List<CATEGORIA_CLIENTE>();
                    Session["ListaCatCliente"] = null;
                    Session["IdCatCliente"] = item.CACL_CD_ID;
                    return RedirectToAction("MontarTelaCatCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCatCliente(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatCliente"] = 2;
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            Session["CatCliente"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensCatCliente"] != null)
            {


            }

            objetoCCAntes = item;
            Session["IdCatCliente"] = id;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatCliente(CategoriaClienteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    Int32 volta = ccApp.ValidateEdit(item, objetoCCAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCC = new List<CATEGORIA_CLIENTE>();
                    Session["ListaCatCliente"] = null;
                    return RedirectToAction("MontarTelaCatCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoCatCliente()
        {

            return RedirectToAction("EditarCatCliente", new { id = (Int32)Session["IdCatCliente"] });
        }

        [HttpGet]
        public ActionResult ExcluirCatCliente(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatCliente"] = 2;
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            objetoCCAntes = (CATEGORIA_CLIENTE)Session["CatCliente"];
            item.CACL_IN_ATIVO = 0;
            Int32 volta = ccApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatCliente"] = 4;
                return RedirectToAction("MontarTelaCatCliente");
            }
            Session["ListaCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult ReativarCatCliente(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatCliente"] = 2;
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            objetoCCAntes = (CATEGORIA_CLIENTE)Session["CatCliente"];
            item.CACL_IN_ATIVO = 1;
            Int32 volta = ccApp.ValidateReativar(item, usuario);
            Session["ListaCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult MontarTelaCargo()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CARGO>)Session["ListaCargo"] == null)
            {
                listaMasterCG = caApp.GetAllItens(idAss);
                Session["ListaCargo"] = listaMasterCG;
            }
            ViewBag.Listas = (List<CARGO>)Session["ListaCargo"];
            Session["Cargo"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCargo"] != null)
            {
                if ((Int32)Session["MensCargo"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCargo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0067", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCargo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0068", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCargo"] = 1;
            objetoCG = new CARGO();
            return View(objetoCG);
        }

        public ActionResult RetirarFiltroCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }

        public ActionResult MostrarTudoCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCG = caApp.GetAllItensAdm(idAss);
            Session["ListaCargo"] = listaMasterCG;
            return RedirectToAction("MontarTelaCargo");
        }

        public ActionResult VoltarBaseCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult IncluirCargo()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCargo"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            CARGO item = new CARGO();
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CARG_IN_ATIVO = 1;
            vm.CARG_IN_TIPO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCargo(CargoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = caApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCargo"] = 3;
                        return RedirectToAction("MontarTelaCargo");
                    }

                    // Sucesso
                    listaMasterCC = new List<CATEGORIA_CLIENTE>();
                    Session["ListaCargo"] = null;
                    Session["IdCargo"] = item.CARG_CD_ID;
                    return RedirectToAction("MontarTelaCargo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCargo(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCargo"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CARGO item = caApp.GetItemById(id);
            Session["Cargo"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensCargo"] != null)
            {


            }

            objetoCGAntes = item;
            Session["IdCargo"] = id;
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCargo(CargoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                    Int32 volta = caApp.ValidateEdit(item, objetoCGAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCG = new List<CARGO>();
                    Session["ListaCargo"] = null;
                    return RedirectToAction("MontarTelaCargo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoCargo()
        {

            return RedirectToAction("EditarCargo", new { id = (Int32)Session["IdCargo"] });
        }

        [HttpGet]
        public ActionResult ExcluirCargo(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCargo"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CARGO item = caApp.GetItemById(id);
            objetoCGAntes = (CARGO)Session["Cargo"];
            item.CARG_IN_ATIVO = 0;
            Int32 volta = caApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCargo"] = 4;
                return RedirectToAction("MontarTelaCargo");
            }
            Session["ListaCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult ReativarCargo(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCargo"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CARGO item = caApp.GetItemById(id);
            objetoCGAntes = (CARGO)Session["Cargo"];
            item.CARG_IN_ATIVO = 1;
            Int32 volta = caApp.ValidateReativar(item, usuario);
            Session["ListaCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult MontarTelaOrigem()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CRM_ORIGEM>)Session["ListaOrigem"] == null)
            {
                listaMasterOR = orApp.GetAllItens(idAss);
                Session["ListaOrigem"] = listaMasterOR;
            }
            ViewBag.Listas = (List<CRM_ORIGEM>)Session["ListaOrigem"];
            Session["Cargo"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensOrigem"] != null)
            {
                if ((Int32)Session["MensOrigem"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrigem"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0070", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOrigem"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0071", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaOrigem"] = 1;
            objetoOR = new CRM_ORIGEM();
            return View(objetoOR);
        }

        public ActionResult RetirarFiltroOrigem()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaOrigem"] = null;
            return RedirectToAction("MontarTelaOrigem");
        }

        public ActionResult MostrarTudoOrigem()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterOR = orApp.GetAllItensAdm(idAss);
            Session["ListaOrigem"] = listaMasterOR;
            return RedirectToAction("MontarTelaOrigem");
        }

        public ActionResult VoltarBaseOrigem()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaOrigem");
        }

        [HttpGet]
        public ActionResult IncluirOrigem()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensOrigem"] = 2;
                    return RedirectToAction("MontarTelaOrigem");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            CRM_ORIGEM item = new CRM_ORIGEM();
            CRMOrigemViewModel vm = Mapper.Map<CRM_ORIGEM, CRMOrigemViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CROR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirOrigem(CRMOrigemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_ORIGEM item = Mapper.Map<CRMOrigemViewModel, CRM_ORIGEM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = orApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensOrigem"] = 3;
                        return RedirectToAction("MontarTelaOrigem");
                    }

                    // Sucesso
                    listaMasterOR = new List<CRM_ORIGEM>();
                    Session["ListaOrigem"] = null;
                    Session["IdOrigem"] = item.CROR_CD_ID;
                    return RedirectToAction("MontarTelaOrigem");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarOrigem(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensOrigem"] = 2;
                    return RedirectToAction("MontarTelaOrigem");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_ORIGEM item = orApp.GetItemById(id);
            Session["Origem"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensOrigem"] != null)
            {


            }

            objetoORAntes = item;
            Session["IdOrigem"] = id;
            CRMOrigemViewModel vm = Mapper.Map<CRM_ORIGEM, CRMOrigemViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarOrigem(CRMOrigemViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CRM_ORIGEM item = Mapper.Map<CRMOrigemViewModel, CRM_ORIGEM>(vm);
                    Int32 volta = orApp.ValidateEdit(item, objetoORAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterOR = new List<CRM_ORIGEM>();
                    Session["ListaOrigem"] = null;
                    return RedirectToAction("MontarTelaOrigem");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoOrigem()
        {

            return RedirectToAction("EditarOrigem", new { id = (Int32)Session["IdOrigem"] });
        }

        [HttpGet]
        public ActionResult ExcluirOrigem(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensOrigem"] = 2;
                    return RedirectToAction("MontarTelaOrigem");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_ORIGEM item = orApp.GetItemById(id);
            objetoORAntes = (CRM_ORIGEM)Session["Origem"];
            item.CROR_IN_ATIVO = 0;
            Int32 volta = orApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensOrigem"] = 4;
                return RedirectToAction("MontarTelaOrigem");
            }
            Session["ListaOrigem"] = null;
            return RedirectToAction("MontarTelaOrigem");
        }

        [HttpGet]
        public ActionResult ReativarOrigem(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensOrigem"] = 2;
                    return RedirectToAction("MontarTelaOrigem");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_ORIGEM item = orApp.GetItemById(id);
            objetoORAntes = (CRM_ORIGEM)Session["Origem"];
            item.CROR_IN_ATIVO = 1;
            Int32 volta = orApp.ValidateReativar(item, usuario);
            Session["ListaOrigem"] = null;
            return RedirectToAction("MontarTelaOrigem");
        }

        [HttpGet]
        public ActionResult MontarTelaMotCancelamento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<MOTIVO_CANCELAMENTO>)Session["ListaMotCancelamento"] == null)
            {
                listaMasterMC = mcApp.GetAllItens(idAss);
                Session["ListaMotCancelamento"] = listaMasterMC;
            }
            ViewBag.Listas = (List<MOTIVO_CANCELAMENTO>)Session["ListaMotCancelamento"];
            Session["MotCancelamento"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMotCancelamento"] != null)
            {
                if ((Int32)Session["MensMotCancelamento"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotCancelamento"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0072", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotCancelamento"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMotCancelamento"] = 1;
            objetoMC = new MOTIVO_CANCELAMENTO();
            return View(objetoMC);
        }

        public ActionResult RetirarFiltroMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        public ActionResult MostrarTudoMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterMC = mcApp.GetAllItensAdm(idAss);
            Session["ListaMotCancelamento"] = listaMasterMC;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        public ActionResult VoltarBaseMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult IncluirMotCancelamento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensMotCancelamento"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            MOTIVO_CANCELAMENTO item = new MOTIVO_CANCELAMENTO();
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.MOCA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirMotCancelamento(MotivoCancelamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    MOTIVO_CANCELAMENTO item = Mapper.Map<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = mcApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMotCancelamento"] = 3;
                        return RedirectToAction("MontarTelaMotCancelamento");
                    }

                    // Sucesso
                    listaMasterMC = new List<MOTIVO_CANCELAMENTO>();
                    Session["ListaMotCancelamento"] = null;
                    Session["IdMotCancelamento"] = item.MOCA_CD_ID;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarMotCancelamento(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensMotCancelamento"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            Session["MotCancelamento"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensMotCancelamento"] != null)
            {


            }

            objetoMCAntes = item;
            Session["IdMotCancelamento"] = id;
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMotCancelamento(MotivoCancelamentoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    MOTIVO_CANCELAMENTO item = Mapper.Map<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>(vm);
                    Int32 volta = mcApp.ValidateEdit(item, objetoMCAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterMC = new List<MOTIVO_CANCELAMENTO>();
                    Session["ListaMotCancelamento"] = null;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoMotCancelamento()
        {

            return RedirectToAction("EditarMotCancelamento", new { id = (Int32)Session["IdMotCancelamento"] });
        }

        [HttpGet]
        public ActionResult ExcluirMotCancelamento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensMotCancelamento"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoMCAntes = (MOTIVO_CANCELAMENTO)Session["MotCancelamento"];
            item.MOCA_IN_ATIVO = 0;
            Int32 volta = mcApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMotCancelamento"] = 4;
                return RedirectToAction("MontarTelaMotCancelamento");
            }
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult ReativarMotCancelamento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensMotCancelamento"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoMCAntes = (MOTIVO_CANCELAMENTO)Session["MotCancelamento"];
            item.MOCA_IN_ATIVO = 1;
            Int32 volta = mcApp.ValidateReativar(item, usuario);
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult MontarTelaMotEncerramento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<MOTIVO_ENCERRAMENTO>)Session["ListaMotEncerramento"] == null)
            {
                listaMasterME = meApp.GetAllItens(idAss);
                Session["ListaMotEncerramento"] = listaMasterME;
            }
            ViewBag.Listas = (List<MOTIVO_ENCERRAMENTO>)Session["ListaMotEncerramento"];
            Session["MotEncerramento"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMotEncerramento"] != null)
            {
                if ((Int32)Session["MensMotEncerramento"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotEncerramento"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0077", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotEncerramento"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMotEncerramento"] = 1;
            objetoME = new MOTIVO_ENCERRAMENTO();
            return View(objetoME);
        }

        public ActionResult RetirarFiltroMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        public ActionResult MostrarTudoMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterME = meApp.GetAllItensAdm(idAss);
            Session["ListaMotEncerramento"] = listaMasterME;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        public ActionResult VoltarBaseMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult IncluirMotEncerramento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensMotEncerramento"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            MOTIVO_ENCERRAMENTO item = new MOTIVO_ENCERRAMENTO();
            MotivoEncerramentoViewModel vm = Mapper.Map<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.MOEN_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirMotEncerramento(MotivoEncerramentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    MOTIVO_ENCERRAMENTO item = Mapper.Map<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = meApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMotEncerramento"] = 3;
                        return RedirectToAction("MontarTelaMotEncerramento");
                    }

                    // Sucesso
                    listaMasterME = new List<MOTIVO_ENCERRAMENTO>();
                    Session["ListaMotEncerramento"] = null;
                    Session["IdMotEncerramento"] = item.MOEN_CD_ID;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarMotEncerramento(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensMotEncerramento"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            Session["MotEncerramento"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensMotEncerramento"] != null)
            {


            }

            objetoMEAntes = item;
            Session["IdMotEncerramento"] = id;
            MotivoEncerramentoViewModel vm = Mapper.Map<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMotEncerramento(MotivoEncerramentoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    MOTIVO_ENCERRAMENTO item = Mapper.Map<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>(vm);
                    Int32 volta = meApp.ValidateEdit(item, objetoMEAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterME = new List<MOTIVO_ENCERRAMENTO>();
                    Session["ListaMotEncerramento"] = null;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoMotEncerramento()
        {

            return RedirectToAction("EditarMotEncerramento", new { id = (Int32)Session["IdMotEncerramento"] });
        }

        [HttpGet]
        public ActionResult ExcluirMotEncerramento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensMotEncerramento"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            objetoMEAntes = (MOTIVO_ENCERRAMENTO)Session["MotEncerramento"];
            item.MOEN_IN_ATIVO = 0;
            Int32 volta = meApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMotEncerramento"] = 4;
                return RedirectToAction("MontarTelaMotEncerramento");
            }
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult ReativarMotEncerramento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensMotEncerramento"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            objetoMEAntes = (MOTIVO_ENCERRAMENTO)Session["MotEncerramento"];
            item.MOEN_IN_ATIVO = 1;
            Int32 volta = meApp.ValidateReativar(item, usuario);
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoAcao()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<TIPO_ACAO>)Session["ListaTipoAcao"] == null)
            {
                listaMasterTA = taApp.GetAllItens(idAss);
                Session["ListaTipoAcao"] = listaMasterTA;
            }
            ViewBag.Listas = (List<TIPO_ACAO>)Session["ListaTipoAcao"];
            Session["TipoAcao"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensTipoAcao"] != null)
            {
                if ((Int32)Session["MensTipoAcao"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoAcao"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0078", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoAcao"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0079", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoAcao"] = 1;
            objetoTA = new TIPO_ACAO();
            return View(objetoTA);
        }

        public ActionResult RetirarFiltroTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        public ActionResult MostrarTudoTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTA = taApp.GetAllItensAdm(idAss);
            Session["ListaTipoAcao"] = listaMasterTA;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        public ActionResult VoltarBaseTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult IncluirTipoAcao()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensTipoAcao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            TIPO_ACAO item = new TIPO_ACAO();
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.TIAC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirTipoAcao(TipoAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TIPO_ACAO item = Mapper.Map<TipoAcaoViewModel, TIPO_ACAO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = taApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoAcao"] = 3;
                        return RedirectToAction("MontarTelaTipoAcao");
                    }

                    // Sucesso
                    listaMasterTA = new List<TIPO_ACAO>();
                    Session["ListaTipoAcao"] = null;
                    Session["IdTipoAcao"] = item.TIAC_CD_ID;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoAcao(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensTipoAcao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TIPO_ACAO item = taApp.GetItemById(id);
            Session["TipoAcao"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensTipoAcao"] != null)
            {


            }

            objetoTAAntes = item;
            Session["IdTipoAcao"] = id;
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoAcao(TipoAcaoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TIPO_ACAO item = Mapper.Map<TipoAcaoViewModel, TIPO_ACAO>(vm);
                    Int32 volta = taApp.ValidateEdit(item, objetoTAAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTA = new List<TIPO_ACAO>();
                    Session["ListaTipoAcao"] = null;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoTipoAcao()
        {

            return RedirectToAction("EditarTipoAcao", new { id = (Int32)Session["IdTipoAcao"] });
        }

        [HttpGet]
        public ActionResult ExcluirTipoAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensTipoAcao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TIPO_ACAO item = taApp.GetItemById(id);
            objetoTAAntes = (TIPO_ACAO)Session["TipoAcao"];
            item.TIAC_IN_ATIVO = 0;
            Int32 volta = taApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoAcao"] = 4;
                return RedirectToAction("MontarTelaTipoAcao");
            }
            Session["ListaTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult ReativarTipoAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensTipoAcao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TIPO_ACAO item = taApp.GetItemById(id);
            objetoTAAntes = (TIPO_ACAO)Session["TipoAcao"];
            item.TIAC_IN_ATIVO = 1;
            Int32 volta = taApp.ValidateReativar(item, usuario);
            Session["ListaTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult MontarTelaCatEquipamento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CATEGORIA_EQUIPAMENTO>)Session["ListaCatEquipamento"] == null)
            {
                listaMasterCE = ceApp.GetAllItens(idAss);
                Session["ListaCatEquipamento"] = listaMasterCE;
            }
            ViewBag.Listas = (List<CATEGORIA_EQUIPAMENTO>)Session["ListaCatEquipamento"];
            Session["CatEquipamento"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCatEquipamento"] != null)
            {
                if ((Int32)Session["MensCatEquipamento"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatEquipamento"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0082", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatEquipamento"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0069", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatEquipamento"] = 1;
            objetoCE = new CATEGORIA_EQUIPAMENTO();
            return View(objetoCE);
        }

        public ActionResult RetirarFiltroCatEquipamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCatEquipamento"] = null;
            return RedirectToAction("MontarTelaCatEquipamento");
        }

        public ActionResult MostrarTudoCatEquipamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCE = ceApp.GetAllItensAdm(idAss);
            Session["ListaCatEquipamento"] = listaMasterCE;
            return RedirectToAction("MontarTelaCatEquipamento");
        }

        public ActionResult VoltarBaseCatEquipamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatEquipamento");
        }

        [HttpGet]
        public ActionResult IncluirCatEquipamento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatEquipamento"] = 2;
                    return RedirectToAction("MontarTelaCatEquipamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            CATEGORIA_EQUIPAMENTO item = new CATEGORIA_EQUIPAMENTO();
            CategoriaEquipamentoViewModel vm = Mapper.Map<CATEGORIA_EQUIPAMENTO, CategoriaEquipamentoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CAEQ_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCatEquipamento(CategoriaEquipamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_EQUIPAMENTO item = Mapper.Map<CategoriaEquipamentoViewModel, CATEGORIA_EQUIPAMENTO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ceApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatEquipamento"] = 3;
                        return RedirectToAction("MontarTelaCatEquipamento");
                    }

                    // Sucesso
                    listaMasterCE = new List<CATEGORIA_EQUIPAMENTO>();
                    Session["ListaCatEquipamento"] = null;
                    Session["IdCatEquipamento"] = item.CAEQ_CD_ID;
                    return RedirectToAction("MontarTelaCatEquipamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCatEquipamento(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatEquipamento"] = 2;
                    return RedirectToAction("MontarTelaCatEquipamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_EQUIPAMENTO item = ceApp.GetItemById(id);
            Session["CatEquipamento"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensCatEquipamento"] != null)
            {


            }

            objetoCEAntes = item;
            Session["IdCatEquipamento"] = id;
            CategoriaEquipamentoViewModel vm = Mapper.Map<CATEGORIA_EQUIPAMENTO, CategoriaEquipamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatEquipamento(CategoriaEquipamentoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_EQUIPAMENTO item = Mapper.Map<CategoriaEquipamentoViewModel, CATEGORIA_EQUIPAMENTO>(vm);
                    Int32 volta = ceApp.ValidateEdit(item, objetoCEAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCE = new List<CATEGORIA_EQUIPAMENTO>();
                    Session["ListaCatEquipamento"] = null;
                    return RedirectToAction("MontarTelaCatEquipamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoCatEquipamento()
        {

            return RedirectToAction("EditarCatEquipamento", new { id = (Int32)Session["IdCatEquipamento"] });
        }

        [HttpGet]
        public ActionResult ExcluirCatEquipamento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatEquipamento"] = 2;
                    return RedirectToAction("MontarTelaCatEquipamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_EQUIPAMENTO item = ceApp.GetItemById(id);
            objetoCEAntes = (CATEGORIA_EQUIPAMENTO)Session["CatEquipamento"];
            item.CAEQ_IN_ATIVO = 0;
            Int32 volta = ceApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatEquipamento"] = 4;
                return RedirectToAction("MontarTelaCatEquipamento");
            }
            Session["ListaCatEquipamento"] = null;
            return RedirectToAction("MontarTelaCatEquipamento");
        }

        [HttpGet]
        public ActionResult ReativarCatEquipamento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatEquipamento"] = 2;
                    return RedirectToAction("MontarTelaCatEquipamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_EQUIPAMENTO item = ceApp.GetItemById(id);
            objetoCEAntes = (CATEGORIA_EQUIPAMENTO)Session["CatEquipamento"];
            item.CAEQ_IN_ATIVO = 1;
            Int32 volta = ceApp.ValidateReativar(item, usuario);
            Session["ListaCatEquipamento"] = null;
            return RedirectToAction("MontarTelaCatEquipamento");
        }

        [HttpGet]
        public ActionResult MontarTelaCatFornecedor()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CATEGORIA_FORNECEDOR>)Session["ListaCatFornecedor"] == null)
            {
                listaMasterCF = cfApp.GetAllItens(idAss);
                Session["ListaCatFornecedor"] = listaMasterCF;
            }
            ViewBag.Listas = (List<CATEGORIA_FORNECEDOR>)Session["ListaCatFornecedor"];
            Session["CatFornecedor"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCatFornecedor"] != null)
            {
                if ((Int32)Session["MensCatFornecedor"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatFornecedor"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0083", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatFornecedor"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0069", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatFornecedor"] = 1;
            objetoCF = new CATEGORIA_FORNECEDOR();
            return View(objetoCF);
        }

        public ActionResult RetirarFiltroCatFornecedor()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCatFornecedor"] = null;
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        public ActionResult MostrarTudoCatFornecedor()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCF = cfApp.GetAllItensAdm(idAss);
            Session["ListaCatFornecedor"] = listaMasterCF;
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        public ActionResult VoltarBaseCatFornecedor()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        [HttpGet]
        public ActionResult IncluirCatFornecedor()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatFornecedor"] = 2;
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            CATEGORIA_FORNECEDOR item = new CATEGORIA_FORNECEDOR();
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CAFO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCatFornecedor(CategoriaFornecedorViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_FORNECEDOR item = Mapper.Map<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cfApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatFornecedor"] = 3;
                        return RedirectToAction("MontarTelaCatFornecedor");
                    }

                    // Sucesso
                    listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
                    Session["ListaCatFornecedor"] = null;
                    Session["IdCatFornecedor"] = item.CAFO_CD_ID;
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCatFornecedor(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatFornecedor"] = 2;
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            Session["CatFornecedor"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensCatFornecedor"] != null)
            {


            }

            objetoCFAntes = item;
            Session["IdCatFornecedor"] = id;
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatFornecedor(CategoriaFornecedorViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_FORNECEDOR item = Mapper.Map<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>(vm);
                    Int32 volta = cfApp.ValidateEdit(item, objetoCFAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
                    Session["ListaCatFornecedor"] = null;
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoCatFornecedor()
        {

            return RedirectToAction("EditarCatFornecedor", new { id = (Int32)Session["IdCatFornecedor"] });
        }

        [HttpGet]
        public ActionResult ExcluirCatFornecedor(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatFornecedor"] = 2;
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            objetoCFAntes = (CATEGORIA_FORNECEDOR)Session["CatFornecedor"];
            item.CAFO_IN_ATIVO = 0;
            Int32 volta = cfApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatFornecedor"] = 4;
                return RedirectToAction("MontarTelaCatFornecedor");
            }
            Session["ListaCatFornecedor"] = null;
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        [HttpGet]
        public ActionResult ReativarCatFornecedor(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCatFornecedor"] = 2;
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            objetoCFAntes = (CATEGORIA_FORNECEDOR)Session["CatFornecedor"];
            item.CAFO_IN_ATIVO = 1;
            Int32 volta = cfApp.ValidateReativar(item, usuario);
            Session["ListaCatFornecedor"] = null;
            return RedirectToAction("MontarTelaCatFornecedor");
        }

    }
}