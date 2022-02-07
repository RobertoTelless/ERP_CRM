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

namespace ERP_CRM_Solution.Controllers
{
    public class FormaPagamentoController : Controller
    {
        private readonly IFormaPagamentoAppService fpApp;
        private readonly IContaBancariaAppService cbApp;

        private String msg;
        private Exception exception;
        FORMA_PAGAMENTO objetoFP = new FORMA_PAGAMENTO();
        FORMA_PAGAMENTO objetoFPAntes = new FORMA_PAGAMENTO();
        List<FORMA_PAGAMENTO> listaMasterFP = new List<FORMA_PAGAMENTO>();
        String extensao;

        public FormaPagamentoController(IFormaPagamentoAppService fpApps, IContaBancariaAppService cbApps)
        {
            fpApp = fpApps;
            cbApp = cbApps;
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
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaFormaPagamento()
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
            if (Session["ListaForma"] == null)
            {
                listaMasterFP = fpApp.GetAllItens(idAss);
                Session["ListaForma"] = listaMasterFP;
            }

            ViewBag.Listas = (FORMA_PAGAMENTO)Session["ListaForma"];
            ViewBag.Title = "Formas de Pagamento";
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");

            // Indicadores
            ViewBag.Itens = listaMasterFP.Count;

            if (Session["MensFormaPag"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensFormaPag"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFormaPag"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoFP = new FORMA_PAGAMENTO();
            return View(objetoFP);
        }

        public ActionResult MostrarTudoFormaPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFP = fpApp.GetAllItensAdm(idAss);
            Session["ListaForma"] = listaMasterFP;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        public ActionResult VoltarBaseFormaPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["Formas"] = fpApp.GetAllItens(idAss);
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        [HttpGet]
        public ActionResult IncluirFormaPagamento(Int32? id)
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
                    Session["MensFormaPag"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            // Prepara view
            FORMA_PAGAMENTO item = new FORMA_PAGAMENTO();
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE_OLD");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            Session["VoltaPop"] = id.Value;
            FormaPagamentoViewModel vm = Mapper.Map<FORMA_PAGAMENTO, FormaPagamentoViewModel>(item);
            vm.FOPA_IN_ATIVO = 1;
            vm.FOPA_IN_CHEQUE = 0;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirFormaPagamento(FormaPagamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMA_PAGAMENTO item = Mapper.Map<FormaPagamentoViewModel, FORMA_PAGAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fpApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterFP = new List<FORMA_PAGAMENTO>();
                    Session["ListaForma"] = null;
                    return RedirectToAction("MontarTelaFormaPagamento");
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
        public ActionResult EditarFormaPagamento(Int32 id)
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
                    Session["MensFormaPag"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            FORMA_PAGAMENTO item = fpApp.GetItemById(id);
            objetoFPAntes = item;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            Session["Forma"] = item;
            Session["IdVolta"] = id;
            FormaPagamentoViewModel vm = Mapper.Map<FORMA_PAGAMENTO, FormaPagamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFormaPagamento(FormaPagamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FORMA_PAGAMENTO item = Mapper.Map<FormaPagamentoViewModel, FORMA_PAGAMENTO>(vm);
                    Int32 volta = fpApp.ValidateEdit(item, objetoFPAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFP = new List<FORMA_PAGAMENTO>();
                    Session["ListaForma"] = null;
                    return RedirectToAction("MontarTelaFormaPagamento");
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
        public ActionResult ExcluirFormaPagamento(Int32 id)
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
                    Session["MensFormaPag"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            FORMA_PAGAMENTO item = fpApp.GetItemById(id);
            objetoFPAntes = (FORMA_PAGAMENTO)Session["Forma"];
            item.FOPA_IN_ATIVO = 0;
            Int32 volta = fpApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensFormaPag"] = 4;
                return RedirectToAction("MontarTelaFormaPagamento");
            }
            listaMasterFP = new List<FORMA_PAGAMENTO>();
            Session["ListaForma"] = null;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        [HttpGet]
        public ActionResult ReativarFormaPagamento(Int32 id)
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
                    Session["MensFormaPag"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            FORMA_PAGAMENTO item = fpApp.GetItemById(id);
            objetoFPAntes = (FORMA_PAGAMENTO)Session["Forma"];
            item.FOPA_IN_ATIVO = 1;
            Int32 volta = fpApp.ValidateReativar(item, usuario);
            listaMasterFP = new List<FORMA_PAGAMENTO>();
            Session["ListaForma"] = null;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

    }
}