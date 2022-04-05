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
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly ISubcategoriaProdutoAppService spApp;
        private readonly ITamanhoAppService tamApp;
        private readonly IUnidadeAppService unApp;
        private readonly IGrupoCCAppService grApp;
        private readonly ISubgrupoAppService sgApp;
        private readonly ICategoriaAtendimentoAppService catApp;
        private readonly IDepartamentoAppService depApp;
        private readonly ICategoriaServicoAppService serApp;
        private readonly ICategoriaOrdemServicoAppService osApp;

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
        CATEGORIA_PRODUTO objetoCP = new CATEGORIA_PRODUTO();
        CATEGORIA_PRODUTO objetoCPAntes = new CATEGORIA_PRODUTO();
        List<CATEGORIA_PRODUTO> listaMasterCP = new List<CATEGORIA_PRODUTO>();
        SUBCATEGORIA_PRODUTO objetoSP = new SUBCATEGORIA_PRODUTO();
        SUBCATEGORIA_PRODUTO objetoSPAntes = new SUBCATEGORIA_PRODUTO();
        List<SUBCATEGORIA_PRODUTO> listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
        TAMANHO objetoTAM = new TAMANHO();
        TAMANHO objetoTAMAntes = new TAMANHO();
        List<TAMANHO> listaMasterTAM = new List<TAMANHO>();
        UNIDADE objetoUN = new UNIDADE();
        UNIDADE objetoUNAntes = new UNIDADE();
        List<UNIDADE> listaMasterUN = new List<UNIDADE>();
        GRUPO_CC objetoGR = new GRUPO_CC();
        GRUPO_CC objetoGRAntes = new GRUPO_CC();
        List<GRUPO_CC> listaMasterGR = new List<GRUPO_CC>();
        SUBGRUPO objetoSG = new SUBGRUPO();
        SUBGRUPO objetoSGAntes = new SUBGRUPO();
        List<SUBGRUPO> listaMasterSG = new List<SUBGRUPO>();
        String extensao;
        CATEGORIA_ATENDIMENTO objetoCAT = new CATEGORIA_ATENDIMENTO();
        CATEGORIA_ATENDIMENTO objetoCATAntes = new CATEGORIA_ATENDIMENTO();
        List<CATEGORIA_ATENDIMENTO> listaMasterCAT = new List<CATEGORIA_ATENDIMENTO>();
        DEPARTAMENTO objetoDEP = new DEPARTAMENTO();
        DEPARTAMENTO objetoDEPAntes = new DEPARTAMENTO();
        List<DEPARTAMENTO> listaMasterDEP = new List<DEPARTAMENTO>();
        CATEGORIA_SERVICO objetoSER = new CATEGORIA_SERVICO();
        CATEGORIA_SERVICO objetoSERAntes = new CATEGORIA_SERVICO();
        List<CATEGORIA_SERVICO> listaMasterSER = new List<CATEGORIA_SERVICO>();
        CATEGORIA_ORDEM_SERVICO objetoOS = new CATEGORIA_ORDEM_SERVICO();
        CATEGORIA_ORDEM_SERVICO objetoOSAntes = new CATEGORIA_ORDEM_SERVICO();
        List<CATEGORIA_ORDEM_SERVICO> listaMasterOS = new List<CATEGORIA_ORDEM_SERVICO>();

        public TabelasAuxiliaresController(ICategoriaClienteAppService ccApps, ICargoAppService caApps, ICRMOrigemAppService orApps, IMotivoCancelamentoAppService mcApps, IMotivoEncerramentoAppService meApps, ITipoAcaoAppService taApps, ICategoriaFornecedorAppService cfApps, ICategoriaEquipamentoAppService ceApps, ICategoriaProdutoAppService cpApps, ISubcategoriaProdutoAppService spApps, ITamanhoAppService tamApps, IUnidadeAppService unApps, IGrupoCCAppService grApps, ISubgrupoAppService sgApps, ICategoriaAtendimentoAppService catApps, IDepartamentoAppService depApps, ICategoriaOrdemServicoAppService osApps, ICategoriaServicoAppService serApps)
        {
            ccApp = ccApps;
            caApp = caApps;
            orApp = orApps;
            mcApp = mcApps;
            meApp = meApps;
            taApp = taApps;
            ceApp = ceApps;
            cfApp = cfApps;
            cpApp = cpApps;
            spApp = spApps;
            tamApp = tamApps;
            unApp = unApps;
            grApp = grApps;
            sgApp = sgApps;
            catApp = catApps;
            depApp = depApps;
            osApp = osApps;
            serApp = serApps;
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
            if ((Int32)Session["VoltaCatFornecedor"] == 2)
            {
                return RedirectToAction("IncluirFornecedor", "Fornecedor");
            }
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
                    if ((Int32)Session["VoltaCatFornecedor"] == 2)
                    {
                        return RedirectToAction("IncluirFornecedor", "Fornecedor");
                    }
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

        [HttpGet]
        public ActionResult MontarTelaCatProduto()
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
            if ((List<CATEGORIA_PRODUTO>)Session["ListaCatProduto"] == null)
            {
                listaMasterCP = cpApp.GetAllItens(idAss);
                Session["ListaCatProduto"] = listaMasterCP;
            }
            ViewBag.Listas = (List<CATEGORIA_PRODUTO>)Session["ListaCatProduto"];
            Session["CatProduto"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCatProduto"] != null)
            {
                if ((Int32)Session["MensCatProduto"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatProduto"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0088", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatProduto"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0069", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatProduto"] = 1;
            objetoCP = new CATEGORIA_PRODUTO();
            return View(objetoCP);
        }

        public ActionResult RetirarFiltroCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCatProduto"] = null;
            return RedirectToAction("MontarTelaCatProduto");
        }

        public ActionResult MostrarTudoCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCP = cpApp.GetAllItensAdm(idAss);
            Session["ListaCatProduto"] = listaMasterCP;
            return RedirectToAction("MontarTelaCatProduto");
        }

        public ActionResult VoltarBaseCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        [HttpGet]
        public ActionResult IncluirCatProduto()
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
                    Session["MensCatProduto"] = 2;
                    return RedirectToAction("MontarTelaCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            List<SelectListItem> exp = new List<SelectListItem>();
            exp.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            exp.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Exp = new SelectList(exp, "Value", "Text");
            List<SelectListItem> food = new List<SelectListItem>();
            food.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            food.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Food = new SelectList(food, "Value", "Text");
            List<SelectListItem> grade = new List<SelectListItem>();
            grade.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            grade.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Grade = new SelectList(grade, "Value", "Text");
            List<SelectListItem> seo = new List<SelectListItem>();
            seo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            seo.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.SEO = new SelectList(seo, "Value", "Text");
            List<SelectListItem> tam = new List<SelectListItem>();
            tam.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            tam.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.TAM = new SelectList(tam, "Value", "Text");

            // Prepara view
            CATEGORIA_PRODUTO item = new CATEGORIA_PRODUTO();
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CAPR_IN_ATIVO = 1;
            vm.CAPR_IN_EXPEDICAO = 0;
            vm.CAPR_IN_FOOD = 0;
            vm.CAPR_IN_GRADE = 0;
            vm.CAPR_IN_SEO = 0;
            vm.CAPR_IN_TAMANHO = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCatProduto(CategoriaProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> exp = new List<SelectListItem>();
            exp.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            exp.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Exp = new SelectList(exp, "Value", "Text");
            List<SelectListItem> food = new List<SelectListItem>();
            food.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            food.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Food = new SelectList(food, "Value", "Text");
            List<SelectListItem> grade = new List<SelectListItem>();
            grade.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            grade.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Grade = new SelectList(grade, "Value", "Text");
            List<SelectListItem> seo = new List<SelectListItem>();
            seo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            seo.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.SEO = new SelectList(seo, "Value", "Text");
            List<SelectListItem> tam = new List<SelectListItem>();
            tam.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            tam.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.TAM = new SelectList(tam, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cpApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatProduto"] = 3;
                        return RedirectToAction("MontarTelaCatProduto");
                    }

                    // Sucesso
                    listaMasterCP = new List<CATEGORIA_PRODUTO>();
                    Session["ListaCatProduto"] = null;
                    Session["IdCatProduto"] = item.CAPR_CD_ID;
                    if ((Int32)Session["VoltaCatProduto"] == 2)
                    {
                        return RedirectToAction("IncluirProduto", "Produto");
                    }
                    return RedirectToAction("MontarTelaCatProduto");
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
        public ActionResult EditarCatProduto(Int32 id)
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
                    Session["MensCatProduto"] = 2;
                    return RedirectToAction("MontarTelaCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            Session["CatProduto"] = item;

            // Indicadores
            List<SelectListItem> exp = new List<SelectListItem>();
            exp.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            exp.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Exp = new SelectList(exp, "Value", "Text");
            List<SelectListItem> food = new List<SelectListItem>();
            food.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            food.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Food = new SelectList(food, "Value", "Text");
            List<SelectListItem> grade = new List<SelectListItem>();
            grade.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            grade.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Grade = new SelectList(grade, "Value", "Text");
            List<SelectListItem> seo = new List<SelectListItem>();
            seo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            seo.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.SEO = new SelectList(seo, "Value", "Text");
            List<SelectListItem> tam = new List<SelectListItem>();
            tam.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            tam.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.TAM = new SelectList(tam, "Value", "Text");
            
            // Mensagens
            if (Session["MensCatProduto"] != null)
            {


            }

            objetoCPAntes = item;
            Session["IdCatProduto"] = id;
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatProduto(CategoriaProdutoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> exp = new List<SelectListItem>();
            exp.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            exp.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Exp = new SelectList(exp, "Value", "Text");
            List<SelectListItem> food = new List<SelectListItem>();
            food.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            food.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Food = new SelectList(food, "Value", "Text");
            List<SelectListItem> grade = new List<SelectListItem>();
            grade.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            grade.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Grade = new SelectList(grade, "Value", "Text");
            List<SelectListItem> seo = new List<SelectListItem>();
            seo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            seo.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.SEO = new SelectList(seo, "Value", "Text");
            List<SelectListItem> tam = new List<SelectListItem>();
            tam.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            tam.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.TAM = new SelectList(tam, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    Int32 volta = cpApp.ValidateEdit(item, objetoCPAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCP = new List<CATEGORIA_PRODUTO>();
                    Session["ListaCatProduto"] = null;
                    return RedirectToAction("MontarTelaCatProduto");
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

        public ActionResult VoltarAnexoCatProduto()
        {

            return RedirectToAction("EditarCatProduto", new { id = (Int32)Session["IdCatProduto"] });
        }

        [HttpGet]
        public ActionResult ExcluirCatProduto(Int32 id)
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
                    Session["MensCatProduto"] = 2;
                    return RedirectToAction("MontarTelaCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            objetoCPAntes = (CATEGORIA_PRODUTO)Session["CatProduto"];
            item.CAPR_IN_ATIVO = 0;
            Int32 volta = cpApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatProduto"] = 4;
                return RedirectToAction("MontarTelaCatProduto");
            }
            Session["ListaCatProduto"] = null;
            return RedirectToAction("MontarTelaCatProduto");
        }

        [HttpGet]
        public ActionResult ReativarCatProduto(Int32 id)
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
                    Session["MensCatProduto"] = 2;
                    return RedirectToAction("MontarTelaCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            objetoCPAntes = (CATEGORIA_PRODUTO)Session["CatProduto"];
            item.CAPR_IN_ATIVO = 1;
            Int32 volta = cpApp.ValidateReativar(item, usuario);
            Session["ListaCatProduto"] = null;
            return RedirectToAction("MontarTelaCatProduto");
        }

        [HttpGet]
        public ActionResult MontarTelaSubCatProduto()
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
            if ((List<SUBCATEGORIA_PRODUTO>)Session["ListaSubCatProduto"] == null)
            {
                listaMasterSP = spApp.GetAllItens(idAss);
                Session["ListaSubCatProduto"] = listaMasterSP;
            }
            ViewBag.Listas = (List<SUBCATEGORIA_PRODUTO>)Session["ListaSubCatProduto"];
            ViewBag.Cats = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");
            Session["SubCatProduto"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensSubCatProduto"] != null)
            {
                if ((Int32)Session["MensSubCatProduto"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubCatProduto"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0089", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubCatProduto"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0090", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaSubCatProduto"] = 1;
            objetoSP = new SUBCATEGORIA_PRODUTO();
            return View(objetoSP);
        }

        public ActionResult RetirarFiltroSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaSubCatProduto"] = null;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        public ActionResult MostrarTudoSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSP = spApp.GetAllItensAdm(idAss);
            Session["ListaSubCatProduto"] = listaMasterSP;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        public ActionResult VoltarBaseSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        [HttpGet]
        public ActionResult IncluirSubCatProduto()
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
                    Session["MensSubCatProduto"] = 2;
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");
            
            // Prepara view
            SUBCATEGORIA_PRODUTO item = new SUBCATEGORIA_PRODUTO();
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.SCPR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirSubCatProduto(SubCategoriaProdutoViewModel vm)
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
                    SUBCATEGORIA_PRODUTO item = Mapper.Map<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = spApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensSubCatProduto"] = 3;
                        return RedirectToAction("MontarTelaSubCatProduto");
                    }

                    // Sucesso
                    listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
                    Session["ListaSubCatProduto"] = null;
                    Session["IdSubCatProduto"] = item.CAPR_CD_ID;
                    if ((Int32)Session["VoltaSubCatProduto"] == 2)
                    {
                        return RedirectToAction("IncluirProduto", "Produto");
                    }
                    return RedirectToAction("MontarTelaSubCatProduto");
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
        public ActionResult EditarSubCatProduto(Int32 id)
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
                    Session["MensSubCatProduto"] = 2;
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            Session["SubCatProduto"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensSubCatProduto"] != null)
            {


            }

            objetoSPAntes = item;
            Session["IdSubCatProduto"] = id;
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarSubCatProduto(SubCategoriaProdutoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    SUBCATEGORIA_PRODUTO item = Mapper.Map<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>(vm);
                    Int32 volta = spApp.ValidateEdit(item, objetoSPAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
                    Session["ListaSubCatProduto"] = null;
                    return RedirectToAction("MontarTelaSubCatProduto");
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

        public ActionResult VoltarAnexoSubCatProduto()
        {

            return RedirectToAction("EditarSubCatProduto", new { id = (Int32)Session["IdSubCatProduto"] });
        }

        [HttpGet]
        public ActionResult ExcluirSubCatProduto(Int32 id)
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
                    Session["MensSubCatProduto"] = 2;
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            objetoSPAntes = (SUBCATEGORIA_PRODUTO)Session["SubCatProduto"];
            item.SCPR_IN_ATIVO = 0;
            Int32 volta = spApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensSubCatProduto"] = 4;
                return RedirectToAction("MontarTelaSubCatProduto");
            }
            Session["ListaSubCatProduto"] = null;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        [HttpGet]
        public ActionResult ReativarSubCatProduto(Int32 id)
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
                    Session["MensSubCatProduto"] = 2;
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            objetoSPAntes = (SUBCATEGORIA_PRODUTO)Session["SubCatProduto"];
            item.SCPR_IN_ATIVO = 1;
            Int32 volta = spApp.ValidateReativar(item, usuario);
            Session["ListaSubCatProduto"] = null;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        [HttpGet]
        public ActionResult MontarTelaTamanho()
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
            if ((List<TAMANHO>)Session["ListaTamanho"] == null)
            {
                listaMasterTAM = tamApp.GetAllItens(idAss);
                Session["ListaTamanho"] = listaMasterTAM;
            }
            ViewBag.Listas = (List<TAMANHO>)Session["ListaTamanho"];
            Session["SubCatProduto"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensTamanho"] != null)
            {
                if ((Int32)Session["MensTamanho"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTamanho"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0091", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTamanho"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0092", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTamanho"] = 1;
            objetoTAM = new TAMANHO();
            return View(objetoTAM);
        }

        public ActionResult RetirarFiltroTamanho()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaTamanho"] = null;
            return RedirectToAction("MontarTelaTamanho");
        }

        public ActionResult MostrarTudoTamanho()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTAM = tamApp.GetAllItensAdm(idAss);
            Session["ListaTamanho"] = listaMasterTAM;
            return RedirectToAction("MontarTelaTamanho");
        }

        public ActionResult VoltarBaseTamanho()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTamanho");
        }

        [HttpGet]
        public ActionResult IncluirTamanho()
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
                    Session["MensTamanho"] = 2;
                    return RedirectToAction("MontarTelaTamanho");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            
            // Prepara view
            TAMANHO item = new TAMANHO();
            TamanhoViewModel vm = Mapper.Map<TAMANHO, TamanhoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.TAMA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirTamanho(TamanhoViewModel vm)
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
                    TAMANHO item = Mapper.Map<TamanhoViewModel, TAMANHO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = tamApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTamanho"] = 3;
                        return RedirectToAction("MontarTelaTamanho");
                    }

                    // Sucesso
                    listaMasterTAM = new List<TAMANHO>();
                    Session["ListaTamanho"] = null;
                    Session["IdTamanho"] = item.TAMA_CD_ID;
                    return RedirectToAction("MontarTelaTamanho");
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
        public ActionResult EditarTamanho(Int32 id)
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
                    Session["MensTamanho"] = 2;
                    return RedirectToAction("MontarTelaTamanho");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TAMANHO item = tamApp.GetItemById(id);
            Session["Tamanho"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensTamanho"] != null)
            {


            }

            objetoTAMAntes = item;
            Session["IdTamanho"] = id;
            TamanhoViewModel vm = Mapper.Map<TAMANHO, TamanhoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTamanho(TamanhoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TAMANHO item = Mapper.Map<TamanhoViewModel, TAMANHO>(vm);
                    Int32 volta = tamApp.ValidateEdit(item, objetoTAMAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTAM = new List<TAMANHO>();
                    Session["ListaTamanho"] = null;
                    return RedirectToAction("MontarTelaTamanho");
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

        public ActionResult VoltarAnexoTamanho()
        {

            return RedirectToAction("EditarTamanho", new { id = (Int32)Session["IdTamanho"] });
        }

        [HttpGet]
        public ActionResult ExcluirTamanho(Int32 id)
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
                    Session["MensTamanho"] = 2;
                    return RedirectToAction("MontarTamanho");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TAMANHO item = tamApp.GetItemById(id);
            objetoTAMAntes = (TAMANHO)Session["Tamanho"];
            item.TAMA_IN_ATIVO = 0;
            Int32 volta = tamApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTamanho"] = 4;
                return RedirectToAction("MontarTelaTamanho");
            }
            Session["ListaTamanho"] = null;
            return RedirectToAction("MontarTelaTamanho");
        }

        [HttpGet]
        public ActionResult ReativarTamanho(Int32 id)
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
                    Session["MensTamanho"] = 2;
                    return RedirectToAction("MontarTelaTamanho");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            TAMANHO item = tamApp.GetItemById(id);
            objetoTAMAntes = (TAMANHO)Session["Tamanho"];
            item.TAMA_IN_ATIVO = 1;
            Int32 volta = tamApp.ValidateReativar(item, usuario);
            Session["ListaTamanho"] = null;
            return RedirectToAction("MontarTelaTamanho");
        }

        [HttpGet]
        public ActionResult MontarTelaUnidade()
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
            if ((List<UNIDADE>)Session["ListaUnidade"] == null)
            {
                listaMasterUN = unApp.GetAllItens(idAss);
                Session["ListaUnidade"] = listaMasterUN;
            }
            ViewBag.Listas = (List<UNIDADE>)Session["ListaUnidade"];
            Session["Unidade"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensUnidade"] != null)
            {
                if ((Int32)Session["MensUnidade"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUnidade"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0093", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUnidade"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0094", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaUnidade"] = 1;
            objetoUN = new UNIDADE();
            return View(objetoUN);
        }

        public ActionResult RetirarFiltroUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaUnidade"] = null;
            return RedirectToAction("MontarTelaUnidade");
        }

        public ActionResult MostrarTudoUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterUN = unApp.GetAllItensAdm(idAss);
            Session["ListaUnidade"] = listaMasterUN;
            return RedirectToAction("MontarTelaUnidade");
        }

        public ActionResult VoltarBaseUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpGet]
        public ActionResult IncluirUnidade()
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
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Medida", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Serviço", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Prepara view
            UNIDADE item = new UNIDADE();
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.UNID_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirUnidade(UnidadeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Medida", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Serviço", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    UNIDADE item = Mapper.Map<UnidadeViewModel, UNIDADE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = unApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUnidade"] = 3;
                        return RedirectToAction("MontarTelaUnidade");
                    }

                    // Sucesso
                    listaMasterUN = new List<UNIDADE>();
                    Session["ListaUnidade"] = null;
                    Session["IdUnidade"] = item.UNID_CD_ID;
                    return RedirectToAction("MontarTelaUnidade");
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
        public ActionResult EditarUnidade(Int32 id)
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
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            UNIDADE item = unApp.GetItemById(id);
            Session["Unidade"] = item;

            // Indicadores
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Medida", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Serviço", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Mensagens
            if (Session["MensUnidade"] != null)
            {


            }

            objetoUNAntes = item;
            Session["IdUnidade"] = id;
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarUnidade(UnidadeViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Medida", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Serviço", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    UNIDADE item = Mapper.Map<UnidadeViewModel, UNIDADE>(vm);
                    Int32 volta = unApp.ValidateEdit(item, objetoUNAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterUN = new List<UNIDADE>();
                    Session["ListaUnidade"] = null;
                    return RedirectToAction("MontarTelaUnidade");
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

        public ActionResult VoltarAnexoUnidade()
        {

            return RedirectToAction("EditarUnidade", new { id = (Int32)Session["IdUnidade"] });
        }

        [HttpGet]
        public ActionResult ExcluirUnidade(Int32 id)
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
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarUnidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            UNIDADE item = unApp.GetItemById(id);
            objetoUNAntes = (UNIDADE)Session["Unidade"];
            item.UNID_IN_ATIVO = 0;
            Int32 volta = unApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensUnidade"] = 4;
                return RedirectToAction("MontarTelaUnidade");
            }
            Session["ListaUnidade"] = null;
            return RedirectToAction("MontarTelaUnidade");
        }
        [HttpGet]
        public ActionResult ReativarUnidade(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VEN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            UNIDADE item = unApp.GetItemById(id);
            objetoUNAntes = (UNIDADE)Session["Unidade"];
            item.UNID_IN_ATIVO = 1;
            Int32 volta = unApp.ValidateReativar(item, usuario);
            Session["ListaUnidade"] = null;
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpGet]
        public ActionResult MontarTelaGrupoCC()
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
            if ((List<GRUPO_CC>)Session["ListaGrupoCC"] == null)
            {
                listaMasterGR = grApp.GetAllItens(idAss);
                Session["ListaGrupoCC"] = listaMasterGR;
            }
            ViewBag.Listas = (List<GRUPO_CC>)Session["ListaGrupoCC"];
            Session["GrupoCC"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensGrupoCC"] != null)
            {
                if ((Int32)Session["MensGrupoCC"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupoCC"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0115", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupoCC"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaGrupoCC"] = 1;
            objetoGR = new GRUPO_CC();
            return View(objetoGR);
        }

        public ActionResult RetirarFiltroGrupoCC()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaGrupoCC"] = null;
            return RedirectToAction("MontarTelaGrupoCC");
        }

        public ActionResult MostrarTudoGrupoCC()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterGR = grApp.GetAllItensAdm(idAss);
            Session["ListaGrupoCC"] = listaMasterGR;
            return RedirectToAction("MontarTelaGrupoCC");
        }

        public ActionResult VoltarBaseGrupoCC()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaGrupoCC");
        }

        [HttpGet]
        public ActionResult IncluirGrupoCC()
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
                    Session["MensGrupoCC"] = 2;
                    return RedirectToAction("MontarTelaGrupoCC");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            GRUPO_CC item = new GRUPO_CC();
            GrupoCCViewModel vm = Mapper.Map<GRUPO_CC, GrupoCCViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.GRCC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirGrupoCC(GrupoCCViewModel vm)
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
                    GRUPO_CC item = Mapper.Map<GrupoCCViewModel, GRUPO_CC>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = grApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensGrupoCC"] = 3;
                        return RedirectToAction("MontarTelaGrupoCC");
                    }

                    // Sucesso
                    listaMasterGR = new List<GRUPO_CC>();
                    Session["ListaGrupoCC"] = null;
                    Session["IdGrupoCC"] = item.GRCC_CD_ID;
                    if ((Int32)Session["VoltaGrupoCC"] == 2)
                    {
                        return RedirectToAction("IncluirCC", "CentroCusto");
                    }
                    return RedirectToAction("MontarTelaGrupoCC");
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
        public ActionResult EditarGrupoCC(Int32 id)
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
                    Session["MensGrupoCC"] = 2;
                    return RedirectToAction("MontarTelaGrupoCC");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO_CC item = grApp.GetItemById(id);
            Session["GrupoCC"] = item;

            // Indicadores
            
            // Mensagens
            if (Session["MensGrupoCC"] != null)
            {


            }

            objetoGRAntes = item;
            Session["IdGrupoCC"] = id;
            GrupoCCViewModel vm = Mapper.Map<GRUPO_CC, GrupoCCViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarGrupoCC(GrupoCCViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    GRUPO_CC item = Mapper.Map<GrupoCCViewModel, GRUPO_CC>(vm);
                    Int32 volta = grApp.ValidateEdit(item, objetoGRAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterGR = new List<GRUPO_CC>();
                    Session["ListaGrupoCC"] = null;
                    return RedirectToAction("MontarTelaGrupoCC");
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

        public ActionResult VoltarAnexoGrupoCC()
        {

            return RedirectToAction("EditarGrupoCC", new { id = (Int32)Session["IdGrupoCC"] });
        }

        [HttpGet]
        public ActionResult ExcluirGrupoCC(Int32 id)
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
                    Session["MensGrupoCC"] = 2;
                    return RedirectToAction("MontarTelaGrupoCC");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO_CC item = grApp.GetItemById(id);
            objetoGRAntes = (GRUPO_CC)Session["GrupoCC"];
            item.GRCC_IN_ATIVO = 0;
            Int32 volta = grApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensGrupoCC"] = 4;
                return RedirectToAction("MontarTelaGrupoCC");
            }
            Session["ListaGrupoCC"] = null;
            return RedirectToAction("MontarTelaGrupoCC");
        }

        [HttpGet]
        public ActionResult ReativarGrupoCC(Int32 id)
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
                    Session["MensGrupoCC"] = 2;
                    return RedirectToAction("MontarTelaGrupoCC");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO_CC item = grApp.GetItemById(id);
            objetoGRAntes = (GRUPO_CC)Session["GrupoCC"];
            item.GRCC_IN_ATIVO = 1;
            Int32 volta = grApp.ValidateReativar(item, usuario);
            Session["ListaGrupoCC"] = null;
            return RedirectToAction("MontarTelaGrupoCC");
        }

        [HttpGet]
        public ActionResult MontarTelaSubgrupo()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
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
            if ((List<SUBGRUPO>)Session["ListaSubgrupo"] == null)
            {
                listaMasterSG = sgApp.GetAllItens(idAss);
                Session["ListaSubgrupo"] = listaMasterSG;
            }
            ViewBag.Listas = (List<SUBGRUPO>)Session["ListaSubgrupo"];
            ViewBag.Cats = new SelectList(grApp.GetAllItens(idAss).OrderBy(x => x.GRCC_NM_NOME), "GRCC_CD_ID", "GRCC_NM_NOME");
            Session["Subgrupo"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensSubgrupo"] != null)
            {
                if ((Int32)Session["MensSubgrupo"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubgrupo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0116", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubgrupo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0117", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaSubgrupo"] = 1;
            objetoSG = new SUBGRUPO();
            return View(objetoSG);
        }

        public ActionResult RetirarFiltroSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public ActionResult MostrarTudoSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSG = sgApp.GetAllItensAdm(idAss);
            Session["ListaSubgrupo"] = listaMasterSG;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public ActionResult VoltarBaseSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult IncluirSubgrupo()
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
                    Session["MensSubgrupo"] = 2;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(grApp.GetAllItens(idAss).OrderBy(x => x.GRCC_NM_NOME), "GRCC_CD_ID", "GRCC_NM_NOME");
            
            // Prepara view
            SUBGRUPO item = new SUBGRUPO();
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO, SubgrupoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.SUBG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirSubgrupo(SubgrupoViewModel vm)
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
                    SUBGRUPO item = Mapper.Map<SubgrupoViewModel, SUBGRUPO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = sgApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensSubgrupo"] = 3;
                        return RedirectToAction("MontarTelaSubgrupo");
                    }

                    // Sucesso
                    listaMasterSG = new List<SUBGRUPO>();
                    Session["ListaSubgrupo"] = null;
                    Session["IdSubgrupo"] = item.SUBG_CD_ID;
                    if ((Int32)Session["VoltaSubgrupo"] == 2)
                    {
                        return RedirectToAction("IncluirCC", "CentroCusto");
                    }
                    return RedirectToAction("MontarTelaSubgrupo");
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
        public ActionResult EditarSubgrupo(Int32 id)
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
                    Session["MensSubgrupo"] = 2;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBGRUPO item = sgApp.GetItemById(id);
            Session["Subgrupo"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensSubgrupo"] != null)
            {


            }

            objetoSGAntes = item;
            Session["IdSubgrupo"] = id;
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO, SubgrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarSubgrupo(SubgrupoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    SUBGRUPO item = Mapper.Map<SubgrupoViewModel, SUBGRUPO>(vm);
                    Int32 volta = sgApp.ValidateEdit(item, objetoSGAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSG = new List<SUBGRUPO>();
                    Session["ListaSubgrupo"] = null;
                    return RedirectToAction("MontarTelaSubgrupo");
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

        public ActionResult VoltarAnexoSubgrupo()
        {

            return RedirectToAction("EditarSubgrupo", new { id = (Int32)Session["IdSubgrupo"] });
        }

        [HttpGet]
        public ActionResult ExcluirSubgrupo(Int32 id)
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
                    Session["MensSubgrupo"] = 2;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBGRUPO item = sgApp.GetItemById(id);
            objetoSGAntes = (SUBGRUPO)Session["Subgrupo"];
            item.SUBG_IN_ATIVO = 0;
            Int32 volta = sgApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensSubgrupo"] = 4;
                return RedirectToAction("MontarTelaSubgrupo");
            }
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult ReativarSubgrupo(Int32 id)
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
                    Session["MensSubgrupo"] = 2;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBGRUPO item = sgApp.GetItemById(id);
            objetoSGAntes = (SUBGRUPO)Session["Subgrupo"];
            item.SUBG_IN_ATIVO = 1;
            Int32 volta = sgApp.ValidateReativar(item, usuario);
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult MontarTelaCatAtendimento()
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
            if ((List<CATEGORIA_ATENDIMENTO>)Session["ListaCatAtendimento"] == null)
            {
                listaMasterCAT = catApp.GetAllItens(idAss);
                Session["ListaCatAtendimento"] = listaMasterCAT;
            }
            ViewBag.Listas = (List<CATEGORIA_ATENDIMENTO>)Session["ListaCatAtendimento"];
            Session["CatAtendimento"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCatAtendimento"] != null)
            {
                if ((Int32)Session["MensCatAtendimento"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatAtendimento"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0066", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatAtendimento"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0069", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatAtendimento"] = 1;
            objetoCAT = new CATEGORIA_ATENDIMENTO();
            return View(objetoCAT);
        }

        public ActionResult RetirarFiltroCatAtendimento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCatAtendimento"] = null;
            return RedirectToAction("MontarTelaCatAtendimento");
        }

        public ActionResult MostrarTudoCatAtendimento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCAT = catApp.GetAllItensAdm(idAss);
            Session["ListaCatAtendimento"] = listaMasterCAT;
            return RedirectToAction("MontarTelaCatAtendimento");
        }

        public ActionResult VoltarBaseCatAtendimento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatAtendimento");
        }

        [HttpGet]
        public ActionResult IncluirCatAtendimento()
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
                    Session["MensCatAtendimento"] = 2;
                    return RedirectToAction("MontarTelaCatAtendimento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            CATEGORIA_ATENDIMENTO item = new CATEGORIA_ATENDIMENTO();
            CategoriaAtendimentoViewModel vm = Mapper.Map<CATEGORIA_ATENDIMENTO, CategoriaAtendimentoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CAAT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCatAtendimento(CategoriaAtendimentoViewModel vm)
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
                    CATEGORIA_ATENDIMENTO item = Mapper.Map<CategoriaAtendimentoViewModel, CATEGORIA_ATENDIMENTO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = catApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatAtendimento"] = 3;
                        return RedirectToAction("MontarTelaCatAtendimento");
                    }

                    // Sucesso
                    listaMasterCAT = new List<CATEGORIA_ATENDIMENTO>();
                    Session["ListaCatAtendimento"] = null;
                    Session["IdCatAtendimento"] = item.CAAT_CD_ID;
                    return RedirectToAction("MontarTelaCatAtendimento");
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
        public ActionResult EditarCatAtendimento(Int32 id)
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
                    Session["MensCatAtendimento"] = 2;
                    return RedirectToAction("MontarTelaCatAtendimento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_ATENDIMENTO item = catApp.GetItemById(id);
            Session["CatAtendimento"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensCatAtendimento"] != null)
            {


            }

            objetoCATAntes = item;
            Session["IdCatAtendimento"] = id;
            CategoriaAtendimentoViewModel vm = Mapper.Map<CATEGORIA_ATENDIMENTO, CategoriaAtendimentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatAtendimento(CategoriaAtendimentoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_ATENDIMENTO item = Mapper.Map<CategoriaAtendimentoViewModel, CATEGORIA_ATENDIMENTO>(vm);
                    Int32 volta = catApp.ValidateEdit(item, objetoCATAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCAT = new List<CATEGORIA_ATENDIMENTO>();
                    Session["ListaCatAtendimento"] = null;
                    return RedirectToAction("MontarTelaCatAtendimento");
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

        public ActionResult VoltarAnexoCatAtendimento()
        {

            return RedirectToAction("EditarCatAtendimento", new { id = (Int32)Session["IdCatAtendimento"] });
        }

        [HttpGet]
        public ActionResult ExcluirCatAtendimento(Int32 id)
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
                    Session["MensCatAtendimento"] = 2;
                    return RedirectToAction("MontarTelaCatAtendimento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_ATENDIMENTO item = catApp.GetItemById(id);
            objetoCATAntes = (CATEGORIA_ATENDIMENTO)Session["CatAtendimento"];
            item.CAAT_IN_ATIVO = 0;
            Int32 volta = catApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatAtendimento"] = 4;
                return RedirectToAction("MontarTelaCatAtendimento");
            }
            Session["ListaCatAtendimento"] = null;
            return RedirectToAction("MontarTelaCatAtendimento");
        }

        [HttpGet]
        public ActionResult ReativarCatAtendimento(Int32 id)
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
                    Session["MensCatAtendimento"] = 2;
                    return RedirectToAction("MontarTelaCatAtendimento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_ATENDIMENTO item = catApp.GetItemById(id);
            objetoCATAntes = (CATEGORIA_ATENDIMENTO)Session["CatAtendimento"];
            item.CAAT_IN_ATIVO = 1;
            Int32 volta = catApp.ValidateReativar(item, usuario);
            Session["ListaCatAtendimento"] = null;
            return RedirectToAction("MontarTelaCatAtendimento");
        }

        [HttpGet]
        public ActionResult MontarTelaDepartamento()
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
            if ((List<DEPARTAMENTO>)Session["ListaDepartamento"] == null)
            {
                listaMasterDEP = depApp.GetAllItens(idAss);
                Session["ListaDepartamento"] = listaMasterDEP;
            }
            ViewBag.Listas = (List<DEPARTAMENTO>)Session["ListaDepartamento"];
            Session["Departamento"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensDepartamento"] != null)
            {
                if ((Int32)Session["MensDepartamento"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensDepartamento"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0120", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensDepartamento"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0121", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaDepartamento"] = 1;
            objetoDEP = new DEPARTAMENTO();
            return View(objetoDEP);
        }

        public ActionResult RetirarFiltroDepartamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaDepartamento"] = null;
            return RedirectToAction("MontarTelaDepartamento");
        }

        public ActionResult MostrarTudoDepartamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterDEP = depApp.GetAllItensAdm(idAss);
            Session["ListaDepartamento"] = listaMasterDEP;
            return RedirectToAction("MontarTelaDepartamento");
        }

        public ActionResult VoltarBaseDepartamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaDepartamento");
        }

        [HttpGet]
        public ActionResult IncluirDepartamento()
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
                    Session["MensDepartamento"] = 2;
                    return RedirectToAction("MontarTelaDepartamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            DEPARTAMENTO item = new DEPARTAMENTO();
            DepartamentoViewModel vm = Mapper.Map<DEPARTAMENTO, DepartamentoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.DEPT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirDepartamento(DepartamentoViewModel vm)
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
                    DEPARTAMENTO item = Mapper.Map<DepartamentoViewModel, DEPARTAMENTO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = depApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensDepartamento"] = 3;
                        return RedirectToAction("MontarTelaDepartamento");
                    }

                    // Sucesso
                    listaMasterDEP = new List<DEPARTAMENTO>();
                    Session["ListaDepartamento"] = null;
                    Session["IdDepartamento"] = item.DEPT_CD_ID;
                    return RedirectToAction("MontarTelaDepartamento");
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
        public ActionResult EditarDepartamento(Int32 id)
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
                    Session["MensDepartamento"] = 2;
                    return RedirectToAction("MontarTelaDepartamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            DEPARTAMENTO item = depApp.GetItemById(id);
            Session["Departamento"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensDepartamento"] != null)
            {


            }

            objetoDEPAntes = item;
            Session["IdDepartamento"] = id;
            DepartamentoViewModel vm = Mapper.Map<DEPARTAMENTO, DepartamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarDepartamento(DepartamentoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    DEPARTAMENTO item = Mapper.Map<DepartamentoViewModel, DEPARTAMENTO>(vm);
                    Int32 volta = depApp.ValidateEdit(item, objetoDEPAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterDEP = new List<DEPARTAMENTO>();
                    Session["ListaDepartamento"] = null;
                    return RedirectToAction("MontarTelaDepartamento");
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

        public ActionResult VoltarAnexoDepartamento()
        {

            return RedirectToAction("EditarDepartamento", new { id = (Int32)Session["IdDepartamento"] });
        }

        [HttpGet]
        public ActionResult ExcluirDepartamento(Int32 id)
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
                    Session["MensDepartamento"] = 2;
                    return RedirectToAction("MontarTelaDepartamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            DEPARTAMENTO item = depApp.GetItemById(id);
            objetoDEPAntes = (DEPARTAMENTO)Session["Departamento"];
            item.DEPT_IN_ATIVO = 0;
            Int32 volta = depApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensDepartamento"] = 4;
                return RedirectToAction("MontarTelaDepartamento");
            }
            Session["ListaDepartamento"] = null;
            return RedirectToAction("MontarTelaDepartamento");
        }

        [HttpGet]
        public ActionResult ReativarDepartamento(Int32 id)
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
                    Session["MensDepartamento"] = 2;
                    return RedirectToAction("MontarTelaDepartamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            DEPARTAMENTO item = depApp.GetItemById(id);
            objetoDEPAntes = (DEPARTAMENTO)Session["Departamento"];
            item.DEPT_IN_ATIVO = 1;
            Int32 volta = depApp.ValidateReativar(item, usuario);
            Session["ListaDepartamento"] = null;
            return RedirectToAction("MontarTelaDepartamento");
        }

        [HttpGet]
        public ActionResult MontarTelaCatServico()
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
            if ((List<CATEGORIA_SERVICO>)Session["ListaCatServico"] == null)
            {
                listaMasterSER = serApp.GetAllItens(idAss);
                Session["ListaCatServico"] = listaMasterSER;
            }
            ViewBag.Listas = (List<CATEGORIA_SERVICO>)Session["ListaCatServico"];
            Session["CatServico"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCatServico"] != null)
            {
                if ((Int32)Session["MensCatServico"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatServico"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0066", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatServico"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0069", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatServico"] = 1;
            objetoSER = new CATEGORIA_SERVICO();
            return View(objetoSER);
        }

        public ActionResult RetirarFiltroCatServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCatServico"] = null;
            return RedirectToAction("MontarTelaCatServico");
        }

        public ActionResult MostrarTudoCatServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSER = serApp.GetAllItensAdm(idAss);
            Session["ListaCatServico"] = listaMasterSER;
            return RedirectToAction("MontarTelaCatServico");
        }

        public ActionResult VoltarBaseCatServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatServico");
        }

        [HttpGet]
        public ActionResult IncluirCatServico()
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
                    Session["MensCatServico"] = 2;
                    return RedirectToAction("MontarTelaCatServico");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            CATEGORIA_SERVICO item = new CATEGORIA_SERVICO();
            CategoriaServicoViewModel vm = Mapper.Map<CATEGORIA_SERVICO, CategoriaServicoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CASE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCatServico(CategoriaServicoViewModel vm)
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
                    CATEGORIA_SERVICO item = Mapper.Map<CategoriaServicoViewModel, CATEGORIA_SERVICO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = serApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatServico"] = 3;
                        return RedirectToAction("MontarTelaCatServico");
                    }

                    // Sucesso
                    listaMasterSER = new List<CATEGORIA_SERVICO>();
                    Session["ListaCatServico"] = null;
                    Session["IdCatServico"] = item.CASE_CD_ID;
                    return RedirectToAction("MontarTelaCatServico");
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
        public ActionResult EditarCatServico(Int32 id)
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
                    Session["MensCatServico"] = 2;
                    return RedirectToAction("MontarTelaCatServico");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_SERVICO item = serApp.GetItemById(id);
            Session["CatServico"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensCatServico"] != null)
            {


            }

            objetoSERAntes = item;
            Session["IdCatServico"] = id;
            CategoriaServicoViewModel vm = Mapper.Map<CATEGORIA_SERVICO, CategoriaServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatServico(CategoriaServicoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_SERVICO item = Mapper.Map<CategoriaServicoViewModel, CATEGORIA_SERVICO>(vm);
                    Int32 volta = serApp.ValidateEdit(item, objetoSERAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSER = new List<CATEGORIA_SERVICO>();
                    Session["ListaCatServico"] = null;
                    return RedirectToAction("MontarTelaCatServico");
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

        public ActionResult VoltarAnexoCatServico()
        {

            return RedirectToAction("EditarCatServico", new { id = (Int32)Session["IdCatServico"] });
        }

        [HttpGet]
        public ActionResult ExcluirCatServico(Int32 id)
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
                    Session["MensCatServico"] = 2;
                    return RedirectToAction("MontarTelaCatServico");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_SERVICO item = serApp.GetItemById(id);
            objetoSERAntes = (CATEGORIA_SERVICO)Session["CatServico"];
            item.CASE_IN_ATIVO = 0;
            Int32 volta = serApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatServico"] = 4;
                return RedirectToAction("MontarTelaCatServico");
            }
            Session["ListaCatServico"] = null;
            return RedirectToAction("MontarTelaCatServico");
        }

        [HttpGet]
        public ActionResult ReativarCatServico(Int32 id)
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
                    Session["MensCatServico"] = 2;
                    return RedirectToAction("MontarTelaCatServico");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_SERVICO item = serApp.GetItemById(id);
            objetoSERAntes = (CATEGORIA_SERVICO)Session["CatServico"];
            item.CASE_IN_ATIVO = 1;
            Int32 volta = serApp.ValidateReativar(item, usuario);
            Session["ListaCatServico"] = null;
            return RedirectToAction("MontarTelaCatServico");
        }

        [HttpGet]
        public ActionResult MontarTelaCatOS()
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
            if ((List<CATEGORIA_ORDEM_SERVICO>)Session["ListaCatOS"] == null)
            {
                listaMasterOS= osApp.GetAllItens(idAss);
                Session["ListaCatOS"] = listaMasterOS;
            }
            ViewBag.Listas = (List<CATEGORIA_ORDEM_SERVICO>)Session["ListaCatOS"];
            Session["CatOS"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCatOS"] != null)
            {
                if ((Int32)Session["MensCatOS"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatOS"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0066", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatOS"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0069", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatOS"] = 1;
            objetoOS= new CATEGORIA_ORDEM_SERVICO();
            return View(objetoOS);
        }

        public ActionResult RetirarFiltroCatOS()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCatOS"] = null;
            return RedirectToAction("MontarTelaCatOS");
        }

        public ActionResult MostrarTudoCatOS()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterOS = osApp.GetAllItensAdm(idAss);
            Session["ListaCatOS"] = listaMasterOS;
            return RedirectToAction("MontarTelaCatOS");
        }

        public ActionResult VoltarBaseCatOS()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatOS");
        }

        [HttpGet]
        public ActionResult IncluirCatOS()
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
                    Session["MensCatOS"] = 2;
                    return RedirectToAction("MontarTelaCatOS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            CATEGORIA_ORDEM_SERVICO item = new CATEGORIA_ORDEM_SERVICO();
            CategoriaOrdemServicoViewModel vm = Mapper.Map<CATEGORIA_ORDEM_SERVICO, CategoriaOrdemServicoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CAOS_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCatOS(CategoriaOrdemServicoViewModel vm)
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
                    CATEGORIA_ORDEM_SERVICO item = Mapper.Map<CategoriaOrdemServicoViewModel, CATEGORIA_ORDEM_SERVICO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = osApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatOS"] = 3;
                        return RedirectToAction("MontarTelaCatOS");
                    }

                    // Sucesso
                    listaMasterOS= new List<CATEGORIA_ORDEM_SERVICO>();
                    Session["ListaCatOS"] = null;
                    Session["IdCatOS"] = item.CAOS_CD_ID;
                    return RedirectToAction("MontarTelaCatOS");
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
        public ActionResult EditarCatOS(Int32 id)
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
                    Session["MensCatOS"] = 2;
                    return RedirectToAction("MontarTelaCatOS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_ORDEM_SERVICO item = osApp.GetItemById(id);
            Session["CatOS"] = item;

            // Indicadores

            // Mensagens
            if (Session["MensCatOS"] != null)
            {


            }

            objetoOSAntes = item;
            Session["IdCatOS"] = id;
            CategoriaOrdemServicoViewModel vm = Mapper.Map<CATEGORIA_ORDEM_SERVICO, CategoriaOrdemServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatOS(CategoriaOrdemServicoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_ORDEM_SERVICO item = Mapper.Map<CategoriaOrdemServicoViewModel, CATEGORIA_ORDEM_SERVICO>(vm);
                    Int32 volta = osApp.ValidateEdit(item, objetoOSAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterOS = new List<CATEGORIA_ORDEM_SERVICO>();
                    Session["ListaCatOS"] = null;
                    return RedirectToAction("MontarTelaCatOS");
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

        public ActionResult VoltarAnexoCatOS()
        {

            return RedirectToAction("EditarCatOS", new { id = (Int32)Session["IdCatOS"] });
        }

        [HttpGet]
        public ActionResult ExcluirCatOS(Int32 id)
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
                    Session["MensCatOS"] = 2;
                    return RedirectToAction("MontarTelaCatOS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_ORDEM_SERVICO item = osApp.GetItemById(id);
            objetoOSAntes = (CATEGORIA_ORDEM_SERVICO)Session["CatOS"];
            item.CAOS_IN_ATIVO = 0;
            Int32 volta = osApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatOS"] = 4;
                return RedirectToAction("MontarTelaCatOS");
            }
            Session["ListaCatOS"] = null;
            return RedirectToAction("MontarTelaCatOS");
        }

        [HttpGet]
        public ActionResult ReativarCatOS(Int32 id)
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
                    Session["MensCatOS"] = 2;
                    return RedirectToAction("MontarTelaCatOS");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CATEGORIA_ORDEM_SERVICO item = osApp.GetItemById(id);
            objetoOSAntes = (CATEGORIA_ORDEM_SERVICO)Session["CatOS"];
            item.CAOS_IN_ATIVO = 1;
            Int32 volta = osApp.ValidateReativar(item, usuario);
            Session["ListaCatOS"] = null;
            return RedirectToAction("MontarTelaCatOS");
        }



    }
}