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

namespace ERP_CRM_Solution.Controllers
{
    public class ConfiguracaoController : Controller
    {
        private readonly IConfiguracaoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        //private readonly ICategoriaClienteAppService catApp;

        private String msg;
        private Exception exception;
        CONFIGURACAO objeto = new CONFIGURACAO();
        CONFIGURACAO objetoAntes = new CONFIGURACAO();
        List<CONFIGURACAO> listaMaster = new List<CONFIGURACAO>();
        //CATEGORIA_CLIENTE objetoCat = new CATEGORIA_CLIENTE();
        //CATEGORIA_CLIENTE objetoCatAntes = new CATEGORIA_CLIENTE();
        //List<CATEGORIA_CLIENTE> listaMasterCat = new List<CATEGORIA_CLIENTE>();
        String extensao;

        public ConfiguracaoController(IConfiguracaoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            //catApp = catApps;
            //posApp = posApps;
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

        [HttpPost]
        public JsonResult GetConfiguracao()
        {
            var config = baseApp.GetItemById(2);
            var serialConfig = new CONFIGURACAO
            {
                CONF_CD_ID = config.CONF_CD_ID,
                ASSI_CD_ID = config.ASSI_CD_ID,
                CONF_NR_FALHAS_DIA = config.CONF_NR_FALHAS_DIA,
                CONF_NM_HOST_SMTP = config.CONF_NM_HOST_SMTP,
                CONF_NM_PORTA_SMTP = config.CONF_NM_PORTA_SMTP,
                CONF_NM_EMAIL_EMISSOO = config.CONF_NM_EMAIL_EMISSOO,
                CONF_NM_SENHA_EMISSOR = config.CONF_NM_SENHA_EMISSOR,
                CONF_NR_REFRESH_DASH = config.CONF_NR_REFRESH_DASH,
                CONF_NM_ARQUIVO_ALARME = config.CONF_NM_ARQUIVO_ALARME,
                CONF_NR_REFRESH_NOTIFICACAO = config.CONF_NR_REFRESH_NOTIFICACAO,
                CONF_SG_LOGIN_SMS = config.CONF_SG_LOGIN_SMS,
                CONF_SG_SENHA_SMS = config.CONF_SG_SENHA_SMS,
            };

            return Json(serialConfig);
        }

        [HttpGet]
        public ActionResult MontarTelaConfiguracao()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
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
            objeto = baseApp.GetItemById(idAss);
            Session["Configuracao"] = objeto;

            ViewBag.Listas = (CONFIGURACAO)Session["Configuracao"];
            ViewBag.Title = "Configuracao";
            var listaGrid = new List<SelectListItem>();
            listaGrid.Add(new SelectListItem() { Text = "10", Value = "10" });
            listaGrid.Add(new SelectListItem() { Text = "25", Value = "25" });
            listaGrid.Add(new SelectListItem() { Text = "50", Value = "50" });
            listaGrid.Add(new SelectListItem() { Text = "100", Value = "100" });
            ViewBag.ListaGrid = new SelectList(listaGrid, "Value", "Text");

            // Indicadores

            // Mensagem

            // Abre view
            Session["MensConfiguracao"] = 0;
            objetoAntes = objeto;
            if (objeto.CONF_NR_FALHAS_DIA == null)
            {
                objeto.CONF_NR_FALHAS_DIA = 3;
            }
            Session["Configuracao"] = objeto;
            Session["IdConf"] = 1;
            ConfiguracaoViewModel vm = Mapper.Map<CONFIGURACAO, ConfiguracaoViewModel>(objeto);
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaConfiguracao(ConfiguracaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaGrid = new List<SelectListItem>();
            listaGrid.Add(new SelectListItem() { Text = "10", Value = "10" });
            listaGrid.Add(new SelectListItem() { Text = "25", Value = "25" });
            listaGrid.Add(new SelectListItem() { Text = "50", Value = "50" });
            listaGrid.Add(new SelectListItem() { Text = "100", Value = "100" });
            ViewBag.ListaGrid = new SelectList(listaGrid, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONFIGURACAO item = Mapper.Map<ConfiguracaoViewModel, CONFIGURACAO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Sucesso
                    objeto = new CONFIGURACAO();
                    Session["ListaConfiguracao"] = null;
                    Session["Configuracao"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracao");
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

        public ActionResult VoltarBaseConfiguracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaConfiguracao");
        }

        [HttpGet]
        public ActionResult EditarConfiguracao(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONFIGURACAO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Configuracao"] = item;
            Session["IdVolta"] = id;
            ConfiguracaoViewModel vm = Mapper.Map<CONFIGURACAO, ConfiguracaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarConfiguracao(ConfiguracaoViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONFIGURACAO item = Mapper.Map<ConfiguracaoViewModel, CONFIGURACAO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Sucesso
                    objeto = new CONFIGURACAO();
                    Session["ListaConfiguracao"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracao");
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

        //[HttpGet]
        //public ActionResult MontarTelaCatCliente()
        //{
        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    if ((USUARIO)Session["UserCredentials"] != null)
        //    {
        //        usuario = (USUARIO)Session["UserCredentials"];

        //        // Verfifica permissão
        //        if (usuario.PERFIL.PERF_SG_SIGLA == "OPR" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
        //        {
        //            Session["MensConfiguracao"] = 2;
        //            return RedirectToAction("CarregarBase", "BaseAdmin");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];


        //    // Carrega listas
        //    if ((List<CATEGORIA_CLIENTE>)Session["ListaCat"] == null || ((List<CATEGORIA_CLIENTE>)Session["ListaCat"]).Count == 0)
        //    {
        //        listaMasterCat = catApp.GetAllItens();
        //        Session["ListaCat"] = listaMasterCat;
        //    }
        //    ViewBag.Listas = (List<CATEGORIA_CLIENTE>)Session["ListaCat"];
        //    ViewBag.Title = "CatCliente";
        //    Session["CatCliente"] = null;
        //    Session["IncluirCat"] = 0;

        //    // Indicadores
        //    ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

        //    if (Session["MensConfiguracao"] != null)
        //    {
        //        if ((Int32)Session["MensConfiguracao"] == 2)
        //        {
        //            ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
        //        }
        //        if ((Int32)Session["MensConfiguracao"] == 10)
        //        {
        //            ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0028", CultureInfo.CurrentCulture));
        //        }
        //        if ((Int32)Session["MensConfiguracao"] == 11)
        //        {
        //            ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0029", CultureInfo.CurrentCulture));
        //        }
        //    }

        //    // Abre view
        //    Session["MensConfiguracao"] = null;
        //    Session["VoltaCat"] = 1;
        //    objetoCat = new CATEGORIA_CLIENTE();
        //    return View(objetoCat);
        //}

        //public ActionResult RetirarFiltroCat()
        //{

        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    Session["ListaCat"] = null;
        //    return RedirectToAction("MontarTelaCatCliente");
        //}

        //public ActionResult MostrarTudoCat()
        //{

        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    listaMasterCat = catApp.GetAllItensAdm();
        //    Session["ListaCat"] = listaMasterCat;
        //    return RedirectToAction("MontarTelaCatCliente");
        //}

        //public ActionResult VoltarBaseCat()
        //{
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    return RedirectToAction("MontarTelaCatCliente");
        //}

        //[HttpGet]
        //public ActionResult IncluirCat()
        //{
        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    if ((USUARIO)Session["UserCredentials"] != null)
        //    {
        //        usuario = (USUARIO)Session["UserCredentials"];

        //        // Verfifica permissão
        //        if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
        //        {
        //            Session["MensConfiguracao"] = 2;
        //            return RedirectToAction("MontarTelaCatCliente", "Configuracao");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];

        //    // Prepara listas

        //    // Prepara view
        //    CATEGORIA_CLIENTE item = new CATEGORIA_CLIENTE();
        //    CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
        //    vm.ASSI_CD_ID = idAss;
        //    vm.CACL_IN_ATIVO = 1;
        //    return View(vm);
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult IncluirCat(CategoriaClienteViewModel vm)
        //{
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Executa a operação
        //            CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
        //            USUARIO usuario = (USUARIO)Session["UserCredentials"];
        //            Int32 volta = catApp.ValidateCreate(item, usuario);

        //            // Verifica retorno
        //            if (volta == 1)
        //            {
        //                Session["MensConfiguracao"] = 10;
        //                return RedirectToAction("MontarTelaCatCliente");
        //            }

        //            // Sucesso
        //            listaMasterCat = new List<CATEGORIA_CLIENTE>();
        //            Session["ListaCat"] = null;
        //            Session["IncluirCat"] = 1;
        //            Session["IdCat"] = item.CACL_CD_ID;
        //            return RedirectToAction("VoltarBaseCat");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //            return View(vm);
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        //[HttpGet]
        //public ActionResult EditarCat(Int32 id)
        //{

        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    if ((USUARIO)Session["UserCredentials"] != null)
        //    {
        //        usuario = (USUARIO)Session["UserCredentials"];

        //        // Verfifica permissão
        //        if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
        //        {
        //            Session["MensConfiguracao"] = 2;
        //            return RedirectToAction("MontarTelaCat", "Configuracao");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];

        //    CATEGORIA_CLIENTE item = catApp.GetItemById(id);
        //    Session["Cat"] = item;

        //    // Indicadores
        //    ViewBag.Incluir = (Int32)Session["IncluirCat"];

        //    // Mensagens
        //    if (Session["MensConfiguracao"] != null)
        //    {


        //    }

        //    Session["VoltaCat"] = 1;
        //    objetoCatAntes = item;
        //    Session["IdCat"] = id;
        //    CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
        //    return View(vm);
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult EditarCat(CategoriaClienteViewModel vm)
        //{
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Executa a operação
        //            USUARIO usuario = (USUARIO)Session["UserCredentials"];
        //            CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
        //            Int32 volta = catApp.ValidateEdit(item, objetoCatAntes, usuario);

        //            // Verifica retorno

        //            // Sucesso
        //            listaMasterCat = new List<CATEGORIA_CLIENTE>();
        //            Session["ListaCat"] = null;
        //            return RedirectToAction("MontarTelaCatCliente");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //            return View(vm);
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        //[HttpGet]
        //public ActionResult ExcluirCat(Int32 id)
        //{
        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    if ((USUARIO)Session["UserCredentials"] != null)
        //    {
        //        usuario = (USUARIO)Session["UserCredentials"];

        //        // Verfifica permissão
        //        if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
        //        {
        //            Session["MensConfiguracao"] = 2;
        //            return RedirectToAction("MontarTelaCatCliente");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];

        //    CATEGORIA_CLIENTE item = catApp.GetItemById(id);
        //    objetoCatAntes = (CATEGORIA_CLIENTE)Session["Cat"];
        //    item.CACL_IN_ATIVO = 0;
        //    Int32 volta = catApp.ValidateDelete(item, usuario);
        //    if (volta == 1)
        //    {
        //        Session["MensConfiguracao"] = 11;
        //        return RedirectToAction("MontarTelaCatCliente");
        //    }
        //    Session["ListaCat"] = null;
        //    return RedirectToAction("MontarTelaCatCliente");
        //}

        //[HttpGet]
        //public ActionResult ReativarCat(Int32 id)
        //{
        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    if ((USUARIO)Session["UserCredentials"] != null)
        //    {
        //        usuario = (USUARIO)Session["UserCredentials"];

        //        // Verfifica permissão
        //        if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
        //        {
        //            Session["MensConfiguracao"] = 2;
        //            return RedirectToAction("MontarTelaCatCliente");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];

        //    CATEGORIA_CLIENTE item = catApp.GetItemById(id);
        //    objetoCatAntes = (CATEGORIA_CLIENTE)Session["Cat"];
        //    item.CACL_IN_ATIVO = 1;
        //    Int32 volta = catApp.ValidateReativar(item, usuario);
        //    Session["ListaCat"] = null;
        //    return RedirectToAction("MontarTelaCatCliente");
        //}

    }
}