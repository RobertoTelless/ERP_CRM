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
    public class BancoController : Controller
    {
        private readonly IBancoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IContaBancariaAppService contaApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IContaPagarAppService cpApp;
        private readonly IContaReceberAppService crApp;

        private String msg;
        private Exception exception;
        String extensao = String.Empty;
        BANCO objetoBanco = new BANCO();
        BANCO objetoBancoAntes = new BANCO();
        List<BANCO> listaMasterBanco = new List<BANCO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        CONTA_BANCO objConta = new CONTA_BANCO();
        CONTA_BANCO objContaAntes = new CONTA_BANCO();
        List<CONTA_BANCO> listaMasterConta = new List<CONTA_BANCO>();
        CONTA_BANCO contaPadrao = new CONTA_BANCO();

        public BancoController(IBancoAppService baseApps, ILogAppService logApps, IContaBancariaAppService contaApps, IUsuarioAppService usuApps, IContaPagarAppService cpApps, IContaReceberAppService crApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            contaApp = contaApps;
            usuApp = usuApps;
            cpApp = cpApps;
            crApp = crApps; 
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
        public ActionResult MontarTelaBanco()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
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
            if (Session["ListaBanco"] == null)
            {
                listaMasterBanco = baseApp.GetAllItens(idAss);
                Session["ListaBanco"] = listaMasterBanco;
            }
            ViewBag.Listas = ((List<BANCO>)Session["ListaBanco"]).ToList();
            ViewBag.Title = "Bancos";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensBanco"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensBanco"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0039", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoBanco = new BANCO();
            Session["MensBanco"] = 0;
            return View(objetoBanco);
        }

        public ActionResult RetirarFiltroBanco()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaBanco"] = null;
            return RedirectToAction("MontarTelaBanco");
        }

        public ActionResult MostrarTudoBanco()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterBanco = baseApp.GetAllItensAdm(idAss);
            Session["ListaBanco"] = listaMasterBanco;
            return RedirectToAction("MontarTelaBanco");
        }

        [HttpPost]
        public ActionResult FiltrarBanco(BANCO item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<BANCO> listaObj = new List<BANCO>();
                Int32 volta = baseApp.ExecuteFilter(item.BANC_SG_CODIGO, item.BANC_NM_NOME, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensBanco"] = 1;
                }

                // Sucesso
                listaMasterBanco = listaObj;
                Session["ListaBanco"] = listaObj;
                return RedirectToAction("MontarTelaBanco");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaBanco");
            }
        }

        public ActionResult VoltarBaseBanco()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            listaMasterBanco = new List<BANCO>();
            Session["ListaBanco"] = null;
            return RedirectToAction("MontarTelaBanco");
        }

        [HttpGet]
        public ActionResult IncluirBanco()
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
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            BANCO item = new BANCO();
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            vm.BANC_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirBanco(BancoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensBanco"] = 3;
                        return RedirectToAction("MontarTelaBanco", "Banco");
                    }

                    // Sucesso
                    Session["Banco"] = item;
                    Session["IdBanco"] = item.BANC_CD_ID;
                    listaMasterBanco = new List<BANCO>();
                    Session["ListaBanco"] = null;
                    Session["VoltaConta"] = 1;
                    return RedirectToAction("IncluirConta");
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
        public ActionResult EditarBanco(Int32 id)
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
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (Session["MensBanco"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensBanco"] == 6)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 5)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 7)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0063", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            BANCO item = baseApp.GetItemById(id);
            objetoBancoAntes = item;
            Session["IdBanco"] = id;
            Session["Banco"] = item;
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarBanco(BancoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoBancoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensBanco"] = 3;
                        return RedirectToAction("MontarTelaBanco", "Banco");
                    }

                    // Sucesso
                    listaMasterBanco = new List<BANCO>();
                    Session["ListaBanco"] = null;
                    return RedirectToAction("MontarTelaBanco");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
                }
            }
            else
            {
                return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
            }
        }

        [HttpGet]
        public ActionResult ExcluirBanco(Int32 id)
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
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            BANCO item = baseApp.GetItemById(id);
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirBanco(BancoViewModel vm)
        {

            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensBanco"] = 4;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }

                // Sucesso
                listaMasterBanco = new List<BANCO>();
                Session["ListaBanco"] = null;
                return RedirectToAction("MontarTelaBanco");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        [HttpGet]
        public ActionResult ReativarBanco(Int32 id)
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
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            BANCO item = baseApp.GetItemById(id);
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarBanco(BancoViewModel vm)
        {
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterBanco = new List<BANCO>();
                Session["ListaBanco"] = null;
                return RedirectToAction("MontarTelaBanco");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        [HttpGet]
        public ActionResult IncluirConta()
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
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(contaApp.GetAllTipos(idAss).OrderBy(x => x.TICO_NM_NOME).ToList<TIPO_CONTA>(), "TICO_CD_ID", "TICO_NM_NOME");

            // Prepara view
            CONTA_BANCO item = new CONTA_BANCO();
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            vm.BANC_CD_ID = (Int32)Session["IdBanco"];
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.COBA_DT_ABERTURA = DateTime.Today.Date;
            vm.COBA_VL_SALDO_INICIAL = 0;
            vm.COBA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirConta(ContaBancariaViewModel vm)
        {

            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(contaApp.GetAllTipos(idAss).OrderBy(x => x.TICO_NM_NOME).ToList<TIPO_CONTA>(), "TICO_CD_ID", "TICO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = contaApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensBanco"] = 5;
                        return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
                    }
                    if (volta == 2)
                    {
                        Session["MensBanco"] = 7;
                        return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
                    }

                    // Sucesso
                    Session["MensBanco"] = 0;
                    listaMasterConta = new List<CONTA_BANCO>();
                    Session["ListaContaBancaria"] = null;
                    Session["ContasBancarias"] = contaApp.GetAllItens(idAss);
                    return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"]  });
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

        public ActionResult RetirarFiltroLancamento()
        {
            Session["FiltroLancamento"] = null;
            return RedirectToAction("EditarConta", new { id = (Int32)Session["IdConta"] });
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarConta(Int32 id)
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
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(contaApp.GetAllTipos(idAss).OrderBy(x => x.TICO_NM_NOME).ToList<TIPO_CONTA>(), "TICO_CD_ID", "TICO_NM_NOME");
            List<CONTA_BANCO_LANCAMENTO> tipo = new List<CONTA_BANCO_LANCAMENTO>();
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Crédito", CBLA_IN_TIPO = 1 });
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Débito", CBLA_IN_TIPO = 2 });
            ViewBag.TipoLanc = new SelectList(tipo.Select(x => new { x.CBLA_IN_TIPO, x.CBLA_DS_DESCRICAO }).ToList(), "CBLA_IN_TIPO", "CBLA_DS_DESCRICAO");
            ViewBag.TabDadosGer = "active";

            // Prepara view
            CONTA_BANCO item = contaApp.GetItemById(id);
            ViewBag.Lanc = item.CONTA_BANCO_LANCAMENTO.Where(x => x.CBLA_IN_ATIVO == 1).Count();
            if ((Int32)Session["PermFinanceiro"] == 1)
            {
                //ViewBag.Pagar = pagApp.GetAllItens().Where(p => p.COBA_CD_ID == id).ToList().Count;
                //ViewBag.Receber = recApp.GetAllItens().Where(p => p.COBA_CD_ID == id).ToList().Count;
            }
            objContaAntes = item;

            if (Session["FiltroLancamento"] != null)
            {
                ViewBag.TabDadosGer = "";
                ViewBag.TabLanc = "active";

                CONTA_BANCO_LANCAMENTO cbl = (CONTA_BANCO_LANCAMENTO)Session["FiltroLancamento"];
                Session["FiltroLancamento"] = null;
                List<CONTA_BANCO_LANCAMENTO> lstLanc = new List<CONTA_BANCO_LANCAMENTO>();
                Int32 volta = contaApp.ExecuteFilterLanc(cbl.COBA_CD_ID, cbl.CBLA_DT_LANCAMENTO, cbl.CBLA_IN_TIPO, cbl.CBLA_DS_DESCRICAO, out lstLanc);

                if (volta == 0)
                {
                    item.CONTA_BANCO_LANCAMENTO = new List<CONTA_BANCO_LANCAMENTO>();
                    item.CONTA_BANCO_LANCAMENTO = lstLanc;
                }
                else
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
            }

            Session["IdVolta"] = id;
            Session["IdConta"] = id;
            Session["ContaPadrao"] = item;
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            Session["FiltroLancamento"] = null;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarConta(ContaBancariaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(contaApp.GetAllTipos(idAss).OrderBy(x => x.TICO_NM_NOME).ToList<TIPO_CONTA>(), "TICO_CD_ID", "TICO_NM_NOME");
            List<CONTA_BANCO_LANCAMENTO> tipo = new List<CONTA_BANCO_LANCAMENTO>();
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Crédito", CBLA_IN_TIPO = 1 });
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Débito", CBLA_IN_TIPO = 2 });
            ViewBag.TipoLanc = new SelectList(tipo.Select(x => new { x.CBLA_IN_TIPO, x.CBLA_DS_DESCRICAO }).ToList(), "CBLA_IN_TIPO", "CBLA_DS_DESCRICAO");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                    Int32 volta = contaApp.ValidateEdit(item, objContaAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensBanco"] = 7;
                        return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
                    }

                    // Sucesso
                    Session["MensBanco"] = 0;
                    listaMasterConta = new List<CONTA_BANCO>();
                    Session["ListaContaBancaria"] = null;
                    return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
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

        public ActionResult FiltrarLancamento(CONTA_BANCO_LANCAMENTO item)
        {
            Session["FiltroLancamento"] = item;
            return RedirectToAction("EditarConta", new { id = (Int32)Session["IdConta"] });
        }

        [HttpGet]
        public ActionResult ExcluirConta(Int32 id)
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
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CONTA_BANCO item = contaApp.GetItemById(id);
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirConta(ContaBancariaViewModel vm)
        {
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                Int32 volta = contaApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensBanco"] = 6;
                    return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
                }

                // Sucesso
                listaMasterConta = new List<CONTA_BANCO>();
                Session["ListaContaBancaria"] = null;
                return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        [HttpGet]
        public ActionResult ReativarConta(Int32 id)
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
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CONTA_BANCO item = contaApp.GetItemById(id);
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarConta(ContaBancariaViewModel vm)
        {
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                Int32 volta = contaApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterConta = new List<CONTA_BANCO>();
                Session["ListaContaBancaria"] = null;
                return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        public ActionResult VoltarBaseConta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            if ((Int32)Session["PermFinanceiro"] == 1)
            {
                if (Session["voltaLiquidacao"] != null && (Int32)Session["voltaLiquidacao"] == 1)
                {
                    return RedirectToAction("VerCP", "ContaPagar", new { id = (Int32)Session["idContaPagar"], liquidar = 1 });
                }
            }
            listaMasterConta = new List<CONTA_BANCO>();
            Session["ListaContaBancaria"] = null;
            return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            // Prepara view
            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            ContaBancariaContatoViewModel vm = Mapper.Map<CONTA_BANCO_CONTATO, ContaBancariaContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(ContaBancariaContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_BANCO_CONTATO item = Mapper.Map<ContaBancariaContatoViewModel, CONTA_BANCO_CONTATO>(vm);
                    Int32 volta = contaApp.ValidateEditContato(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoConta");
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
        public ActionResult ExcluirContato(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            item.CBCT_IN_ATIVO = 0;
            Int32 volta = contaApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            item.CBCT_IN_ATIVO = 1;
            Int32 volta = contaApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        [HttpGet]
        public ActionResult IncluirContato()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CONTA_BANCO_CONTATO item = new CONTA_BANCO_CONTATO();
            ContaBancariaContatoViewModel vm = Mapper.Map<CONTA_BANCO_CONTATO, ContaBancariaContatoViewModel>(item);
            vm.COBA_CD_ID = (Int32)Session["IdConta"];
            vm.CBCT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(ContaBancariaContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCO_CONTATO item = Mapper.Map<ContaBancariaContatoViewModel, CONTA_BANCO_CONTATO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = contaApp.ValidateCreateContato(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoConta");
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
        public ActionResult IncluirLancamento()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");

            CONTA_BANCO_LANCAMENTO item = new CONTA_BANCO_LANCAMENTO();
            ContaBancariaLancamentoViewModel vm = Mapper.Map<CONTA_BANCO_LANCAMENTO, ContaBancariaLancamentoViewModel>(item);
            vm.COBA_CD_ID = (Int32)Session["IdVolta"];
            vm.CBLA_IN_ATIVO = 1;
            vm.CBLA_IN_ORIGEM = 1;
            vm.CBLA_DT_LANCAMENTO = DateTime.Today.Date;
            vm.CONTA_BANCO = (CONTA_BANCO)Session["ContaPadrao"];
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirLancamento(ContaBancariaLancamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCO_LANCAMENTO item = Mapper.Map<ContaBancariaLancamentoViewModel, CONTA_BANCO_LANCAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_BANCO conta = (CONTA_BANCO)Session["ContaPadrao"];
                    Int32 volta = contaApp.ValidateCreateLancamento(item, conta);
                    Int32 volta1 = AcertaSaldo(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoConta");
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
        public ActionResult EditarLancamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");
            CONTA_BANCO_LANCAMENTO item = contaApp.GetLancamentoById(id);
            ContaBancariaLancamentoViewModel vm = Mapper.Map<CONTA_BANCO_LANCAMENTO, ContaBancariaLancamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarLancamento(ContaBancariaLancamentoViewModel vm)
        {

            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_BANCO_LANCAMENTO item = Mapper.Map<ContaBancariaLancamentoViewModel, CONTA_BANCO_LANCAMENTO>(vm);
                    Int32 volta = contaApp.ValidateEditLancamento(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoConta");
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
        public ActionResult ExcluirLancamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_BANCO_LANCAMENTO item = contaApp.GetLancamentoById(id);
            item.CBLA_IN_ATIVO = 0;
            if (item.CBLA_IN_TIPO == 1)
            {
                item.CONTA_BANCO.COBA_VL_SALDO_ATUAL = item.CONTA_BANCO.COBA_VL_SALDO_ATUAL - item.CBLA_VL_VALOR.Value;
            }
            else
            {
                item.CONTA_BANCO.COBA_VL_SALDO_ATUAL = item.CONTA_BANCO.COBA_VL_SALDO_ATUAL + item.CBLA_VL_VALOR.Value;
            }

            Int32 volta = contaApp.ValidateEditLancamento(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        [HttpGet]
        public ActionResult ReativarLancamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_BANCO_LANCAMENTO item = contaApp.GetLancamentoById(id);
            item.CBLA_IN_ATIVO = 1;
            if (item.CBLA_IN_TIPO == 1)
            {
                item.CONTA_BANCO.COBA_VL_SALDO_ATUAL = item.CONTA_BANCO.COBA_VL_SALDO_ATUAL + item.CBLA_VL_VALOR.Value;
            }
            else
            {
                item.CONTA_BANCO.COBA_VL_SALDO_ATUAL = item.CONTA_BANCO.COBA_VL_SALDO_ATUAL - item.CBLA_VL_VALOR.Value;
            }

            Int32 volta = contaApp.ValidateEditLancamento(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        public ActionResult VoltarAnexoConta()
        {

            return RedirectToAction("EditarConta", new { id = (Int32)Session["IdConta"] });
        }

        public ActionResult VoltarAnexoBanco()
        {

            return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
        }

        public Int32 AcertaSaldo(CONTA_BANCO_LANCAMENTO item)
        {
            try
            {
                // Acerta saldo
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONTA_BANCO_LANCAMENTO lanc = contaApp.GetLancamentoById(item.CBLA_CD_ID);
                if (item.CBLA_IN_TIPO == 1)
                {
                    lanc.CONTA_BANCO.COBA_VL_SALDO_ATUAL = lanc.CONTA_BANCO.COBA_VL_SALDO_ATUAL + item.CBLA_VL_VALOR.Value;
                }
                else
                {
                    lanc.CONTA_BANCO.COBA_VL_SALDO_ATUAL = lanc.CONTA_BANCO.COBA_VL_SALDO_ATUAL - item.CBLA_VL_VALOR.Value;
                }
                Int32 volta = contaApp.ValidateEditLancamento(lanc);
                return volta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult MontarTelaResumoFinanceiro()
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera CP
            List<CONTA_PAGAR> pag = cpApp.GetAllItens(idAss);

            Decimal pago = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_PAGAR_PARCELA == null).Sum(p => p.CAPA_VL_VALOR_PAGO).Value;
            pago += (Decimal)pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_QUITACAO.Value.Month == DateTime.Now.Month & x.CPPA_DT_QUITACAO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 1).Sum(p => p.CPPA_VL_VALOR);
            ViewBag.Pago = pago;

            Decimal sumPagar = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Sum(p => p.CAPA_VL_VALOR).Value;
            sumPagar += (Decimal)pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CPPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 0).Sum(p => p.CPPA_VL_VALOR);
            ViewBag.APagar = sumPagar;

            Decimal sumAtrasoCP = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_NR_ATRASO > 0 & p.CAPA_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Sum(p => p.CAPA_VL_VALOR).Value;
            sumAtrasoCP += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_NR_ATRASO > 0 & x.CPPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).Sum(p => p.CPPA_VL_VALOR).Value;
            ViewBag.Atraso = sumAtrasoCP;

            Int32 pagos = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_PAGAR_PARCELA == null).ToList().Count;
            pagos += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_QUITACAO.Value.Month == DateTime.Now.Month & x.CPPA_DT_QUITACAO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 1).ToList().Count;

            Int32 atrasos = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_NR_ATRASO > 0 & p.CAPA_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Count();
            atrasos += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_NR_ATRASO > 0 & x.CPPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).ToList().Count;

            Int32 pendentes = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).ToList().Count;
            pendentes += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CPPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 0).ToList().Count;

            Session["TotalCP"] = pag.Count;
            Session["APagarMes"] = pendentes;
            Session["Atraso"] = atrasos;
            Session["PagoMes"] = pagos;

            // Resumo Mes Pagamentos
            List<DateTime> datasCP = pag.Where(m => m.CAPA_IN_ATIVO == 1 & m.CAPA_IN_LIQUIDADA == 1 & (m.CONTA_PAGAR_PARCELA == null || m.CONTA_PAGAR_PARCELA.Count == 0)).Select(p => p.CAPA_DT_LIQUIDACAO.Value.Date).Distinct().ToList();
            List<DateTime> datasParc = pag.Where(m => m.CAPA_IN_ATIVO == 1 & m.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_IN_QUITADA == 1).Select(p => p.CPPA_DT_QUITACAO.Value.Date).Distinct().ToList();
            List<DateTime> datas = datasCP.Concat(datasParc).Distinct().ToList();

            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            List<CONTA_PAGAR> lista5 = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1).ToList();
            foreach (DateTime item in datasCP)
            {
                List<CONTA_PAGAR> lista10 = lista5.Where(p => p.CAPA_DT_LIQUIDACAO.Value.Date == item.Date).ToList();
                Decimal conta = lista10.Sum(p => p.CAPA_VL_VALOR_PAGO).Value;

                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.ValorDec = conta;
                lista.Add(mod1);
            }
            ViewBag.ListaPagDia = lista;
            ViewBag.ContaPagDia = lista.Count;
            Session["ListaDatas"] = datasCP;
            Session["ListaPagResumo"] = lista;

            List<ModeloViewModel> listaX = new List<ModeloViewModel>();
            List<CONTA_PAGAR_PARCELA> lista6 = pag.Where(p => p.CAPA_IN_ATIVO == 1).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_IN_QUITADA == 1).ToList();
            foreach (DateTime item in datasParc)
            {
                List<CONTA_PAGAR_PARCELA> lista10 = lista6.Where(p => p.CPPA_DT_QUITACAO.Value.Date == item.Date).ToList();
                Decimal conta = lista10.Sum(p => p.CPPA_VL_VALOR_PAGO).Value;

                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.ValorDec = conta;
                listaX.Add(mod1);
            }
            ViewBag.ListaPagDiaParc = listaX;
            ViewBag.ContaPagDiaParg = listaX.Count;
            Session["ListaDatasParc"] = datasParc;
            Session["ListaPagResumoParc"] = listaX;

            // Resumo CP Situacao  
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            ModeloViewModel mod = new ModeloViewModel();
            mod.Data = "Liquidados";
            mod.Valor = pagos;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Em Atraso";
            mod.Valor = atrasos;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Pendentes";
            mod.Valor = pendentes;
            lista1.Add(mod);
            ViewBag.ListaCPSituacao = lista1;
            Session["ListaCPSituacao"] = lista1;
            Session["VoltaDash"] = 3;

            // Recupera CR
            List<CONTA_RECEBER> rec = crApp.GetAllItens(idAss);

            Decimal aReceberDia = (Decimal)rec.Where(x => x.CARE_IN_ATIVO == 1 & x.CARE_IN_LIQUIDADA == 0 & x.CARE_DT_VENCIMENTO.Value.Date == DateTime.Now.Date & (x.CONTA_RECEBER_PARCELA == null || x.CONTA_RECEBER_PARCELA.Count == 0)).Sum(x => x.CARE_VL_SALDO);
            aReceberDia += (Decimal)rec.Where(x => x.CARE_IN_ATIVO == 1 & x.CARE_IN_LIQUIDADA == 0 & x.CARE_DT_VENCIMENTO.Value.Day == DateTime.Now.Day & x.CONTA_RECEBER_PARCELA != null).SelectMany(x => x.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Date == DateTime.Now.Date & x.CRPA_IN_QUITADA == 0).Sum(x => x.CRPA_VL_VALOR);
            ViewBag.CRS = aReceberDia;

            Decimal recebido = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year).Sum(p => p.CARE_VL_VALOR_LIQUIDADO).Value;
            recebido += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 1).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.Recebido = recebido;

            Decimal sumReceber = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 0 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumReceber += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 0 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 0).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.AReceber = sumReceber;

            Decimal sumAtraso = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_NR_ATRASO > 0 & p.CARE_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumAtraso += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_NR_ATRASO > 0 & x.CRPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.AtrasoCR = sumAtraso;

            Int32 recebidas = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
            recebidas += rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 1).ToList().Count;

            Int32 atrasosCR = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_NR_ATRASO > 0 & p.CARE_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Count();
            atrasosCR += rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_NR_ATRASO > 0 & x.CRPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).ToList().Count;

            Int32 pendentesCR = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 0 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).ToList().Count;
            pendentesCR += rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 0 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 0).ToList().Count;

            Session["TotalCR"] = rec.Count;
            Session["Recebido"] = recebidas;
            Session["AReceber"] = pendentesCR;
            Session["AtrasoCR"] = atrasosCR;

            // Resumo Mes Recebimentos
            List<DateTime> datasCR = rec.Where(m => m.CARE_DT_DATA_LIQUIDACAO != null).Select(p => p.CARE_DT_DATA_LIQUIDACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> listaCR = new List<ModeloViewModel>();
            foreach (DateTime item in datasCR)
            {
                CONTA_RECEBER cr = rec.Where(p => p.CARE_DT_DATA_LIQUIDACAO == item).FirstOrDefault();
                Decimal? conta = rec.Where(p => p.CARE_DT_DATA_LIQUIDACAO == item).Sum(p => p.CARE_VL_VALOR_RECEBIDO);
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.ValorDec = conta.Value;
                listaCR.Add(mod1);
            }
            ViewBag.ListaRecDia = listaCR;
            ViewBag.ContaRecDia = listaCR.Count;
            Session["ListaDatasCR"] = datasCR;
            Session["ListaRecResumo"] = listaCR;

            // Resumo CR Situacao  
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            ModeloViewModel mod2 = new ModeloViewModel();
            mod2.Data = "Recebidas";
            mod2.Valor = recebidas;
            lista2.Add(mod2);
            mod2 = new ModeloViewModel();
            mod2.Data = "Em Atraso";
            mod2.Valor = atrasosCR;
            lista2.Add(mod2);
            mod2 = new ModeloViewModel();
            mod2.Data = "Pendentes";
            mod2.Valor = pendentesCR;
            lista2.Add(mod2);
            ViewBag.ListaCRSituacao = lista2;
            Session["ListaCRSituacao"] = lista2;
            Session["VoltaDash"] = 3;
            Session["ListaForma"] = null;

            // Recupera Contas
            List<CONTA_BANCO> contas = contaApp.GetAllItens(idAss);
            List<ModeloViewModel> listaContas = new List<ModeloViewModel>();
            Decimal saldoTotal = 0;
            foreach (CONTA_BANCO item in contas)
            {
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.Nome = item.COBA_NM_NOME;
                mod1.Data = item.BANCO.BANC_NM_NOME + " - " + item.COBA_NR_AGENCIA + "/" + item.COBA_NR_CONTA;
                mod1.ValorDec = item.COBA_VL_SALDO_ATUAL.Value;
                listaContas.Add(mod1);
                saldoTotal += item.COBA_VL_SALDO_ATUAL.Value;
            }
            ViewBag.ListaContas = listaContas;
            ViewBag.SaldoTotal = saldoTotal;
            Session["ListaContas"] = listaContas;
            Session["SaldoTotal"] = saldoTotal;

            // Caixa
            Decimal caixa = saldoTotal + sumReceber - sumPagar;
            ViewBag.Caixa = caixa;

            // Abre view
            return View(vm);
        }
    }
}