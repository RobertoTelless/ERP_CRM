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
using HtmlAgilityPack;

namespace SMS_Presentation.Controllers
{
    public class AssinanteController : Controller
    {
        private readonly IAssinanteAppService baseApp;
        private readonly IPlanoAppService planApp;
        private readonly ILogAppService logApp;
        private readonly INotificacaoAppService notiApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        ASSINANTE objeto = new ASSINANTE();
        ASSINANTE objetoAntes = new ASSINANTE();
        List<ASSINANTE> listaMaster = new List<ASSINANTE>();
        PLANO objetoPlano = new PLANO();
        PLANO objetoAntesPlano = new PLANO();
        List<PLANO> listaMasterPlano = new List<PLANO>();
        String extensao;

        public AssinanteController(IAssinanteAppService baseApps, ILogAppService logApps, INotificacaoAppService notiApps, IConfiguracaoAppService confApps, IPlanoAppService planApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notiApps;
            confApp = confApps;
            planApps = planApps;
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

        public ActionResult VoltarDash()
        {

            return RedirectToAction("MontarTelaDashboardAssinantes", "Assinante");
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardAssinantes()
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

            // Recupera listas 
            List<ASSINANTE> listaTotal = baseApp.GetAllItens();
            List<ASSINANTE> bloqueados = listaTotal.Where(p => p.ASSI_IN_BLOQUEADO == 1).ToList();
            List<PLANO> planos = planApp.GetAllItens();

            Int32 numAssinantes = listaTotal.Count;
            Int32 numBloqueados = bloqueados.Count;
            ViewBag.NumAssinantes = numAssinantes;
            ViewBag.NumBloqueados = numBloqueados;
            ViewBag.UF = new SelectList((List<UF>)Session["UFs"], "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Planos = planos;
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            Session["TotalAssinantes"] = listaTotal.Count;
            Session["Bloqueados"] = numBloqueados;
            Session["VoltaDash"] = 3;
            return View(vm);
        }

       [HttpGet]
        public ActionResult MontarTelaPlano()
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

            // Carrega listas
            if ((List<PLANO>)Session["ListaPlano"] == null || ((List<PLANO>)Session["ListaPlano"]).Count == 0)
            {
                listaMasterPlano = planApp.GetAllItens();
                Session["ListaPlano"] = listaMasterPlano;
            }
            ViewBag.Listas = (List<PLANO>)Session["ListaPlano"];
            ViewBag.Title = "Planos";

            // Indicadores
            ViewBag.Planos = ((List<PLANO>)Session["ListaPlano"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensPlano"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPlano"] == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0126", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0030", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0080", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensPlano"] = 0;
            Session["VoltaPlano"] = 1;
            objetoPlano = new PLANO();
            if (Session["FiltroPlano"] != null)
            {
                objetoPlano = (PLANO)Session["FiltroPlano"];
            }
            objetoPlano.PLAN_IN_ATIVO = 1;
            return View(objetoPlano);
        }

        public ActionResult RetirarFiltroPlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaPlano"] = null;
            Session["FiltroPlano"] = null;
            return RedirectToAction("MontarTelaPlano");
        }

        public ActionResult MostrarTudoPlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            listaMasterPlano = planApp.GetAllItensAdm();
            Session["FiltroPlano"] = null;
            Session["ListaPlano"] = listaMasterPlano;
            return RedirectToAction("MontarTelaPlano");
        }

        [HttpPost]
        public ActionResult FiltrarPlano(PLANO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                List<PLANO> listaObj = new List<PLANO>();
                Session["FiltroPlano"] = item;
                Int32 volta = planApp.ExecuteFilter(item.PLAN_NM_NOME, item.PLAN_DS_DESCRICAO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("MontarTelaPlano");
                }

                // Sucesso
                listaMasterPlano = listaObj;
                Session["ListaPlano"]  = listaObj;
                return RedirectToAction("MontarTelaPlano");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaPlano");
            }
        }

        public ActionResult VoltarBasePlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPlano");
        }

        [HttpGet]
        public ActionResult IncluirPlano()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPlano"] = 2;
                    return RedirectToAction("MontarTelaPlano");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            ViewBag.Periodicidade = new SelectList(planApp.GetAllPeriodicidades(), "PLPE_CD_ID", "PLPE_NM_NOME");

            // Prepara view
            PLANO item = new PLANO();
            PlanoViewModel vm = Mapper.Map<PLANO, PlanoViewModel>(item);
            vm.PLAN_DT_CRIACAO = DateTime.Today.Date;
            vm.PLAN_DT_VALIDADE = DateTime.Today.Date.AddMonths(12);
            vm.PLAN_IN_ATIVO = 1;
            vm.PLAN_IN_ATENDIMENTOS = 0;
            vm.PLAN_IN_COMPRA = 0;
            vm.PLAN_IN_CRM = 0;
            vm.PLAN_IN_ESTOQUE = 0;
            vm.PLAN_IN_FATURA = 0;
            vm.PLAN_IN_FINANCEIRO = 0;
            vm.PLAN_IN_MENSAGENS = 0;
            vm.PLAN_IN_NIVEL = 0;
            vm.PLAN_IN_OS = 0;
            vm.PLAN_IN_PATRIMONIO = 0;
            vm.PLAN_IN_POSVENDA = 0;
            vm.PLAN_IN_SERVICOS = 0;
            vm.PLAN_IN_VENDAS = 0;
            vm.PLAN_NR_ACOES = 0;
            vm.PLAN_NR_COMPRA = 0;
            vm.PLAN_NR_CONTATOS = 0;
            vm.PLAN_NR_EMAIL = 0;
            vm.PLAN_NR_FORNECEDOR = 0;
            vm.PLAN_NR_PATRIMONIO = 0;
            vm.PLAN_NR_PROCESSOS = 0;
            vm.PLAN_NR_PRODUTO = 0;
            vm.PLAN_NR_SMS = 0;
            vm.PLAN_NR_USUARIOS = 0;
            vm.PLAN_NR_VENDA = 0;
            vm.PLAN_NR_WHATSAPP = 0;
            vm.PLAN_VL_PRECO = 0;
            vm.PLAN_VL_PROMOCAO = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirPlano(PlanoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ViewBag.Periodicidade = new SelectList(planApp.GetAllPeriodicidades(), "PLPE_CD_ID", "PLPE_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PLANO item = Mapper.Map<PlanoViewModel, PLANO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = planApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPlano"] = 3;
                        return RedirectToAction("MontarTelaPlano");
                    }

                    // Sucesso
                    listaMasterPlano = new List<PLANO>();
                    Session["ListaPlano"] = null;

                    // Volta
                    return RedirectToAction("MontarTelaPlano");
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
        public ActionResult EditarPlano(Int32 id)
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
                    Session["MensPlano"] = 2;
                    return RedirectToAction("MontarTelaPlano");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            ViewBag.Periodicidade = new SelectList(planApp.GetAllPeriodicidades(), "PLPE_CD_ID", "PLPE_NM_NOME");
            PLANO item = planApp.GetItemById(id);

            // Mensagens
            if (Session["MensPlano"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPlano"] == 5)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 6)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
            }

            Session["VoltaPlano"] = 1;
            objetoAntesPlano = item;
            Session["Plano"] = item;
            Session["IdPlano"] = id;
            Session["IdVolta"] = id;
            PlanoViewModel vm = Mapper.Map<PLANO, PlanoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarPlano(PlanoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ViewBag.Periodicidade = new SelectList(planApp.GetAllPeriodicidades(), "PLPE_CD_ID", "PLPE_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PLANO item = Mapper.Map<PlanoViewModel, PLANO>(vm);
                    Int32 volta = planApp.ValidateEdit(item, objetoAntesPlano, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterPlano = new List<PLANO>();
                    Session["ListaPlano"] = null;
                    if (Session["FiltroPlano"] != null)
                    {
                        FiltrarPlano((PLANO)Session["FiltroPlano"]);
                    }
                    return RedirectToAction("MontarTelaPlano");
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
        public ActionResult ExcluirPlano(Int32 id)
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
                    Session["MensPlano"] = 2;
                    return RedirectToAction("MontarTelaPlano");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Processa
            PLANO item = planApp.GetItemById(id);
            objetoAntesPlano = (PLANO)Session["Plano"];
            item.PLAN_IN_ATIVO = 0;
            Int32 volta = planApp.ValidateDelete(item, usuario);
            return RedirectToAction("MontarTelaPlano");
        }

        [HttpGet]
        public ActionResult ReativarPlano(Int32 id)
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
                    Session["MensPlano"] = 2;
                    return RedirectToAction("MontarTelaPlano");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Processa
            PLANO item = planApp.GetItemById(id);
            objetoAntesPlano = (PLANO)Session["Plano"];
            item.PLAN_IN_ATIVO = 1;
            Int32 volta = planApp.ValidateDelete(item, usuario);
            return RedirectToAction("MontarTelaPlano");
        }

        public ActionResult VoltarAnexoPlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarPlano", new { id = (Int32)Session["IdPlano"] });
        }


    }
}