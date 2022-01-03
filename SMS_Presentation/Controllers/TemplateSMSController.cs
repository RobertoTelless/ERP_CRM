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
    public class TemplateSMSController : Controller
    {
        private readonly ITemplateSMSAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        TEMPLATE_SMS objeto = new TEMPLATE_SMS();
        TEMPLATE_SMS objetoAntes = new TEMPLATE_SMS();
        List<TEMPLATE_SMS> listaMaster = new List<TEMPLATE_SMS>();
        String extensao;

        public TemplateSMSController(ITemplateSMSAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
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
        public ActionResult MontarTelaTemplateSMS()
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
            if ((List<TEMPLATE_SMS>)Session["ListaTemplateSMS"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss).ToList();
                Session["ListaTemplateSMS"] = listaMaster;
            }
            ViewBag.Listas = (List<TEMPLATE_SMS>)Session["ListaTemplateSMS"];
            ViewBag.Title = "Template";
            Session["TemplateSMS"] = null;
            Session["IncluirTemplateSMS"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensTemplateSMS"] != null)
            {
                if ((Int32)Session["MensTemplateSMS"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTemplateSMS"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0032", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTemplateSMS"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0033", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTemplateSMS"] = 1;
            objeto = new TEMPLATE_SMS();
            return View(objeto);
        }

        public ActionResult RetirarFiltroTemplateSMS()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaTemplateSMS"] = null;
            return RedirectToAction("MontarTelaTemplateSMS");
        }

        public ActionResult MostrarTudoTemplateSMS()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
            Session["ListaTemplateSMS"] = null;
            return RedirectToAction("MontarTelaTemplateSMS");
        }

        public ActionResult VoltarBaseTemplateSMS()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTemplateSMS");
        }

        [HttpPost]
        public ActionResult FiltrarTemplateSMS(TEMPLATE_SMS item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<TEMPLATE_SMS> listaObj = new List<TEMPLATE_SMS>();
                Session["FiltroTemplateSMS"] = item;
                Int32 volta = baseApp.ExecuteFilter(item.TSMS_SG_SIGLA, item.TSMS_NM_NOME, item.TSMS_TX_CORPO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensTemplateSMS"] = 1;
                    return RedirectToAction("MontarTelaTemplateSMS");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaTemplateSMS"] = listaObj;
                return RedirectToAction("MontarTelaTemplateSMS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaTemplateSMS");
            }
        }

        [HttpGet]
        public ActionResult IncluirTemplateSMS()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "OPR" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensTemplate"] = 2;
                    return RedirectToAction("MontarTelaTemplateSMS", "TemplateSMS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            
            // Prepara view
            TEMPLATE_SMS item = new TEMPLATE_SMS();
            TemplateSMSViewModel vm = Mapper.Map<TEMPLATE_SMS, TemplateSMSViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.TSMS_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTemplateSMS(TemplateSMSViewModel vm)
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
                    TEMPLATE_SMS item = Mapper.Map<TemplateSMSViewModel, TEMPLATE_SMS>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTemplateSMS"] = 3;
                        return RedirectToAction("MontarTelaTemplateSMS");
                    }

                    // Sucesso
                    listaMaster = new List<TEMPLATE_SMS>();
                    Session["ListaTemplateSMS"] = null;
                    Session["IdTemplateSMS"] = item.TSMS_CD_ID;
                    return RedirectToAction("MontarTelaTemplateSMS");
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
        public ActionResult EditarTemplateSMS(Int32 id)
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
                    Session["MensTemplateSMS"] = 2;
                    return RedirectToAction("MontarTelaTemplateSMS", "TemplateSMS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TEMPLATE_SMS item = baseApp.GetItemById(id);
            Session["TemplateSMS"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensTemplateSMS"] != null)
            {


            }

            Session["VoltaTemplateSMS"] = 1;
            objetoAntes = item;
            Session["IdTemplate"] = id;
            TemplateSMSViewModel vm = Mapper.Map<TEMPLATE_SMS, TemplateSMSViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarTemplateSMS(TemplateSMSViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TEMPLATE_SMS item = Mapper.Map<TemplateSMSViewModel, TEMPLATE_SMS>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<TEMPLATE_SMS>();
                    Session["ListaTemplateSMS"] = null;
                    return RedirectToAction("MontarTelaTemplateSMS");
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

        public ActionResult VoltarAnexoTemplateSMS()
        {

            return RedirectToAction("EditarTemplateSMS", new { id = (Int32)Session["IdTemplateSMS"] });
        }

        [HttpGet]
        public ActionResult ExcluirTemplateSMS(Int32 id)
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
                    Session["MensTemplateSMS"] = 2;
                    return RedirectToAction("MontarTelaTemplateSMS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TEMPLATE_SMS item = baseApp.GetItemById(id);
            objetoAntes = (TEMPLATE_SMS)Session["TemplateSMS"];
            item.TSMS_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTemplateSMS"] = 4;
                return RedirectToAction("MontarTelaTemplateSMS");
            }
            Session["ListaTemplateSMS"] = null;
            return RedirectToAction("MontarTelaTemplateSMS");
        }

        [HttpGet]
        public ActionResult ReativarTemplateSMS(Int32 id)
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
                    Session["MensTemplateSMS"] = 2;
                    return RedirectToAction("MontarTelaTemplateSMS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TEMPLATE_SMS item = baseApp.GetItemById(id);
            objetoAntes = (TEMPLATE_SMS)Session["TemplateSMS"];
            item.TSMS_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaTemplateSMS"] = null;
            return RedirectToAction("MontarTelaTemplateSMS");
        }

        [HttpGet]
        public ActionResult VerTemplateSMS(Int32 id)
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
                    Session["MensTemplateSMS"] = 2;
                    return RedirectToAction("MontarTelaTemplateSMS", "TemplateSMS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            Session["IdTemplateSMS"] = id;
            TEMPLATE_SMS item = baseApp.GetItemById(id);
            TemplateSMSViewModel vm = Mapper.Map<TEMPLATE_SMS, TemplateSMSViewModel>(item);
            return View(vm);
        }

    }
}